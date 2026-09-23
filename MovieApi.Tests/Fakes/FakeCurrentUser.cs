using MovieApi.Core.Security;

namespace MovieApi.Tests.Fakes
{
    public sealed class FakeCurrentUser : ICurrentUser
    {
        public Guid? UserId { get; } = Guid.NewGuid();
    }
}
