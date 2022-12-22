using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories;

public class PageResult<TModel>
{
	public PageResult(IEnumerable<TModel> records, int itemsPerPage, int totalCount, int currentPage, int totalPages)
	{
		Records = records;
		ItemsPerPage = itemsPerPage;
		TotalCount = totalCount;
		CurrentPage = currentPage;
		TotalPages = totalPages;
	}

	public IEnumerable<TModel> Records { get; }
	public int ItemsPerPage { get; }
	public int TotalCount { get; }
	public int CurrentPage { get; }
	public int TotalPages { get; }
}