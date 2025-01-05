using MassTransit;

namespace EasyKart.Orders.StateMachine
{
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public Guid OrderId { get; set; }
        public DateTime Created { get; set; }
    }
}
