# Mi aplicación

Aplicación de escritorio en **C# / .NET 9** con **arquitectura limpia**, generada con la plantilla `desktop-app`.

## Cómo ejecutarla

Requisitos: [.NET SDK 9](https://dotnet.microsoft.com/download).

```bash
dotnet run --project src/MiApp.UI   # arranca la aplicación
dotnet test                         # pruebas de dominio, casos de uso y ViewModels
```

La base de datos SQLite se crea en `~/.local/share/MiApp/datos.db` (Linux) o `%LOCALAPPDATA%\MiApp\datos.db` (Windows). La ruta y la cultura se configuran en [`src/MiApp.UI/appsettings.json`](src/MiApp.UI/appsettings.json).

## Arquitectura

```
          ┌────────────────────┐
          │      MiApp.UI      │  Avalonia + MVVM (CommunityToolkit.Mvvm)
          └──┬──────────────┬──┘
             │ usa          │ solo para registrar dependencias (raíz de composición)
             ▼              ▼
┌──────────────────┐  ┌──────────────────────┐
│ MiApp.Application│◄─┤ MiApp.Infrastructure │  EF Core + SQLite
└────────┬─────────┘  └──────────┬───────────┘
         │                       │
         ▼                       ▼
     ┌───────────────────────────────┐
     │          MiApp.Core           │  Dominio puro: sin dependencias externas
     └───────────────────────────────┘
```

Las dependencias siempre apuntan hacia el dominio. `Core` define los contratos (`ITodoItemRepository`, `IUnitOfWork`) e `Infrastructure` los implementa. Los ViewModels solo conocen las interfaces de `Application` y sus DTOs.

| Capa | Qué va ahí |
|---|---|
| `Core` | Entidades (`Entity<TId>` con IDs fuertemente tipados), objetos de valor, interfaces de repositorios, `Result`/`Error` (`Results/`) y catálogos de errores por entidad (`Errors/`) |
| `Application` | Servicios de casos de uso (`TransactionalService` guarda o descarta en bloque), DTOs, `AddApplication()` |
| `Infrastructure` | `AppDbContext`, configuraciones de EF, repositorios, migraciones, `AddInfrastructure()` |
| `UI` | `AppHost` (Generic Host: configuración, logging, DI), ViewModels, vistas, estilos y tema claro/oscuro |

`TodoItem` es una entidad de ejemplo que atraviesa todas las capas. Úsala como modelo y sustitúyela por las tuyas.

## Añadir una entidad nueva

1. **Core:** el ID en `ValueObjects/EntityIds.cs`, la entidad en `Entities/` (su `Create` devuelve `Result<T>`), su catálogo de errores en `Errors/` y su repositorio en `Interfaces/`.
2. **Infrastructure:** el `DbSet` y el conversor del ID en `AppDbContext`, una clase `IEntityTypeConfiguration` en `Configuration/`, el repositorio en `Repositories/` y su registro en `DependencyInjection.cs`.
3. **Application:** el DTO, la interfaz del servicio (las operaciones que pueden fallar devuelven `Result`) y su implementación, registrada en `DependencyInjection.cs`.
4. **UI:** el ViewModel y la vista, registrados en `AppHost`, `ViewLocator` y `MainWindowViewModel.Pages`.
5. **Migración:** ver abajo.

## Patrón Result

Los fallos esperados (datos inválidos, registro inexistente) no lanzan excepciones: se devuelven como `Result` / `Result<T>` con un `Error` (código, mensaje y tipo: `Validation`, `NotFound`, `Conflict`). Las excepciones quedan para lo imprevisto (disco, base de datos, bugs).

| Capa | Cómo lo usa |
|---|---|
| `Core` | Las entidades devuelven `Result<T>` desde `Create` y `Result` desde los métodos que validan. Los errores viven en un catálogo por entidad (`Errors/TodoItemErrors.cs`). |
| `Application` | Los servicios devuelven `Result`. `TransactionalService.ExecuteAsync` guarda solo si el resultado es correcto y descarta los cambios si falla o si salta una excepción. |
| `UI` | `ViewModelBase.Succeeded(result)` muestra el mensaje del error en la barra de avisos. `RunSafeAsync` sigue capturando las excepciones imprevistas para que la app no se cierre. |

```csharp
// Core: devolver un fallo o un valor (conversiones implícitas)
if (string.IsNullOrEmpty(trimmed)) return TodoItemErrors.TitleRequired;
return trimmed;

// UI: reaccionar al resultado
var created = await todos.CreateAsync(NewTitle!);
if (!Succeeded(created)) return;   // muestra el error y conserva el formulario
```

## Migraciones

Al arrancar, `DbInitializer` aplica las migraciones pendientes con `MigrateAsync()`. `dotnet-ef` está fijado como herramienta local en la misma versión que EF Core:

```bash
dotnet tool restore
dotnet ef migrations add NombreDelCambio --project src/MiApp.Infrastructure --output-dir Data/Migrations
dotnet ef migrations remove --project src/MiApp.Infrastructure   # deshacer la última (si no se ha publicado)
```

Revisa siempre el código generado: si renombras una propiedad, EF puede interpretarlo como borrar una columna y crear otra.

## Convenciones del proyecto

- `Directory.Build.props`: .NET 9, `Nullable`, analizadores `latest-recommended` y **avisos como errores**.
- `Directory.Packages.props`: versiones de NuGet centralizadas; los `.csproj` no llevan versión.
- `tests/Directory.Build.props`: xUnit + NSubstitute para todos los proyectos de test.
- Las migraciones están marcadas como código generado (`Data/Migrations/.editorconfig`) para que los analizadores no las revisen.
