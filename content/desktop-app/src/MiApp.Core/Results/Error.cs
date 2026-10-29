namespace MiApp.Core.Results;

/// <summary>Tipo de fallo: permite a la UI (o a una API futura) reaccionar distinto a cada caso.</summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
}

/// <summary>
/// Fallo esperado de un caso de uso. El código es estable (sirve para tests y logs);
/// el mensaje es el texto que ve el usuario.
/// </summary>
public sealed record Error(string Code, string Message, ErrorType Type)
{
    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);
    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);
    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);
}
