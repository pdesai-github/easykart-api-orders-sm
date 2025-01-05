
using EasyKart.Orders.StateMachine.Consumers;
using EasyKart.Shared.Commands;
using EasyKart.Shared.Events;
using MassTransit;

namespace EasyKart.Orders.StateMachine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string conn = builder.Configuration.GetConnectionString("AzureServiceBus");

            // Add services to the container.
            builder.Services.AddMassTransit(x =>
            {
                x.AddSagaStateMachine<OrderStateMachine, OrderState>()
                    .InMemoryRepository();

                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(conn);

                    cfg.Message<OrderCreatedEvent>(x =>
                    {
                        x.SetEntityName("ordercreated");
                    });
                    cfg.Message<ReserveInventoryCommand>(x =>
                    {
                        x.SetEntityName("ordercreated");
                    });
                    cfg.Message<MakePaymentCommand>(x =>
                    {
                        x.SetEntityName("ordercreated");
                    });
                    cfg.Message<UpdateOrderCommand>(x =>
                    {
                        x.SetEntityName("ordercreated");
                    });
                    cfg.Message<InventoryReservedEvent>(x =>
                    {
                        x.SetEntityName("ordercreated");
                    });
                    cfg.Message<InventoryOutOfStockEvent>(x =>
                    {
                        x.SetEntityName("ordercreated");
                    });

                    cfg.SubscriptionEndpoint<OrderCreatedEvent>("OrderCreatedSubscription", e =>
                    {
                        e.ConfigureSaga<OrderState>(context);
                    });
                    cfg.SubscriptionEndpoint<InventoryReservedEvent>("InventoryReservedSubscription", e =>
                    {
                        e.ConfigureSaga<OrderState>(context);
                    });
                    cfg.SubscriptionEndpoint<InventoryOutOfStockEvent>("InventoryOutOfStockEventSubscription", e =>
                    {
                        e.ConfigureSaga<OrderState>(context);
                    });

                    //cfg.Message<OrderCreated>(x =>
                    //{
                    //    x.SetEntityName("ordercreated");
                    //});
                    //cfg.Message<InventoryReserved>(x =>
                    //{
                    //    x.SetEntityName("ordercreated");
                    //});
                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
