using System;
using System.Collections.Generic;

namespace OrderService.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime CreateOn { get; set; }
        public int UserId { get; set; }
    }
}