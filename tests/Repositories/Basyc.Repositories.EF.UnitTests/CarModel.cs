using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Repositories.EF.Tests;

public class CarModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime Age { get; set; }
	public int CustomerId { get; set; }
}
