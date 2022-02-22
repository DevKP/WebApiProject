using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;

namespace WebApiProject.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository _productRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            _productRepository = productsRepository;
        }

        [HttpGet()]
        public IActionResult GetAll()
        {
            var products = _productRepository.GetAll();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int? id)
        {
            var products = _productRepository.GetById(id);
            if (products == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(products);
        }
    }
}