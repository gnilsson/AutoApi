using System.Collections.ObjectModel;
using AutoApi.Rest.Configuration.Definitions;
using AutoApi.Rest.Shared.Responses;
using AutoApi.Utility;

namespace AutoApi.Rest.Configuration;

public enum ActionImplementationCategory
{
    Default = 1,
    Custom
}

public class SemanticsDefiner
{
    private IReadOnlyCollection<object> _actions = default!;
    private IReadOnlyCollection<object> _queries = default!;

    public void Configure(List<ConfigurationDefinition.Action> actionDescriptions, IDictionary<Type, IEnumerable<ConfigurationDefinition.Field>> expandeableMembers)
    {
        var actions = BuildActions(actionDescriptions);

        _actions = new ReadOnlyCollection<object>(actions.ToArray());

        var queries = BuildQueries(expandeableMembers);

        _queries = new ReadOnlyCollection<object>(queries.ToArray());
    }

    private static IEnumerable<object> BuildActions(ICollection<ConfigurationDefinition.Action> actionDescriptions)
    {
        var actionNames = actionDescriptions
            .Select(x => (x.ActionName, Key: (x.Controller, x.ActionMethodName, x.CustomActionImplementationOrder)))
            .ToDictionary(x => $"{x.Key.Controller}.{x.Key.ActionMethodName}.{x.Key.CustomActionImplementationOrder}", x => x.ActionName);

        foreach (var entityActionGroup in actionDescriptions.GroupBy(x => x.ResponseType))
        {
            var actionKeys = entityActionGroup.AsEnumerable().Select(x => $"{x.Controller}.{x.ActionMethodName}.{x.CustomActionImplementationOrder}");
            var newActions = new Dictionary<string, string>();

            foreach (var actionKey in actionKeys)
            {
                if (actionNames.TryGetValue(actionKey, out var newAction))
                {
                    var split = actionKey.Split('.');
                    //var newKey = string.Join("", split[1], split[2]);
                    var newKey = split[1];
                    newActions.Add(newKey, newAction);
                }
            }

            yield return ExpressionUtility.CreateConstructor(typeof(EndpointAction<>).MakeGenericType(entityActionGroup.Key), typeof(IDictionary<string, string>))(newActions);
        }
    }

    private static IEnumerable<object> BuildQueries(IDictionary<Type, IEnumerable<ConfigurationDefinition.Field>> fieldDescriptions)
    {
        return fieldDescriptions.Select(x => ExpressionUtility.CreateConstructor(
                typeof(Query<>).MakeGenericType(typeof(PagedResponse<>).MakeGenericType(x.Key)),
                typeof(IEnumerable<ConfigurationDefinition.Field>))(x.Value));
    }

    public EndpointAction<T> GetAction<T>() => (_actions.Single(x => x.GetType().GenericTypeArguments[0] == typeof(T)) as EndpointAction<T>)!;

    public Query<T> GetQuery<T>() => (_queries.Single(x => x.GetType().GenericTypeArguments[0] == typeof(T)) as Query<T>)!;

    public class EndpointAction<T>
    {
        public EndpointAction(IDictionary<string, string> actionNames) => Names = new ReadOnlyDictionary<string, string>(actionNames);

        public static IReadOnlyDictionary<string, string> Names { get; private set; } = default!;
    }

    public class Query<T>
    {
        public Query(IEnumerable<ConfigurationDefinition.Field> members)
        {
            DefaultFields = new ReadOnlyCollection<string>(SelectNamesByCategory(members, ConfigurationDefinition.FieldCategory.Default));
            RelationalFields = new ReadOnlyCollection<string>(SelectNamesByCategory(members, ConfigurationDefinition.FieldCategory.Relational));
        }

        private static List<string> SelectNamesByCategory(IEnumerable<ConfigurationDefinition.Field> members, ConfigurationDefinition.FieldCategory fieldCategory)
        {
            return members
                .Where(x => x.Category == fieldCategory)
                .Select(x => x.Name)
                .ToList();
        }

        public IReadOnlyCollection<string> DefaultFields { get; }
        public IReadOnlyCollection<string> RelationalFields { get; }
    }
}
