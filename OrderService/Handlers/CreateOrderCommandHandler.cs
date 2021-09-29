using System.Threading.Tasks;

namespace OrderService.Handlers
{
    //public class CreateOrderCommandHandler : 
    //IConsumer<CreateOrderCommand>,
    //IConsumer<PaymentCreated>
    //{
    //    private readonly IBus bus;

    //    public CreateOrderCommandHandler(
    //         IBus bus
    //    )
    //    {
    //        this.bus = bus;
    //    }

    //    public async Task Consume(ConsumeContext<CreateOrderCommand> context)
    //    {
    //        var orderId = context.Message.OrderId;

    //        //create order
    //        // bus.Publish(new CreatePaymentCommand { OrderId = orderId });
    //        await context.RespondAsync<OrderCreated>(new OrderCreated
    //        {
    //            IsCreated=true
    //        });
    //    }

    //    public Task Consume(ConsumeContext<PaymentCreated> context)
    //    {
    //        if(context.Message.Created==false) 
    //        {
    //            //TODO: RollBack Order
    //            throw new System.Exception("payment exception");
    //        }

    //        //TODO: 
    //        return Task.CompletedTask;
    //    }
    //}
}