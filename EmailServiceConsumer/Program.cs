using EmailServiceConsumer.Email;
using EmailServiceConsumer.Eventos.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Serialization;

using IHost host = Host.CreateDefaultBuilder(args).Build();
IConfiguration config = host.Services.GetRequiredService <IConfiguration>();

var server = config.GetValue<string>("Rabbit:Server");
var user = config.GetValue<string>("Rabbit:UserName");
var password = config.GetValue<string>("Rabbit:Password");
var queue = config.GetValue<string>("Rabbit:Queue");
var exchange = config.GetValue<string>("Rabbit:Exchange");
var routingkey = config.GetValue<string>("Rabbit:RoutingKey");

var factory = new ConnectionFactory()
{
    HostName = server,
    UserName = user,
    Password = password,
};

var conn = factory.CreateConnection();
using var channel = conn.CreateModel();

channel.QueueDeclare(queue, true, false, true);
channel.ExchangeDeclare(exchange, "topic", true, true);
channel.QueueBind(queue, exchange, routingkey);

var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

consumer.Received += ConsumirMensagem;

Console.ReadKey();


void ConsumirMensagem(object sender, BasicDeliverEventArgs e)
{
    var eventoString = Encoding.UTF8.GetString(e.Body.ToArray());
    var evento = JsonConvert.DeserializeObject<EsqueciSenhaModel>(eventoString);

    var emailService = new EmailService();
    emailService.EnviarEmail("Sistema Var", evento.Email, evento.Assunto, evento.Texto);

}





