using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RocketLeagueModManager.App.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RocketLeagueModManager.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.File(Path.Combine("Logs", "RocketLeagueModManager.log"))
                .CreateLogger();

            host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                })
                .ConfigureLogging(logging =>
                {
                    logging.AddSerilog();
                })
                .Build();

            ServiceProvider = host.Services;
        }

        private void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            var appSettingsJson = File.ReadAllText("appsettings.json");
            var appSettings = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(appSettingsJson);
            
            services.AddSingleton(appSettings);
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<SettingsViewModel>();

            services.AddTransient<MainWindow>();
            services.AddTransient<SettingsEditor>();
        }

        protected async override void OnStartup(StartupEventArgs e)
        {
            await host.StartAsync();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            using (host)
            {
                await host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }
    }
}
