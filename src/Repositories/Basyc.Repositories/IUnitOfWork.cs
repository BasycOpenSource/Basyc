namespace Basyc.Repositories;

/// <summary>
/// All repositories must inherit <see cref="IUnitOfWorkRepository{TModel, TKey}"/>
/// <br/> This class should containg all repositories as readonly properties.
/// </summary>
public interface IUnitOfWork : IBulkRepository, IDisposable
{
}
