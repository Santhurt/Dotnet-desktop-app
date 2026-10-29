namespace MiApp.Core.Results;

/// <summary>
/// Resultado de una operación que puede fallar por una regla de negocio.
/// Los fallos esperados (datos inválidos, registro inexistente) se devuelven como <see cref="Error"/>;
/// las excepciones quedan para lo imprevisto (disco, base de datos, bugs).
/// </summary>
public class Result
{
    private readonly Error? _error;

    protected Result(Error? error) => _error = error;

    public bool IsSuccess => _error is null;
    public bool IsFailure => !IsSuccess;

    /// <summary>El error del fallo. Leerlo en un resultado correcto es un bug, por eso lanza.</summary>
    public Error Error => _error ?? throw new InvalidOperationException("Un resultado correcto no tiene error.");

    public static Result Success() => new(null);

    public static Result Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    public static Result<T> Success<T>(T value) => new(value);

    public static Result<T> Failure<T>(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    // Permite escribir "return SomeErrors.Xxx;" en un método que devuelve Result.
    public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>Resultado que, si es correcto, lleva un valor.</summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T value) : base(null) => _value = value;

    internal Result(Error error) : base(error)
    {
    }

    /// <summary>El valor del éxito. Leerlo en un fallo es un bug, por eso lanza.</summary>
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException($"No hay valor: {Error.Message}");

    // Permiten escribir "return valor;" o "return SomeErrors.Xxx;" en un método que devuelve Result<T>.
    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) => Failure<T>(error);

    /// <summary>Transforma el valor si es un éxito; si es un fallo, propaga el mismo error.</summary>
    public Result<TOut> Map<TOut>(Func<T, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return IsSuccess ? map(Value) : Error;
    }
}
