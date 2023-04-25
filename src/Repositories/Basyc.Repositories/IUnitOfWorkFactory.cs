namespace Basyc.Repositories;

public interface IUnitOfWorkFactory<T> where T : class, IUnitOfWork
{
    T CreateUnitOfWork();
}
