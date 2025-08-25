using Microsoft.AspNetCore.Mvc;
using ReSieve.Example.Entities;
using ReSieve.Example.Repository;

namespace ReSieve.Example.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Product>> Get([FromQuery] ReSieveModel model)
    {
        var products = repository.GetFilteredProducts(model);
        return Ok(products);
    }
}

