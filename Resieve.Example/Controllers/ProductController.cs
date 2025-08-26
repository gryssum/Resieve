using Microsoft.AspNetCore.Mvc;
using Resieve.Example.Entities;
using Resieve.Example.Repository;

namespace Resieve.Example.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get([FromQuery] ResieveModel model)
    {
        var products = repository.GetFilteredProducts(model);
        return Ok(products);
    }
}

