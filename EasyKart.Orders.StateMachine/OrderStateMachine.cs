using EasyKart.Shared.Commands;
using EasyKart.Shared.Events;
using EasyKart.Shared.Models;
using MassTransit;

namespace EasyKart.Orders.StateMachine
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State OrderCreated { get; set; }
        public State InventoryReserverd { get; set; }

        public Event<OrderCreatedEvent> OrderCreatedEvent { get; set; }
        public Event<InventoryReservedEvent> InventoryReservedEvent { get; set; }
        public Event<InventoryOutOfStockEvent> InventoryOutOfStockEvent { get; set; }

        public OrderStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderCreatedEvent, x =>
            {
                x.CorrelateById(context => context.Message.Order.Id);
            });
            Event(() => InventoryReservedEvent, x =>
            {
                x.CorrelateById(context => context.Message.Order.Id);
            });
            Event(() => InventoryOutOfStockEvent, x =>
            {
                x.CorrelateById(context => context.Message.Order.Id);
            });

            Initially(
                When(OrderCreatedEvent)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.Order.Id;
                        context.Saga.Created = context.Message.Order.CreatedDate;
                        context.Saga.CurrentState = nameof(OrderCreated);
                    })
                    .Publish(context => new ReserveInventoryCommand() { Order = context.Message.Order })
                    .TransitionTo(OrderCreated)
            );

            During(OrderCreated,
                When(InventoryReservedEvent)
                    .Publish(context => {
                        Console.WriteLine("InventoryReservedEvent");
                        return new MakePaymentCommand()
                        {
                            Order = context.Message.Order,
                            CreatedAt = DateTime.Now
                        };
                    })
                    .Publish(context => {
                        Console.WriteLine("InventoryReservedEvent");
                        Order o = context.Message.Order;
                        o.Status = "InventoryReserved";
                        return new UpdateOrderCommand()
                        {
                            Order = o,
                            CreatedAt = DateTime.Now
                        };
                    })
                    .TransitionTo(InventoryReserverd),

              
                When(InventoryOutOfStockEvent)
                    .Publish(context => {
                        Console.WriteLine("InventoryOutOfStockEvent");
                        Order o = context.Message.Order;
                        o.Status = "InventoryOutOfStock";
                        return new UpdateOrderCommand()
                        {
                            Order = o,
                            CreatedAt = DateTime.Now
                        };
                    })
                    .TransitionTo(InventoryReserverd)
     
            );
        }
    }
}
