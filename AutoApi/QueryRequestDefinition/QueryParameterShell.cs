using AutoApi.Utility;

namespace AutoApi.QueryRequestDefinition;

public record QueryParameterShell(ExpressionUtility.ConstructorDelegate Constructor, string[] NavigationArgs);
