using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.Extensions.Logging;
using WebApiProject.Domain.Repositories;
using WebApiProject.Web.Data;
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
            Guard.Against.Null(productsRepository, nameof(productsRepository));
            Guard.Against.Null(mapper, nameof(mapper));
            Guard.Against.Null(logger, nameof(logger));

            _productsRepository = productsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Response<ProductResponseModel>> GetAsync(int id)
        {
            var response = new Response<ProductResponseModel>();

            try
            {
                _logger.LogInformation("Retrieving product Id {Id} from database.", id);

                var product = await _productsRepository.GetByIdAsync(id);
                if (product is not null)
                {
                    response.Data = _mapper.Map<ProductResponseModel>(product);
                    response.Status = ResponseStatus.Ok;
                }
                else
                {
                    response.Status = ResponseStatus.NotFound;
                    response.ErrorMessage = ErrorMessages.NotFoundInDatabase;

                    _logger.LogInformation("Product Id:{Id} not found.", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving product Id:{Id} from database.", id);

                response.Status = ResponseStatus.Error;
                response.ErrorMessage = ErrorMessages.ErrorWhileRetrievingEntity;
            }

            return response;
        }


        public async Task<Response<ProductsListResponseModel>> GetAllAsync()
        {
            var response = new Response<ProductsListResponseModel>();

            try
            {
                _logger.LogInformation("Retrieving all products from database.");

                var products = await _productsRepository.GetAllAsync();
                response.Data = _mapper.Map<ProductsListResponseModel>(products);
                response.Status = ResponseStatus.Ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while retrieving all products from database.");

                response.Status = ResponseStatus.Error;
                response.ErrorMessage = ErrorMessages.ErrorWhileRetrievingEntity;
            }

            return response;
        }
    }
}
