using System.Collections.ObjectModel;

namespace MiApp.UI.ViewModels;

/// <summary>Página de ejemplo con lista, alta, cambio de estado y borrado.</summary>
public sealed partial class TodosViewModel(ITodoService todos) : ViewModelBase
{
    public ObservableCollection<TodoItemDto> Items { get; } = [];

    public bool HasItems => Items.Count > 0;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    private string? _newTitle;

    public override Task LoadAsync() => RunSafeAsync(ReloadAsync);

    private bool CanAdd() => !string.IsNullOrWhiteSpace(NewTitle);

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private Task AddAsync() => RunSafeAsync(async () =>
    {
        var created = await todos.CreateAsync(NewTitle!);
        if (!Succeeded(created))
        {
            return; // el formulario conserva el texto para que el usuario lo corrija
        }

        NewTitle = null;
        await ReloadAsync();
        StatusMessage = $"Tarea «{created.Value.Title}» creada.";
    });

    [RelayCommand]
    private Task ToggleAsync(TodoItemDto item) => RunSafeAsync(async () =>
    {
        var result = await todos.SetDoneAsync(item.Id, !item.IsDone);
        Succeeded(result);
        await ReloadAsync(); // también en caso de fallo: la lista vuelve a reflejar la BD
    });

    [RelayCommand]
    private Task DeleteAsync(TodoItemDto item) => RunSafeAsync(async () =>
    {
        var result = await todos.DeleteAsync(item.Id);
        await ReloadAsync();
        if (Succeeded(result))
        {
            StatusMessage = $"Tarea «{item.Title}» eliminada.";
        }
    });

    private async Task ReloadAsync()
    {
        Items.Clear();
        foreach (var item in await todos.GetAllAsync())
        {
            Items.Add(item);
        }

        OnPropertyChanged(nameof(HasItems));
    }
}
