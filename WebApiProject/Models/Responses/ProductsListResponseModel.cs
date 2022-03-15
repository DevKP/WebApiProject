using System.Collections.Generic;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Web.Models.Responses
{
    public class ProductsListResponseModel
    {
        public IEnumerable<ProductResponseModel> Products { get; set; }
    }
}
