using Common.Interfaces;
using Common.Models.Order;
using System.Text.Json;

namespace Consumer.Handlers
{
    public class OrderStartedHandler : IEventHandler<OrderStartedEvent>
    {
        public Task HandleAsync(OrderStartedEvent @event)
        {
            Console.WriteLine($"Handling OrderStartedEvent: {@event.ActivityId}, {JsonSerializer.Serialize(@event.Activity)}");

            return Task.CompletedTask;
        }
    }
}
