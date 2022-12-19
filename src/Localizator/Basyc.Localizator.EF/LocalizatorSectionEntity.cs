using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Infrastructure.EF
{
    public class LocalizatorSectionEntity
    {
        public LocalizatorSectionEntity()
        {

        }

        [Key]
        public string UniqueSectionName { get; set; }
        public List<LocalizatorEntity> Localizators { get; set; }

    }
}
