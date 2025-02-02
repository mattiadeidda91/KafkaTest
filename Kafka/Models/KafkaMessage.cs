﻿using Confluent.Kafka;

namespace Kafka.Models
{
    public class KafkaMessage<T> :
        Message<string,T>
    {
        public KafkaMessage()
        {
            this.Key = Guid.NewGuid().ToString();
        }

        public KafkaMessage(T message):
            this()
        {
            this.Value = message;
        }
    }
}
