using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MiApp.UI.ViewModels;

namespace MiApp.UI.Views;

/// <summary>
/// Resuelve la vista de cada ViewModel. Se usa pattern matching en lugar de reflexión:
/// es explícito, rápido y seguro frente al recorte (trimming) de código.
/// </summary>
public sealed class ViewLocator : IDataTemplate
{
    public Control? Build(object? param) => param switch
    {
        HomeViewModel => new HomeView(),
        TodosViewModel => new TodosView(),
        null => null,
        _ => new TextBlock { Text = $"No hay vista registrada para {param.GetType().Name}" },
    };

    public bool Match(object? data) => data is ViewModelBase;
}
