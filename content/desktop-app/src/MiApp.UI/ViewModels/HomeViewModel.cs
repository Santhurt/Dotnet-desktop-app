namespace MiApp.UI.ViewModels;

public sealed partial class HomeViewModel(ITodoService todos) : ViewModelBase
{
    [ObservableProperty]
    private int _pendingCount;

    [ObservableProperty]
    private int _doneCount;

    public override Task LoadAsync() => RunSafeAsync(async () =>
    {
        var items = await todos.GetAllAsync();
        PendingCount = items.Count(i => !i.IsDone);
        DoneCount = items.Count - PendingCount;
    });
}
