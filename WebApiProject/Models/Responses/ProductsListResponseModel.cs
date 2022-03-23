using System.Collections.Generic;

namespace WebApiProject.Web.Models.Responses
{
    public class ProductsListResponseModel
    {
        public IEnumerable<ProductResponseModel> Products { get; set; }
    }
}
