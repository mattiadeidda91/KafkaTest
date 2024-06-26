using Microsoft.Extensions.Logging;

namespace Common.Logger
{
    public static partial class LogMessageProducer
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "Producer successfully sent the Kafka message '{Message}'")]
        public static partial void LogInfoKafkaMessageSent(this ILogger logger, string message);
    }
}
