using Mehedi.Core.SharedKernel;

namespace Mehedi.Application.SharedKernel.Persistence;

/// <summary>
/// IReadDbContext will be used to access read database
/// Specially it'll be useful for NoSQL databases
/// </summary>
public interface IReadDbContext
{
    /// <summary>
    /// Gets the collection for the specified query model with page.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <param name="start"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<IEnumerable<TQueryModel>> GetCollectionAsync<TQueryModel>(int start = 0, int pageSize = 100) where TQueryModel : IQueryModel;
    /// <summary>
    /// Gets the Query Model by id.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TQueryModel?> GetByIdAsync<TQueryModel, TKey>(TKey id) where TQueryModel : IQueryModel;

    /// <summary>
    /// Get read database context
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    TContext GetDatabaseContext<TContext>();
}
