using Common.Interfaces;

namespace Common.Models.Order
{
    public class OrderStartedEvent : BaseEvent<Order>, IEvent
    {
    }
}
