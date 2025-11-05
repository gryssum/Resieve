using Microsoft.AspNetCore.Mvc;
using Resieve.Example.Entities;
using Resieve.Example.Repository;

namespace Resieve.Example.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository repository, ProductAdvancedRepository advancedRepository) : ControllerBase
{
    [HttpGet("simple")]
    public ActionResult<IEnumerable<Product>> Get([FromQuery] ResieveModel model)
    {
        var products = repository.GetFilteredProductsAsync(model);
        return Ok(products);
    }
    
    [HttpGet("advanced")]
    public ActionResult<PaginatedResponse<Product>> GetFromAdvanced([FromQuery] ResieveModel model)
    {
        var products = advancedRepository.GetFilteredProductsAsync(model);
        return Ok(products);
    }
}

