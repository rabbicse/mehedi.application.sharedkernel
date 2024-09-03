using Mehedi.Core.SharedKernel;
using System.Linq.Expressions;

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
    Task<IEnumerable<TQueryModel>> GetAllCollectionAsync<TQueryModel>() 
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Gets the collection for the specified query model with page.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <param name="start"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<IEnumerable<TQueryModel>> GetCollectionAsync<TQueryModel>(int start = 0, int pageSize = 100) 
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Gets the Query Model by id.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<TQueryModel?> GetByIdAsync<TQueryModel, TKey>(TKey id) where TQueryModel : IQueryModel;

    /// <summary>
    /// Upserts a query model into the database.
    /// </summary>
    /// <typeparam name="TQueryModel">The type of the query model.</typeparam>
    /// <param name="queryModel">The query model to upsert.</param>
    /// <param name="upsertFilter">The filter expression to determine the upsert condition.</param>
    /// <returns>A task representing the asynchronous upsert operation.</returns>
    Task UpsertAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Upserts query models into the database.
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <param name="queryModels"></param>
    /// <returns></returns>
    Task UpsertAsync<TQueryModel>(IEnumerable<TQueryModel> queryModels)
    where TQueryModel : IQueryModel;

    /// <summary>
    /// Deletes query models from the database that match the specified filter.
    /// </summary>
    /// <typeparam name="TQueryModel">The type of the query model.</typeparam>
    /// <param name="deleteFilter">The filter expression to determine which query models to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
        where TQueryModel : IQueryModel;
    /// <summary>
    /// Search document by multiple fields, it's wildcard search
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <param name="queries"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<(int, IEnumerable<TQueryModel>)> WildSearchAsync<TQueryModel>(Dictionary<string, string> queries, 
                                                            int pageNumber = 0, 
                                                            int pageSize = 100)
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Search document by multiple fields, it's fuzzy search
    /// </summary>
    /// <typeparam name="TQueryModel"></typeparam>
    /// <param name="queries"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<(int, IEnumerable<TQueryModel>)> FuzzySearchAsync<TQueryModel>(Dictionary<string, string> queries,
                                                        int pageNumber = 0,
                                                        int pageSize = 100)
        where TQueryModel : IQueryModel;

    /// <summary>
    /// Get the generic database context for NoSQL
    /// </summary>
    /// <returns></returns>
    object GetDatabaseContext();
}
