using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

class Subscriber
{
    static async Task Main(string[] args)
    {

        var factory = new ConnectionFactory()
        {
            Uri = new Uri("amqps://qycjilrn:TFvVbNHx0l5B29sHVg9PBY6KyqePfPlT@fuji.lmq.cloudamqp.com/qycjilrn")
        };

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Criação da fila
        channel.QueueDeclare(queue: "emailQueue",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine("Mensagem recebida: {0}", message);

            // Monta o JSON no formato que a API espera
            var jsonBody = $"{{ \"endereco\": \"{message}\" }}";
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            // Configura para ignorar problemas de certificado SSL (apenas para desenvolvimento)
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            using var httpClient = new HttpClient(handler);
            var response = await httpClient.PostAsync("https://localhost:7272/api/emails", content);


            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Email salvo com sucesso na API.");
            }
            else
            {
                Console.WriteLine($"Falha ao salvar email. Status: {response.StatusCode}");
            }
        };

        channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);

        Console.WriteLine("Consumidor aguardando mensagens. Pressione Enter para sair...");
        Console.ReadLine();
    }
}
