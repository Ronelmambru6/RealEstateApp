# RealEstateApp

Aplicación inmobiliaria desarrollada en **.NET 9** con **Onion Architecture**: una aplicación web MVC (clientes, agentes y administradores) y una Web API REST con JWT y Swagger (administradores y desarrolladores).

## Equipo

| Rol | GitHub |
| :--- | :--- |
| Dev A | [@IbnFrias](https://github.com/IbnFrias) |
| Dev B | [@Ronelmambru6](https://github.com/Ronelmambru6) |

## Arquitectura

```
src/
├── Core/
│   ├── RealEstateApp.Core.Domain          # Entidades, enums y settings
│   └── RealEstateApp.Core.Application     # Interfaces, servicios, ViewModels, DTOs, AutoMapper
├── Infrastructure/
│   ├── RealEstateApp.Infrastructure.Persistence  # ApplicationDbContext, repositorios, servicios de datos
│   ├── RealEstateApp.Infrastructure.Identity     # ASP.NET Identity, roles, seeds, servicios de cuentas
│   └── RealEstateApp.Infrastructure.Shared       # Correo SMTP (MailKit)
└── Presentation/
    ├── RealEstateApp.Presentation.WebApp  # MVC (Razor + Bootstrap)
    └── RealEstateApp.Presentation.WebApi  # API REST (JWT + Swagger)
```

Los controladores solo usan interfaces de servicios y ViewModels (WebApp) o DTOs (WebApi). Las convenciones de trabajo están en [Convenciones.md](Convenciones.md).

## Requisitos

- .NET 9 SDK
- SQL Server LocalDB (viene con Visual Studio)

## Puesta en marcha

1. **Credenciales de correo (opcional, solo para el registro de clientes):** copia la sección `MailSettings` de `appsettings.json` a `appsettings.Development.json` del proyecto WebApp y completa tus datos SMTP. No subas contraseñas reales al repositorio.
2. **Base de datos:** en la **Consola del Administrador de paquetes** de Visual Studio, aplica las migraciones de los dos contextos (cada uno con su proyecto como "Proyecto predeterminado"):

   ```powershell
   Update-Database -Context ApplicationDbContext -Project RealEstateApp.Infrastructure.Persistence -StartupProject RealEstateApp.Presentation.WebApp
   Update-Database -Context IdentityContext -Project RealEstateApp.Infrastructure.Identity -StartupProject RealEstateApp.Presentation.WebApp
   ```

3. **Ejecutar:**
   - Web: proyecto `RealEstateApp.Presentation.WebApp` (`https://localhost:7136`).
   - API: proyecto `RealEstateApp.Presentation.WebApi` (`https://localhost:7213/swagger`).

Al iniciar, la aplicación crea automáticamente los roles y los usuarios de prueba.

## Usuarios de prueba (seed)

Contraseña de todos: `Pa$$word123!`

| Rol | Usuario | Correo | Acceso |
| :--- | :--- | :--- | :--- |
| Administrador | `adminuser` | admin@realestate.com | Web y API |
| Agente | `agentuser` | agente@realestate.com | Web |
| Cliente | `clientuser` | cliente@realestate.com | Web |
| Desarrollador | `devuser` | dev@realestate.com | Solo API |

## Módulos

- **Público:** catálogo de propiedades con filtros, detalle con slider, agentes activos y sus propiedades.
- **Cliente:** favoritas y "Mis propiedades", ofertas y chat con el agente.
- **Agente:** panel, perfil, mantenimiento de propiedades, ofertas recibidas y bandeja de mensajes.
- **Administrador:** dashboard, agentes, administradores, desarrolladores, tipos de propiedades, tipos de ventas y mejoras.
- **Web API** (`/api/v1`): autenticación JWT, propiedades, agentes y catálogos (tipos de propiedades, tipos de ventas y mejoras).

## Pruebas

La matriz de pruebas integrales con los usuarios del seed está en [docs/QA.md](docs/QA.md).
