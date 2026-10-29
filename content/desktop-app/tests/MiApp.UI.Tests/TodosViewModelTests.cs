namespace MiApp.UI.Tests;

public class TodosViewModelTests
{
    private readonly ITodoService _todos = Substitute.For<ITodoService>();

    public TodosViewModelTests() =>
        _todos.GetAllAsync(Arg.Any<CancellationToken>()).Returns([new TodoItemDto(Guid.NewGuid(), "Existente", false)]);

    [Fact]
    public async Task LoadAsync_carga_las_tareas()
    {
        var vm = new TodosViewModel(_todos);

        await vm.LoadAsync();

        Assert.Equal("Existente", Assert.Single(vm.Items).Title);
        Assert.True(vm.HasItems);
    }

    [Fact]
    public void Add_solo_se_puede_ejecutar_con_titulo()
    {
        var vm = new TodosViewModel(_todos);
        Assert.False(vm.AddCommand.CanExecute(null));

        vm.NewTitle = "Nueva";

        Assert.True(vm.AddCommand.CanExecute(null));
    }

    [Fact]
    public async Task Add_crea_la_tarea_y_limpia_el_formulario()
    {
        _todos.CreateAsync("Nueva", Arg.Any<CancellationToken>()).Returns(Result.Success(new TodoItemDto(Guid.NewGuid(), "Nueva", false)));
        var vm = new TodosViewModel(_todos) { NewTitle = "Nueva" };

        await vm.AddCommand.ExecuteAsync(null);

        Assert.Null(vm.NewTitle);
        Assert.True(vm.HasStatus);
        await _todos.Received(1).CreateAsync("Nueva", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Un_fallo_de_negocio_se_muestra_y_conserva_el_formulario()
    {
        _todos.CreateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<TodoItemDto>(Error.Validation("Test", "Título no válido")));
        var vm = new TodosViewModel(_todos) { NewTitle = "Nueva" };

        await vm.AddCommand.ExecuteAsync(null);

        Assert.Equal("Título no válido", vm.ErrorMessage);
        Assert.Equal("Nueva", vm.NewTitle);
        Assert.False(vm.HasStatus);
    }

    [Fact]
    public async Task Borrar_una_tarea_inexistente_muestra_el_error()
    {
        _todos.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(Error.NotFound("Test", "No existe")));
        var vm = new TodosViewModel(_todos);

        await vm.DeleteCommand.ExecuteAsync(new TodoItemDto(Guid.Empty, "X", false));

        Assert.True(vm.HasError);
        Assert.False(vm.HasStatus);
        Assert.False(vm.IsBusy);
    }

    [Fact]
    public async Task Una_excepcion_imprevista_se_muestra_sin_romper_la_app()
    {
        _todos.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Result>(new IOException("disco lleno")));
        var vm = new TodosViewModel(_todos);

        await vm.DeleteCommand.ExecuteAsync(new TodoItemDto(Guid.Empty, "X", false));

        Assert.True(vm.HasError);
        Assert.False(vm.IsBusy);
    }
}
