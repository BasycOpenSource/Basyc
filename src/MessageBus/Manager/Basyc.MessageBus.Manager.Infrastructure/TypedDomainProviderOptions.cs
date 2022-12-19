using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.MessageBus.Manager.Infrastructure
{
    public class TypedDomainProviderOptions
    {
        public List<TypedDomainSettings> TypedDomainOptions { get; set; } = new List<TypedDomainSettings>();
    }
}