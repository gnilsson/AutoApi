using AutoApi.Utility;

namespace AutoApi.EntityFramework.Repository;

public class RepositoryShell
{
    public ExpressionUtility.ConstructorDelegate Constructor { get; set; }

    public object QueryConfiguration { get; set; }

    public RepositoryShell(
        ExpressionUtility.ConstructorDelegate repositoryConstructor,
        object queryConfiguration)
    {
        Constructor = repositoryConstructor;
        QueryConfiguration = queryConfiguration;
    }
}
