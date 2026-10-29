namespace MiApp.Domain.Tests;

public class ResultTests
{
    private static readonly Error SomeError = Error.Validation("Test.Error", "Algo salió mal.");

    [Fact]
    public void Un_valor_se_convierte_en_exito()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Un_error_se_convierte_en_fallo()
    {
        Result<int> result = SomeError;

        Assert.True(result.IsFailure);
        Assert.Equal(SomeError, result.Error);
    }

    [Fact]
    public void Leer_Value_de_un_fallo_lanza() =>
        Assert.Throws<InvalidOperationException>(() => Result.Failure<int>(SomeError).Value);

    [Fact]
    public void Leer_Error_de_un_exito_lanza() =>
        Assert.Throws<InvalidOperationException>(() => Result.Success().Error);

    [Fact]
    public void Map_transforma_el_exito_y_propaga_el_fallo()
    {
        Assert.Equal("42", Result.Success(42).Map(n => n.ToString(System.Globalization.CultureInfo.InvariantCulture)).Value);
        Assert.Equal(SomeError, Result.Failure<int>(SomeError).Map(n => n * 2).Error);
    }
}
