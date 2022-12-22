using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Localizator.Abstraction;

public record GetLocalizatorResult(bool localizatorFound, ILocalizator localizator);