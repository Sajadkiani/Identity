using System;
using System.Threading.Tasks;
using MassTransit;
using Models;

namespace OrderService.Consumers.Orders
{
    public class OrderConsumer : IConsumer<PaymentCreatedModel>
    {
        public Task Consume(ConsumeContext<PaymentCreatedModel> context)
        {
            Console.WriteLine("orderservice --> get PaymentCreated");
            return Task.CompletedTask;
        }
    }
}   