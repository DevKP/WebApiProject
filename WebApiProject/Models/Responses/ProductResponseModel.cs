namespace WebApiProject.Web.Models.Responses
{
    public class ProductResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
    }
}
