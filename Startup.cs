using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RiskGame.API.Models;
using RiskGame.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using RiskGame.API.Models.PlayerFolder;
using RiskGame.API.Models.AssetFolder;
using RiskGame.API.Persistence;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using RiskGame.API.Logic;
using RiskGame.API.Persistence.Repositories;

namespace RiskGame.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DatabaseSettings>(
                Configuration?.GetSection("RiskGameDatabaseSettings"));

            services.AddSingleton<IDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            // SINGLETON SERVICES
            services.AddSingleton<IPlayerService,PlayerService>();
            services.AddSingleton<IAssetService,AssetService>();
            services.AddSingleton<IShareService,ShareService>();
            services.AddSingleton<IMarketService, MarketService>();
            services.AddSingleton<IEconService, EconService>();
            services.AddSingleton<ITransactionService, TransactionService>();

            // SERVICES
            services.Add(new ServiceDescriptor(typeof(TransactionContext), new TransactionContext(Configuration?.GetSection("RiskGameDatabaseSettings").GetChildren().Where(v => v.Key == "MySqlConnectionString").Select(v => v.Value).FirstOrDefault().ToString())));

            // LOGIC
            services.AddTransient<IPlayerLogic, PlayerLogic>();
            services.AddTransient<IAssetLogic, AssetLogic>();
            services.AddTransient<ITransactionLogic,TransactionLogic>();
            services.AddTransient<IEconLogic, EconLogic>();

            // REPOSITORIES
            services.AddSingleton<IAssetRepo, AssetRepo>();
            services.AddSingleton<IPlayerRepo, PlayerRepo>();
            services.AddSingleton<IShareRepo, ShareRepo>();
            services.AddSingleton<IMarketRepo, MarketRepo>();
            services.AddSingleton<IEconRepo, EconRepo>();

            services.AddControllersWithViews();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
