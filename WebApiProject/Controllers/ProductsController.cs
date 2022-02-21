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
            var subject = _productRepository.GetAll();

            return Ok(subject);
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var subject = _productRepository.GetById(id);
            if (subject == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(subject);
        }
    }
}