using System.Reflection;
using System.Reflection.Emit;
using AutoApi.Domain;
using AutoApi.EntityFramework.Repository;
using AutoApi.Mediator;
using AutoApi.Rest.Configuration.Definitions;
using AutoApi.Rest.Pipeline.Controller;
using Microsoft.AspNetCore.Mvc;

namespace AutoApi.Rest.Configuration.Builders;

public class ReflectionContainer
{
    public Type? CreatedType { get; init; }
    public MethodInfo? MethodInfo { get; init; }
}

public class SomeRequest
{
    public string? Value { get; set; }
}

public class NestedGetQuery<UResponse> : IRequest<UResponse>
{
    public Guid Id { get; init; }
}

public static class DynamicAssemblyBuilder
{
    static DynamicAssemblyBuilder()
    {
        _dynamicModuleBuilder = CreateModuleBuilder();
    }

    private static readonly ModuleBuilder _dynamicModuleBuilder;

    public static Dictionary<string, ReflectionContainer> ReflectionContainers { get; } = new();

    public static object? Controller { get; private set; }

    private static ModuleBuilder CreateModuleBuilder()
    {
        var assemblySimpleName = "MyGreatPlugin";
        var assemblyFileName = string.Concat(assemblySimpleName, ".dll");
        var assemblyName = new AssemblyName(assemblyFileName);
        var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = dynamicAssembly.DefineDynamicModule("MyPluginModule");
        return moduleBuilder;
    }

    public static Type CreateDynamicControllerType(ConfigurationDefinition.Entity entityDefinition, ConfigurationDefinition.Entity[] foreignEntityDefinitions)
    {
        var controllerType = typeof(AutoApiRestController<,,>).MakeGenericType(entityDefinition.ResponseType, entityDefinition.QueryType, entityDefinition.CommandType);

        var constructor = controllerType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new[] { typeof(IMediator), typeof(IRepositoryWrapper) })!;

        var extendedType = _dynamicModuleBuilder.DefineType(
            $"PluginExtendedType{entityDefinition.EntityType.Name}",
            TypeAttributes.Class | TypeAttributes.Public,
            controllerType);

        //var apiControllerAttributeConstructor = typeof(ApiControllerAttribute).GetConstructor(Type.EmptyTypes);
        //var apiControllerAttributeBuilder = new CustomAttributeBuilder(apiControllerAttributeConstructor, Array.Empty<object>());
        //extendedType.SetCustomAttribute(apiControllerAttributeBuilder);

        //var genericParameter = extendedType.DefineGenericParameters(new[] { "TEntity " })[0];
        //genericParameter.SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint);
        //genericParameter.SetInterfaceConstraints(new[] { typeof(IEntity) });

        var constructorBuilder = extendedType.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            new Type[] { typeof(IMediator), typeof(IRepositoryWrapper) });

        var field1 = extendedType.DefineField("_repositoryWrapper", typeof(IRepositoryWrapper), FieldAttributes.Private | FieldAttributes.InitOnly);
        var field2 = extendedType.DefineField("_mediator", typeof(IMediator), FieldAttributes.Private | FieldAttributes.InitOnly);

        var msilGenerator = constructorBuilder.GetILGenerator();

        msilGenerator.Emit(OpCodes.Ldarg_0);
        msilGenerator.Emit(OpCodes.Ldarg_1);
        msilGenerator.Emit(OpCodes.Ldarg_2);
        msilGenerator.Emit(OpCodes.Call, constructor);
        msilGenerator.Emit(OpCodes.Nop);
        msilGenerator.Emit(OpCodes.Nop);
        msilGenerator.Emit(OpCodes.Ldarg_0);
        msilGenerator.Emit(OpCodes.Ldarg_1);
        msilGenerator.Emit(OpCodes.Stfld, field1);
        msilGenerator.Emit(OpCodes.Ldarg_0);
        msilGenerator.Emit(OpCodes.Ldarg_2);
        msilGenerator.Emit(OpCodes.Stfld, field2);
        msilGenerator.Emit(OpCodes.Ret);

        foreach (var foreignEntityDefinition in foreignEntityDefinitions)
        {
            DefineGetNestedMethod(entityDefinition, controllerType, extendedType, foreignEntityDefinition);
        }

        var type = extendedType.CreateType()!;

        var reflectionContainer = new ReflectionContainer
        {
            CreatedType = type
        };

        ReflectionContainers.Add(entityDefinition.EntityType.Name, reflectionContainer);

        return type;
    }

    private static void DefineGetNestedMethod(ConfigurationDefinition.Entity entityDefinition, Type controllerType, TypeBuilder extendedType, ConfigurationDefinition.Entity foreignEntityDefinition)
    {
        var method = extendedType.DefineMethod(
            $"Get{entityDefinition.EntityType.Name}Nested{foreignEntityDefinition.EntityType.Name}s",
            MethodAttributes.Public,
            typeof(Task<IActionResult>),
            new[] { typeof(Guid), foreignEntityDefinition.QueryType });

        var httpGetAttributeConstructor = typeof(HttpGetAttribute).GetConstructor(Type.EmptyTypes)!;
        var httpGetAttributeBuilder = new CustomAttributeBuilder(httpGetAttributeConstructor, Array.Empty<object>());
        method.SetCustomAttribute(httpGetAttributeBuilder);

        var idParam = method.DefineParameter(1, ParameterAttributes.None, "id");

        var requestParam = method.DefineParameter(2, ParameterAttributes.None, "request");
        var fromQueryAttributeConstructor = typeof(FromQueryAttribute).GetConstructor(Type.EmptyTypes)!;
        var fromQueryAttributeBuilder = new CustomAttributeBuilder(fromQueryAttributeConstructor, Array.Empty<object>());
        requestParam.SetCustomAttribute(fromQueryAttributeBuilder);

        var getNestedMethod = controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.IsGenericMethodDefinition && x.Name == "GetNested")!
            .MakeGenericMethod(foreignEntityDefinition.ResponseType, foreignEntityDefinition.QueryType, entityDefinition.EntityType, foreignEntityDefinition.EntityType);

        var ilgen = method.GetILGenerator();

        ilgen.Emit(OpCodes.Nop);
        ilgen.Emit(OpCodes.Ldarg_0);
        ilgen.Emit(OpCodes.Ldarg_1);
        ilgen.Emit(OpCodes.Ldarg_2);
        ilgen.Emit(OpCodes.Call, getNestedMethod);
        ilgen.Emit(OpCodes.Ret);
    }
}
