using Microsoft.Extensions.DependencyInjection;

namespace MiApp.UI.ViewModels;

public sealed record NavigationItem(string Title, string Icon, Type ViewModelType);

/// <summary>Crea los ViewModels de cada página.</summary>
public interface IPageFactory
{
    ViewModelBase Create(Type viewModelType);
}

/// <summary>
/// Cada página vive en su propio ámbito de DI: obtiene un DbContext nuevo al entrar
/// y se libera al salir (el patrón recomendado por EF Core para apps de escritorio).
/// </summary>
public sealed class ScopedPageFactory(IServiceScopeFactory scopeFactory) : IPageFactory, IDisposable
{
    private IServiceScope? _currentScope;

    public ViewModelBase Create(Type viewModelType)
    {
        _currentScope?.Dispose();
        _currentScope = scopeFactory.CreateScope();
        return (ViewModelBase)_currentScope.ServiceProvider.GetRequiredService(viewModelType);
    }

    public void Dispose() => _currentScope?.Dispose();
}
