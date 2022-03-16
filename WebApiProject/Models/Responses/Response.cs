namespace WebApiProject.Web.Models.Responses
{
    public class Response
    {
        public ResponseStatus Status { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }

    public class Response<T> : Response
    {
        public T Data { get; set; }
    }

    public enum ResponseStatus
    {
        Error,
        Ok,
        NotFound
    }
}
