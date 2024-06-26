using Common.Models;
using Confluent.Kafka;

namespace Kafka.Interfaces
{
    public interface IEventBusService
    {
        Task<DeliveryResult<string, T>> SendMessage<T>(T message, string? key = null, Headers? headers = null);
        Task Subscribe<T>(Action<ConsumeResult<string, T>> action, string? groupId =null);
        Task Subscribe(Action<ConsumeResult<string, string>> action, string? groupId = null);
        (TEvent, Headers) GenerateMsgBusEvent<TEvent, TActivity>(TActivity activity) where TEvent : BaseEvent<TActivity>, new() where TActivity : class;

    }
}