using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories;

public interface IPageRepository<TModel>
{
	Task<PageResult<TModel>> GetPageAsync(int page, int itemsPerPage);
}
