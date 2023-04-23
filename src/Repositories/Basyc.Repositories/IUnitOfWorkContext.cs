using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories;

/// <summary>
/// Is responsible for saving repository changes.
/// <br/> Indicates unique storage source that can be shared between multiple repositories (e.g. DbContext)
/// <br/> Implements mechanism that read changes in supported repositories
/// </summary>
public interface IUnitOfWorkContext
{
    /// <summary>
    /// Differentiate between multiple unit of work repositories sources
    /// </summary>
    string Id { get; }

    Task Save();
}
