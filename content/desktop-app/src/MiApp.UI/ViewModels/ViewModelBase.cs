namespace MiApp.UI.ViewModels;

/// <summary>
/// Base de todos los ViewModels: estado "ocupado", mensajes al usuario y
/// ejecución segura de operaciones. Los fallos de negocio llegan como <see cref="Result"/>
/// (ver <see cref="Succeeded"/>); las excepciones imprevistas se muestran sin romper la app.
/// </summary>
public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? _errorMessage;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatus))]
    private string? _statusMessage;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);
    public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

    /// <summary>Se llama al navegar a la página.</summary>
    public virtual Task LoadAsync() => Task.CompletedTask;

    [RelayCommand]
    private void DismissMessages()
    {
        ErrorMessage = null;
        StatusMessage = null;
    }

    /// <summary>Si el resultado es un fallo, muestra su mensaje. Devuelve true si todo fue bien.</summary>
    protected bool Succeeded(Result result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (result.IsFailure)
        {
            ErrorMessage = result.Error.Message;
        }

        return result.IsSuccess;
    }

    protected async Task RunSafeAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        ErrorMessage = null;
        StatusMessage = null;
        IsBusy = true;
        try
        {
            await action();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ErrorMessage = $"No se pudo acceder al fichero: {ex.Message}";
        }
#pragma warning disable CA1031 // Una aplicación de escritorio no debe cerrarse por un error imprevisto en una acción del usuario.
        catch (Exception ex)
#pragma warning restore CA1031
        {
            ErrorMessage = $"Error inesperado: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
