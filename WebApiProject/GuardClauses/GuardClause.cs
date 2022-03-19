namespace WebApiProject.Web.GuardClauses
{
    public interface IGuardClause
    { }


    public class Guard : IGuardClause
    {
        public static IGuardClause Against = new Guard();
    }
}