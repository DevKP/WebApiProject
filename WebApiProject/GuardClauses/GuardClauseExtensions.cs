using System;

namespace WebApiProject.Web.GuardClauses
{
    public static class GuardClauseExtensions
    {
        public static void Null<T>(this IGuardClause guard, T parameter, string parameterName)
        {
            if (parameter is null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }
    }
}