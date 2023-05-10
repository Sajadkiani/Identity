using System.Collections.Generic;

namespace OrderService.ViewModels;

public class ProductViewModel
{
    public class AddProductInput
    {
        public List<int> ProductIds { get; set; }
    }
}