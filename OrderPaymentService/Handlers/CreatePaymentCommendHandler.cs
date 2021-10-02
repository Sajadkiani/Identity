using System.Threading.Tasks;
using Common.Commands.Order.Payments;
using MassTransit;
using Microsoft.Extensions.Logging;
using Models;

namespace OrderPaymentService.Handlers
{
    public class CreatePaymentCommendHandler : IConsumer<CreateOrderPaymentModel>
    {
        private readonly ILogger<CreatePaymentCommendHandler> logger;
        private readonly IBus bus;

        public CreatePaymentCommendHandler(
            ILogger<CreatePaymentCommendHandler> logger,
            IBus bus
        )
        {
            this.logger = logger;
            this.bus = bus;
        }

        public Task Consume(ConsumeContext<CreateOrderPaymentModel> context)
        {
            //TODO
            logger.LogInformation("payment created!");

            bus.Send(new PaymentCreated { Created = true });
            return Task.CompletedTask;
        }
    }
}