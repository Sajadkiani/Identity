using System;
using System.Threading.Tasks;
using MassTransit;
using OrderService.Models.Payments;

namespace OrderService.Consumers.Orders
{
    public class OrderConsumer : IConsumer<PaymentCreated>
    {
        public Task Consume(ConsumeContext<PaymentCreated> context)
        {
            Console.WriteLine("orderservice --> get PaymentCreated");

            return Task.CompletedTask;
        }
    }
}