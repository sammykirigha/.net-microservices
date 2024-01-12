
using System;
using CommandsService.Contracts;
using CommandsService.Data;
using CommandsService.Repository;
using Microsoft.EntityFrameworkCore;

namespace CommandsService;

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
                services.AddDbContext<AppDbContext>(opt =>
                    opt.UseSqlServer(_configuration.GetConnectionString("CommandsConnection")));
        }else
        {
             Console.WriteLine("--> Using InMem Db");
              services.AddDbContext<AppDbContext>(opt =>
                     opt.UseInMemoryDatabase("InMem"));
        }

        services.AddScoped<ICommandRepo, CommandRepo>();
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
    }
}