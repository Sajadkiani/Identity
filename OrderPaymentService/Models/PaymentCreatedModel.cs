namespace OrderPaymentService.Models
{
    public class PaymentCreatedModel
    {
        public bool Created { get; set; }
        public int OrderId { get; set; }
    }
}