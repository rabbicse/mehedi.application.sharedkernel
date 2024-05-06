using System.Text.Json.Serialization;

namespace Mehedi.Application.SharedKernel.Responses;

/// <summary>
/// PagedResult will be useful on http response generation
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResult<T>(PagedInfo pagedInfo, T value) : Result<T>(value)
{
    [JsonInclude]
    public PagedInfo PagedInfo { get; init; } = pagedInfo;
}
