namespace Meshmakers.Common.Shared;

/// <summary>
/// Defines extension methods for the <see cref="Type"/> class
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets the most inner base type of the given type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetMostInnerBaseType(this Type type)
    {
        while (type.BaseType != null && !type.BaseType.IsInterface && type.BaseType != typeof(object)) type = type.BaseType;
        return type;
    }
}
