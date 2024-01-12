
using System;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataService;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;

namespace PlatformService;

public class Startup
{
    private IConfiguration _configuration;
    private IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        if(_env.IsProduction())
        {
             Console.WriteLine("--> Using SqlServer Db");
                services.AddDbContext<AppDBContext>(opt =>
                    opt.UseSqlServer(_configuration.GetConnectionString("PlatformsConnection")));
        }else
        {
             Console.WriteLine("--> Using InMem Db");
                services.AddDbContext<AppDBContext>(opt =>
                     opt.UseInMemoryDatabase("InMem"));
        }

        services.AddScoped<IPlatformRepo, PlatformRepo>();
        services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        services.AddSingleton<IMessageBusClient, MessageBusClient>();
        services.AddControllers();
        services.AddAutoMapper(typeof(Program).Assembly);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if(env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        //app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => 
        {
            endpoints.MapControllers();
        });

        PrepDb.PrepPopulation(app, _env.IsProduction());
    }
}