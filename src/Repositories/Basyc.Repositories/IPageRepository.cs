namespace Basyc.Repositories;

public interface IPageRepository<TModel>
{
    Task<PageResult<TModel>> GetPageAsync(int page, int itemsPerPage);
}
