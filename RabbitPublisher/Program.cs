using RabbitMQ.Client;
using System.Text;

class Publisher
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri("amqps://qycjilrn:TFvVbNHx0l5B29sHVg9PBY6KyqePfPlT@fuji.lmq.cloudamqp.com/qycjilrn")
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Criação da fila (se já existir, ignora)
        channel.QueueDeclare(queue: "emailQueue",
                             durable: true,        // durable = true para sobreviver a reinicializações
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        string message = "ThiagoThailon@teste.com";
        var body = Encoding.UTF8.GetBytes(message);

        // Publica a mensagem na fila
        channel.BasicPublish(exchange: "",
                             routingKey: "emailQueue",
                             basicProperties: null,
                             body: body);

        Console.WriteLine("Mensagem enviada para a fila: {0}", message);
    }
}
