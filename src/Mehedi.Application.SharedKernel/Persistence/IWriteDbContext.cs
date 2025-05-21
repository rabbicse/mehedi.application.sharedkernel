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

    /// <summary>
    /// Set IQueryable to get persistence entities dynamically
    /// If application uses Entityframework then it'll useful
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    IQueryable<TEntity> Entities<TEntity>() where TEntity : class;

    /// <summary>
    /// Add method to perform raw query
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task<IEnumerable<TEntity>> FromSqlRawAsync<TEntity>(string sql, params object[] parameters) where TEntity : class;

    /// <summary>
    /// Add method to perform raw query with different result
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    Task<IEnumerable<TResponse>> FromSqlRawAsync<TEntity, TResponse>(
     string sql,
     params object[] parameters)
     where TEntity : class
     where TResponse : class;
}
