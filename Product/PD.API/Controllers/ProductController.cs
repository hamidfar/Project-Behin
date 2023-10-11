using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PD.API.Application.Commands;
using PD.API.Application.Queries;

namespace PD.API.Controllers
{
    [ApiController]
    [Route("api/product")]
    [Authorize(Roles = "ADMIN", Policy = "Service-PD")]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("product")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var product = await _mediator.Send(command);

            if (product == null)
            {
                return BadRequest("Failed to create the product.");
            }

            return Ok(product);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var query = new GetProductsQuery();
            var products = await _mediator.Send(query);
            return Ok(products);
        }
    }
}
