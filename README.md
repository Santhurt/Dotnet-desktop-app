# Plantilla `desktop-app`

Plantilla `dotnet new` que genera un proyecto nuevo con la misma estructura y configuración que MiApp Finanzas, pero sin la lógica de finanzas: trae una entidad de ejemplo (`TodoItem`) que atraviesa todas las capas.

**Qué incluye:** arquitectura limpia (Core, Application, Infrastructure, UI), Avalonia + MVVM, Generic Host (configuración, logging, DI), EF Core + SQLite con migraciones, patrón Result para los errores de negocio, `dotnet-ef` como herramienta local, `Directory.Build.props` y `Directory.Packages.props`, analizadores con avisos como errores, y tres proyectos de test (xUnit + NSubstitute).

## Instalar

```bash
# Desde el paquete (recomendado: queda instalada aunque muevas o borres este repositorio)
dotnet pack templates -o templates/nupkg
dotnet new install templates/nupkg/Plantillas.Escritorio.1.2.0.nupkg

# O directamente desde la carpeta (útil mientras editas la plantilla: los cambios se ven al momento)
dotnet new install templates/content/desktop-app
```

## Usar

```bash
dotnet new desktop-app -n Inventario --title "Gestor de inventario"
cd Inventario
dotnet run --project src/Inventario.UI
```

| Opción | Qué hace | Por defecto |
|---|---|---|
| `-n`, `--name` | Nombre de la solución y prefijo de los proyectos y namespaces (`Inventario.Core`…). Usa PascalCase, sin guiones. | Nombre de la carpeta actual |
| `-t`, `--title` | Título visible en la ventana y el menú lateral | `Mi aplicación` |
| `--no-restore` | No restaura los paquetes NuGet al crear el proyecto | `false` |

La carpeta de datos (`~/.local/share/<Nombre>/datos.db`) y los GUID de la solución también se generan a partir del nombre.

## Modificar la plantilla

El contenido está en [`content/desktop-app`](content/desktop-app). Es una solución normal llamada `MiApp`: al generar, `MiApp` se sustituye por el nombre elegido y `Mi aplicación` por el título. La configuración está en [`.template.config/template.json`](content/desktop-app/.template.config/template.json).

Para comprobar los cambios, compílala y pruébala en su sitio, y limpia `bin/` y `obj/` al terminar (el paquete los excluye de todos modos):

```bash
cd templates/content/desktop-app
dotnet build && dotnet test
```

Si cambias el modelo de EF del esqueleto, regenera la migración inicial (`dotnet ef migrations remove` y `dotnet ef migrations add Inicial ...`).

Para publicar una versión nueva, sube `<PackageVersion>` en `Plantillas.Escritorio.csproj`, vuelve a empaquetar y ejecuta `dotnet new install` con el `.nupkg` nuevo (sustituye al anterior).

```bash
dotnet new uninstall Plantillas.Escritorio   # desinstalar
```

### Versiones

| Versión | Cambios |
|---|---|
| 1.2.0 | Comandos en inglés: la plantilla pasa a llamarse `desktop-app` (antes `escritorio-capas`) y la opción `--titulo` pasa a `--title` (`-t` se mantiene). |
| 1.1.0 | Patrón Result: `Result`, `Result<T>` y `Error` en `Core/Results`, catálogo `TodoItemErrors` en `Core/Errors`, servicios que devuelven `Result` y `ViewModelBase.Succeeded`. Se eliminan `DomainException`, `BusinessRuleException` y `EntityNotFoundException`. |
| 1.0.0 | Versión inicial. |
