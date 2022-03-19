using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiProject.Domain.Repositories;
using WebApiProject.Web.Models.Responses;

namespace WebApiProject.Web.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<IProductsService> _logger;

        public ProductsService(IProductsRepository productsRepository, IMapper mapper,
            ILogger<IProductsService> logger)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ProductResponseModel>> GetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving product Id {Id} from database.", id);

                var product = await _productsRepository.GetByIdAsync(id);
                var responseModel = _mapper.Map<ProductResponseModel>(product);
                var response = new Response<ProductResponseModel>
                {
                    Data = responseModel,
                    Status = ResponseStatus.Ok
                };

                if (response.Data is null)
                {
                    response.Status = ResponseStatus.NotFound;
                    response.ErrorMessage = nameof(ResponseStatus.NotFound);

                    _logger.LogInformation("Product Id:{Id} not found.", id);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while retrieving product Id:{Id} from database.", id);

                return new Response<ProductResponseModel>
                {
                    Status = ResponseStatus.Error,
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace
                };
            }
        }


        public async Task<Response<ProductsListResponseModel>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all products from database.");

                var products = await _productsRepository.GetAllAsync();
                var responseModel = _mapper.Map<ProductsListResponseModel>(products);
                var response = new Response<ProductsListResponseModel>
                {
                    Data = responseModel,
                    Status = ResponseStatus.Ok
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while retrieving all products from database.");

                return new Response<ProductsListResponseModel>
                {
                    Status = ResponseStatus.Error,
                    ErrorMessage = "Error while retrieving all products from database.",
                    StackTrace = ex.StackTrace
                };
            }
        }
    }
}
