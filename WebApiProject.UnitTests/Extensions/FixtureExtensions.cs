using System.Linq;
using AutoFixture;

namespace WebApiProject.UnitTests.Extensions
{
    public static class FixtureExtensions
    {
        public static void SetFixtureRecursionDepth(this Fixture fixture, int depth)
        {
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(depth));
        }
    }
}