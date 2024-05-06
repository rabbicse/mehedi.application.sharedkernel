using System.Reflection;

namespace Mehedi.Application.SharedKernel.Extensions;

/// <summary>
/// AssemblyExtensions will be used for generic reflection related work
/// </summary>
public static class AssemblyExtensions
{
    /// <summary>
    /// Retrieves all types in the specified assembly that implement or inherit from a given interface or base class.
    /// </summary>
    /// <typeparam name="TInterface">The interface or base class type.</typeparam>
    /// <param name="assembly">The assembly to search in.</param>
    /// <returns>An enumerable collection of types that implement or inherit from the specified interface or base class.</returns>
    public static IEnumerable<Type> GetAllTypesOf<TInterface>(this Assembly assembly)
    {
        var isAssignableToTInterface = typeof(TInterface).IsAssignableFrom;
        return assembly
            .GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && isAssignableToTInterface(type))
            .ToList();
    }
}
