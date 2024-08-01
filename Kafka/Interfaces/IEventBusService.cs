using Common.Models;
using Confluent.Kafka;
using Kafka.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kafka.Interfaces
{
    public interface IEventBusService
    {
        Task<DeliveryResult<string, T>> SendMessage<T>(T message, CancellationToken cancellationToken, string? key = null, Headers? headers = null);
        Task Subscribe<T>(Action<ConsumeResult<string, T>> action, string? groupId =null);
        Task Subscribe(Action<ConsumeResult<string, string>> action, string? groupId = null);
        (TEvent, Headers) GenerateMsgBusEvent<TEvent, TActivity>(TActivity activity) where TEvent : BaseEvent<TActivity>, new() where TActivity : class;
        public ActionResult<MessageBusResponse<TDto>> ManageBusResponse<TEvent, TDto>(ControllerBase controller, DeliveryResult<string, TEvent> deliveryResult, TDto? activityRequest)
                where TDto : BaseDto
                where TEvent : BaseEvent<TDto>;
    }
}