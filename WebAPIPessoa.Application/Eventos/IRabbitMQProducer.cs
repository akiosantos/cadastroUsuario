using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Eventos
{
    public interface IRabbitMQProducer
    {
        void EnviarMensagem<T>(T message, string queue, string exchange, string routingkey);
    }
}
