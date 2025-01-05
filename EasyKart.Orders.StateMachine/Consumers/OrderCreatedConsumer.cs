using EasyKart.Shared.Events;
using MassTransit;

namespace EasyKart.Orders.StateMachine.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        public Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            Console.WriteLine($"Order received {context.Message.Order.Id}");
            return Task.CompletedTask;
        }
    }
}
