using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AdminServer;

public class PurchaseConsumer
{
    private readonly IModel _channel;
    private readonly string _queueName;
    private string? _consumerTag;


    public PurchaseConsumer()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        _queueName = _channel.QueueDeclare().QueueName;
        
        _channel.QueueBind(queue: _queueName, exchange: "admin-notifications", routingKey: "");
    }
    
    public void StartListening(int nPurchases)
    {
        Console.WriteLine($"[RabbitMQ] Escuchando las próximas {nPurchases} compras...");

        var consumer = new EventingBasicConsumer(_channel);
        int purchaseCount = 0;

        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine($"[RabbitMQ] Compra recibida: {message}");
            purchaseCount++;

            if (purchaseCount >= nPurchases)
            {
                Console.WriteLine($"[RabbitMQ] Se han recibido {nPurchases} compras. Deteniendo consumidor...");
                StopListening();
            }
        }; 
        
        _consumerTag = _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
    }
    
    public void StopListening()
    {
        if (_consumerTag != null)
        {
            _channel.BasicCancel(_consumerTag);
            Console.WriteLine($"[RabbitMQ] Consumer with tag {_consumerTag} stopped.");
        }
    }
    
}