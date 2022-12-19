using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceColectionSerialiazionExtensions
    {
        public static SelectSerializationStage AddBasycSerialization(this IServiceCollection service)
        {
            return new SelectSerializationStage(service);
        }
    }
}