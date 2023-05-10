using Common.Services.Brokers;
using Microsoft.AspNetCore.Mvc;
using ProductService.Models.Products;

namespace ProductService.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IBroker _broker;

    public ProductController(
        IBroker broker
        )
    {
        _broker = broker;
    }
    
    [HttpPost]
    public async Task AddProductAsync(ProductModel.AddProductInput input)
    {
        //TODO: add product 
        await Task.FromResult(0);
    }
    
    [HttpGet("all")]
    public async Task<List<object>> GetProductsAsync()
    {
        //TODO: getall
        await Task.FromResult(0);

        return new List<object>();
    }
}