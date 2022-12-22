using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basyc.Asp;

public interface IStartupClass
{
	void ConfigureServices(IServiceCollection services);
	void Configure(IApplicationBuilder app, IWebHostEnvironment env);
}