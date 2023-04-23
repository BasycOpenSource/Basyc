using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Basyc.Asp;

public class AspDefaultStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => (app) =>
                                                                                           {
                                                                                               var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();

                                                                                               if (env.IsDevelopment())
                                                                                               {
                                                                                                   app.UseDeveloperExceptionPage();
                                                                                                   app.UseSwagger();
                                                                                                   app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", env.ApplicationName + " v1"));
                                                                                               }

                                                                                               app.UseHttpsRedirection();
                                                                                               app.UseRouting();
                                                                                               app.UseAuthorization();

                                                                                               app.UseCors(x =>
                                                                                               {
                                                                                                   x.AllowAnyMethod()
                                                                                                   .AllowAnyHeader()
                                                                                                   .SetIsOriginAllowed(o => true)
                                                                                                   .AllowCredentials();
                                                                                               });

                                                                                               next(app);
                                                                                           };
}
