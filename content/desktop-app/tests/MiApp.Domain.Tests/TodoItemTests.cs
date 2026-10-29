namespace MiApp.Domain.Tests;

public class TodoItemTests
{
    [Fact]
    public void Create_recorta_el_titulo_y_empieza_pendiente()
    {
        var item = TodoItem.Create("  Comprar pan  ").Value;

        Assert.Equal("Comprar pan", item.Title);
        Assert.False(item.IsDone);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_sin_titulo_devuelve_TitleRequired(string title)
    {
        var result = TodoItem.Create(title);

        Assert.True(result.IsFailure);
        Assert.Equal(TodoItemErrors.TitleRequired, result.Error);
    }

    [Fact]
    public void Create_con_titulo_demasiado_largo_devuelve_TitleTooLong()
    {
        var result = TodoItem.Create(new string('x', TodoItem.TitleMaxLength + 1));

        Assert.Equal(TodoItemErrors.TitleTooLong, result.Error);
    }

    [Fact]
    public void Rename_con_titulo_invalido_no_cambia_el_titulo()
    {
        var item = TodoItem.Create("Original").Value;

        var result = item.Rename(" ");

        Assert.True(result.IsFailure);
        Assert.Equal("Original", item.Title);
    }

    [Fact]
    public void MarkAsDone_y_Reopen_cambian_el_estado()
    {
        var item = TodoItem.Create("Tarea").Value;

        item.MarkAsDone();
        Assert.True(item.IsDone);

        item.Reopen();
        Assert.False(item.IsDone);
    }

    [Fact]
    public void Dos_entidades_son_iguales_si_tienen_el_mismo_id()
    {
        var item = TodoItem.Create("Tarea").Value;

        Assert.Equal(item, item);
        Assert.NotEqual(item, TodoItem.Create("Tarea").Value);
    }
}
