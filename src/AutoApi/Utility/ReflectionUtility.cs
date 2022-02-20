using System.Reflection;

namespace AutoApi.Toolkit;

public static class ReflectionUtility
{
    public static T Invoke<T>(this Action<T> action) where T : new()
    {
        var obj = new T();
        action.Invoke(obj);
        return obj;
    }

    public static bool ParentHasInterface(
        this Type type,
        Type interfaceType,
        out Type? parentInterface)
    {
        parentInterface = type
            .GetInterfaces()?
            .FirstOrDefault(x => x.GetTypeInfo().ImplementedInterfaces.Contains(interfaceType));

        return parentInterface is not null;
    }


    public static Type? GetFirstInterfaceChild(
        this IEnumerable<Type> types,
        Type baseType,
        string name)
    {
        return types.FirstOrDefault(x => x.GetInterface(baseType.Name) != null && x.Name.Contains(name));
    }

    public static async Task<object?> InvokeAsync(
        this MethodInfo method,
        object obj,
        params object[] parameters)
    {
        var task = await Task.FromResult(method.Invoke(obj, parameters));
        var resultProperty = task!.GetType().GetProperty("Result");
        return resultProperty!.GetValue(task);
    }

    public static bool TryGetAttribute<T>(this PropertyInfo property, out T? attribute) where T : Attribute
    {
        attribute = property.GetCustomAttribute<T>();
        return attribute is not null;
    }
}
