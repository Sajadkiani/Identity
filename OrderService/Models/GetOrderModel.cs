using System;

namespace OrderService.Models
{
    public class GetOrderModel
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime CreateOn { get; set; }
    }
}