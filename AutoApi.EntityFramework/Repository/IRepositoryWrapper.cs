using AutoApi.Domain;

namespace AutoApi.EntityFramework.Repository;

public interface IRepositoryWrapper
{
    IRepository<TEntity, TResponse> Get<TEntity, TResponse>() where TEntity : IEntity;
    IGeneralRepository General { get; }
    Task SaveAsync();
}
