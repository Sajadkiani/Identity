using AutoMapper;
using OrderService.Entities;
using OrderService.Models;
using OrderService.ViewModels;

namespace OrderService.MapperPofiles
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<GetOrderVm,Order>();
            CreateMap<Order,GetOrderModel>();
        }
    }
}