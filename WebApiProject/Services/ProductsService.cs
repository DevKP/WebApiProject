using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Domain.Repositories;
using WebApiProject.Web.Models.Responses;

namespace WebApiProject.Web.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;

        public ProductsService(IProductsRepository productsRepository, IMapper mapper)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponseModel> GetAsync(int id)
        {
            var product = await _productsRepository.GetByIdAsync(id);
            var responseModel = _mapper.Map<ProductResponseModel>(product);
            return responseModel;
        }

        public async Task<ProductsListResponseModel> GetAllAsync()
        {
            var products = await _productsRepository.GetAllAsync();
            var responseModel = _mapper.Map<ProductsListResponseModel>(products);
            return responseModel;
        }
    }
}
