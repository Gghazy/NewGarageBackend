namespace Garage.Application.Abstractions;

public interface IBranchAccessService
{
    Task<IReadOnlyList<Guid>?> GetAccessibleBranchIdsAsync(CancellationToken ct = default);
}
