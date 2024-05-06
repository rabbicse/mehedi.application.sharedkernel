using Mehedi.Core.SharedKernel;

namespace Mehedi.Application.SharedKernel.Persistence;

/// <summary>
/// IReadDbContext will be used to access read database
/// Specially it'll be useful for NoSQL databases
/// </summary>
public interface IReadDbContext
{
    /// <summary>
    /// Gets the collection for the specified query model.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <returns></returns>
    Task<IEnumerable<TQueryModel>> GetCollectionAsync<TQueryModel>() where TQueryModel : IQueryModel;

    /// <summary>
    /// Creates collections in the database for all query models.
    /// </summary>
    /// <returns>A task representing the asynchronous creation of collections.</returns>
    Task CreateCollectionsAsync();
}
