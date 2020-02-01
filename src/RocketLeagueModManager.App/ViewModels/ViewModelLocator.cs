using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketLeagueModManager.App.ViewModels
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel
            => App.ServiceProvider.GetRequiredService<MainWindowViewModel>();

        public SettingsViewModel SettingsViewModel
            => App.ServiceProvider.GetRequiredService<SettingsViewModel>();
    }
}
