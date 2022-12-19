using Basyc.Localizator.Abstraction.Initialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Infrastructure.EF.Initialization
{
    public static class EfLocalizatorBuilderExtensions
    {
        public static LocalizatorBuilder AddEfStorage(this LocalizatorBuilder builder)
        {
            builder.AddStorage<EFLocalizatorStorage>();
            return builder;
        }
    }
}
