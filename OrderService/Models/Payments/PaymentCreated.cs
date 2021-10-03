namespace OrderService.Models.Payments
{
    public class PaymentCreated
    {
        public bool Created { get; set; }
        public int OrderId { get; set; }
    }
}