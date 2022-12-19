using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.DomainDrivenDesign.Domain
{
    public record CommandWithContextBase<TContext>(TContext Context) : ICommand;

    public record CommandWithContextBase<TContext, TResponse>(TContext Context) : ICommand<TResponse>
        where TResponse : class;
}