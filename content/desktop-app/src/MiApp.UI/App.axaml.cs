using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MiApp.UI.ViewModels;
using MiApp.UI.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MiApp.UI;

public partial class App : Avalonia.Application
{
    private readonly IServiceProvider? _services;

    /// <summary>Constructor para el diseñador de XAML.</summary>
    public App() { }

    public App(IServiceProvider services) => _services = services;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && _services is not null)
        {
            var viewModel = _services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = viewModel };
            viewModel.Start();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
