using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.Web.Extensions;
using WebApiProject.Web.Models.Responses;
using WebApiProject.Web.Services;

namespace WebApiProject.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productsService;

        public ProductsController(IProductsService productsService)
        {
            Guard.Against.Null(productsService, nameof(productsService));

            _productsService = productsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ProductsListResponseModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Response<ProductsListResponseModel>))]
        public async Task<IActionResult> GetAll()
        {
            var productsListResponse = await _productsService.GetAllAsync();

            return productsListResponse.AsActionResult();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response<ProductResponseModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response<ProductResponseModel>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Response<ProductResponseModel>))]
        public async Task<IActionResult> Get(int id)
        {
            var productResponse = await _productsService.GetAsync(id);

            return productResponse.AsActionResult();
        }


    }
}