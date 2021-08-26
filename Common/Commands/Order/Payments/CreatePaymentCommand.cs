using System;

namespace Common.Commands.Order.Payments
{
    public class CreatePaymentCommand
    {
        public Guid OrderId { get; set; }
    }
}