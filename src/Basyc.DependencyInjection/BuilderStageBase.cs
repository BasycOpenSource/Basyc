using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.DependencyInjection
{
    public class BuilderStageBase
    {
        public readonly IServiceCollection services;

        public BuilderStageBase(IServiceCollection services)
        {
            this.services = services;
        }
    }
}