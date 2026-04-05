namespace Lifenote.Application.Contracts;

public interface ICurrentUserService
{
    Task<int> GetCurrentUserIdAsync(CancellationToken cancellationToken = default);
}
