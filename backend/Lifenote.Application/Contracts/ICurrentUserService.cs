namespace Lifenote.Application.Contracts;

public interface ICurrentUserService
{
    Task<Guid> GetCurrentUserIdAsync(CancellationToken cancellationToken = default);
}
