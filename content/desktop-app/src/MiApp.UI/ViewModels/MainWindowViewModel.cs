namespace MiApp.UI.ViewModels;

public sealed partial class MainWindowViewModel(IPageFactory pageFactory) : ViewModelBase
{
    public IReadOnlyList<NavigationItem> Pages { get; } =
    [
        new("Inicio", "🏠", typeof(HomeViewModel)),
        new("Tareas", "✅", typeof(TodosViewModel)),
    ];

    [ObservableProperty]
    private NavigationItem? _selectedPage;

    [ObservableProperty]
    private ViewModelBase? _currentPage;

    public void Start() => SelectedPage = Pages[0];

    partial void OnSelectedPageChanged(NavigationItem? value)
    {
        if (value is null)
        {
            return;
        }

        CurrentPage = pageFactory.Create(value.ViewModelType);
        _ = CurrentPage.LoadAsync(); // LoadAsync nunca lanza: los errores se muestran en la propia página
    }
}
