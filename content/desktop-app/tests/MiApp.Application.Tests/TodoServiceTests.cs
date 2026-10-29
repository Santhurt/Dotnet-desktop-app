namespace MiApp.Application.Tests;

public class TodoServiceTests
{
    private readonly InMemoryTodoItemRepository _repository = new();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly TodoService _service;

    public TodoServiceTests() => _service = new TodoService(_repository, _unitOfWork);

    [Fact]
    public async Task CreateAsync_guarda_la_tarea()
    {
        var result = await _service.CreateAsync("Escribir tests");

        Assert.True(result.IsSuccess);
        Assert.Equal("Escribir tests", Assert.Single(_repository.Items).Title);
        Assert.False(result.Value.IsDone);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateAsync_con_titulo_invalido_devuelve_el_error_y_no_guarda()
    {
        var result = await _service.CreateAsync(" ");

        Assert.Equal(TodoItemErrors.TitleRequired, result.Error);
        Assert.Empty(_repository.Items);
        _unitOfWork.Received(1).DiscardChanges();
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllAsync_muestra_primero_las_pendientes_y_por_orden_alfabetico()
    {
        var done = TodoItem.Create("A hecha").Value;
        done.MarkAsDone();
        _repository.Items.AddRange([TodoItem.Create("C pendiente").Value, done, TodoItem.Create("B pendiente").Value]);

        var titles = (await _service.GetAllAsync()).Select(i => i.Title);

        Assert.Equal(["B pendiente", "C pendiente", "A hecha"], titles);
    }

    [Fact]
    public async Task SetDoneAsync_cambia_el_estado()
    {
        var item = TodoItem.Create("Tarea").Value;
        _repository.Add(item);

        var result = await _service.SetDoneAsync(item.Id.Value, isDone: true);

        Assert.True(result.Value.IsDone);
        Assert.True(item.IsDone);
    }

    [Fact]
    public async Task DeleteAsync_de_una_tarea_inexistente_devuelve_NotFound()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Una_excepcion_imprevista_descarta_los_cambios_y_se_propaga()
    {
        _unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns<int>(_ => throw new InvalidOperationException("BD caída"));

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync("Tarea"));

        _unitOfWork.Received(1).DiscardChanges();
    }
}
