namespace Mehedi.Application.SharedKernel.Persistence;

/// <summary>
/// IWriteDbContext will be used for write database
/// Specially for RDBMS databases
/// </summary>
public interface IWriteDbContext : IDisposable
{
    /// <summary>
    /// Save data to databases
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
