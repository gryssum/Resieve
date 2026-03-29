using Microsoft.AspNetCore.Mvc;
using Resieve.Example.Entities;
using Resieve.Example.Repository;

namespace Resieve.Example.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository repository, ProductAdvancedRepository advancedRepository) : ControllerBase
{
    [HttpGet("simple")]
    public async ValueTask<ActionResult<IEnumerable<Product>>> Get([FromQuery] ResieveModel model)
    {
        var products = await repository.GetFilteredProductsAsync(model);
        return Ok(products);
    }
    
    [HttpGet("advanced")]
    public async ValueTask<ActionResult<PaginatedResponse<Product>>> GetFromAdvanced([FromQuery] ResieveModel model)
    {
        var products = await advancedRepository.GetFilteredProductsAsync(model);
        return Ok(products);
    }
}

