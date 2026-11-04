# Guía de Contribución y Estándares de Desarrollo - RealEstateApp

Este documento define las reglas oficiales de trabajo para el equipo (**Dev A** y **Dev B**) durante el desarrollo de **RealEstateApp** (.NET 9). El cumplimiento estricto de estas normas garantiza que no tengamos conflictos de *merge* (*merge conflicts*) y que respetemos al 100% la **Onion Architecture** exigida.

---

## 1. Flujo de Ramas (Feature Branch Workflow)

Trabajamos con **`main`** como única rama base y fuente de verdad. **Nadie hace commits ni push directo a `main`** (salvo el commit inicial).

### 1.1. Nomenclatura de Ramas
Toda rama debe crearse a partir de `main` actualizado y seguir este formato en *kebab-case* (minúsculas separadas por guiones):

* **Nuevas funcionalidades (`feature/`):**
  ```text
  feature/issue-<numero>-<descripcion-corta>
  ```
  *Ejemplo:* `feature/issue-2-entidades-dbcontext` o `feature/issue-14-mantenimiento-propiedades`

* **Corrección de errores (`fix/`):**
  ```text
  fix/issue-<numero>-<descripcion-error>
  ```
  *Ejemplo:* `fix/issue-6-validacion-usuario-inactivo`

### 1.2. Ciclo de Vida de una Tarea (Paso a Paso en Terminal)
1. **Sincronizar `main` antes de empezar:**
   ```bash
   git checkout main
   git pull origin main
   ```
2. **Crear tu rama de trabajo:**
   ```bash
   git checkout -b feature/issue-X-nombre-corto
   ```
3. **Verificar compilación antes de subir cambios:**
   ```bash
   dotnet build
   ```
   *(Regla de oro: Prohibido subir código o abrir un PR que no compile con 0 errores).*
4. **Hacer Commit y Push:**
   ```bash
   git add .
   git commit -m "feat: add property entity and configurations (closes #2)"
   git push -u origin feature/issue-X-nombre-corto
   ```
5. **Abrir Pull Request (PR):**
   ```bash
   gh pr create --base main --fill
   ```
6. **Revisión y Merge:** Avísale a tu compañero para revisar o aprobar el PR. Una vez fusionado en `main`, ambos deben hacer `git pull origin main` en sus ramas locales.

---

## 2. Convención de Mensajes de Commit (Conventional Commits)

Todos los mensajes de commit deben ser claros y usar los siguientes prefijos:

* `feat:` Nueva funcionalidad o requerimiento de una Issue (ej. `feat: implement generic repository and service (closes #3)`).
* `fix:` Corrección de un bug o validación faltante (ej. `fix: prevent admin from deactivating own account`).
* `refactor:` Cambio en el código que no añade funcionalidad ni corrige bugs (ej. `refactor: move mapping logic to AutoMapper profile`).
* `chore:` Configuración, paquetes NuGet, scripts o documentación (ej. `chore: add MailKit package to Shared layer`).
* `style:` Ajustes visuales en vistas Razor / Bootstrap (ej. `style: format property cards in public home`).

> **Tip de automatización:** Incluye siempre `(closes #<numero_issue>)` al final del commit o en la descripción del PR para que GitHub mueva la tarjeta automáticamente a **Done**.

---

## 3. Reglas de Oro para Evitar Conflictos de Merge

El 90% de los conflictos en proyectos .NET en equipo ocurren en **4 lugares específicos**. Así los evitaremos:

### 3.1. `Program.cs` (¡No tocar!)
* `Program.cs` (tanto en `WebApp` como en `WebApi`) **solo debe llamar a los métodos de extensión** (`AddApplicationLayer()`, `AddPersistenceInfrastructure()`, etc.).
* **Nunca** registres servicios (`builder.Services.AddTransient...`) directamente en `Program.cs`. Hazlo siempre en el archivo `ServiceRegistration.cs` de la capa correspondiente.

### 3.2. `ServiceRegistration.cs` y `GeneralProfile.cs` (AutoMapper)
Como ambos registraremos servicios y mapeos en estos archivos, usaremos **regiones claramente separadas por módulo o desarrollador** para que Git pueda unir los cambios automáticamente sin chocar en la misma línea:

```csharp
// Ejemplo en ServiceRegistration.cs o GeneralProfile.cs
#region Módulos Dev A (Propiedades, Admin, Ofertas)
services.AddTransient<IPropertyService, PropertyService>();
#endregion

#region Módulos Dev B (Catálogos, Favoritos, Chat)
services.AddTransient<IImprovementService, ImprovementService>();
#endregion
```

### 3.3. Migraciones de Entity Framework Core (`Migrations/`)
* **Un solo encargado por turno:** Durante el Sprint 1, **Dev A** creará las entidades iniciales y la primera migración (`InitialCreate`).
* Si más adelante alguien necesita agregar o modificar un campo en una entidad:
  1. Avisa por chat al compañero antes de tocar la entidad.
  2. Crea un PR pequeño solo con el cambio de la entidad y su migración e intégralo rápido a `main`.
  3. **Nunca** generen dos migraciones al mismo tiempo en ramas distintas.

### 3.4. Configuración Local (`appsettings.json`)
* No subas contraseñas reales de correo SMTP ni cambies el `Server=` del ConnectionString de tu compañero en `appsettings.json`.
* Usa `appsettings.Development.json` (o `dotnet user-secrets`) para tus credenciales locales en Linux/Windows, o acuerden un nombre de base de datos estándar para ambos (ej. `RealEstateAppDb` e `IdentityRealEstateAppDb`).

---

## 4. Estándares de Código, Variables y Archivos (C# / .NET 9)

Para que el código parezca escrito por una sola persona, **todos los nombres de clases, propiedades, métodos y carpetas en C# irán en Inglés**, mientras que **los textos de interfaz de usuario (vistas Razor) y mensajes de validación irán en Español** (según exige el documento funcional).

### 4.1. Convenciones de Nomenclatura (Naming Conventions)

| Elemento | Convención | Ejemplo Correcto | Ejemplo Incorrecto |
| :--- | :--- | :--- | :--- |
| **Clases, Records, Enums** | `PascalCase` | `PropertyType`, `OfferStatus` | `propertyType`, `TipoPropiedad` |
| **Interfaces** | `I` + `PascalCase` | `IPropertyRepository` | `PropertyRepositoryInterface` |
| **Métodos y Propiedades** | `PascalCase` | `GetAllAvailableAsync()`, `Price` | `getAllAvailable()`, `precio` |
| **Variables locales y Parámetros** | `camelCase` | `propertyCode`, `agentId` | `PropertyCode`, `id_agente` |
| **Campos privados (Inyección)** | `_camelCase` | `_propertyRepository`, `_mapper` | `propertyRepository`, `m_mapper` |
| **Constantes** | `PascalCase` o `UPPER_CASE` | `MaxImagesPerProperty` | `max_images` |
| **Métodos Asíncronos** | Sufijo `Async` | `CreateAsync()`, `GetByIdAsync()` | `Create()`, `GetById()` |

### 4.2. Sufijos Obligatorios por Tipo de Archivo

* **Entidades (`Core.Domain/Entities/`):** Nombre en singular, sin sufijos.
  * `Property.cs`, `PropertyType.cs`, `SaleType.cs`, `Improvement.cs`, `PropertyImage.cs`, `PropertyImprovement.cs`, `Offer.cs`, `Message.cs`, `FavoriteProperty.cs`.
* **ViewModels (`Core.Application/ViewModels/<Modulo>/`):** Sufijo `ViewModel` o `SaveViewModel`.
  * `PropertyViewModel.cs`, `SavePropertyViewModel.cs`, `PropertyFilterViewModel.cs`.
  * *Recuerda:* **Todas las validaciones (`[Required]`, `[Range]`, etc.) van aquí**, con su `ErrorMessage` exacto en español como pide el PDF.
* **DTOs para la API (`Core.Application/Dtos/<Modulo>/`):** Sufijo `Dto`, `Request` o `Response`.
  * `PropertyDto.cs`, `CreatePropertyTypeRequest.cs`, `AuthenticationResponse.cs`.
* **Repositorios (`Infrastructure.Persistence/Repositories/`):** Sufijo `Repository`.
  * `GenericRepository.cs`, `PropertyRepository.cs`.
* **Servicios (`Core.Application/Services/`):** Sufijo `Service`.
  * `GenericService.cs`, `PropertyService.cs`.
* **Controladores (`Presentation.WebApp/Controllers/` y `WebApi/Controllers/`):** Sufijo `Controller`.
  * `HomeController.cs`, `PropertyController.cs`, `AgentController.cs`.

---

## 5. Reglas Estrictas de Onion Architecture

> ⚠️ **ADVERTENCIA DEL PROYECTO:** *"El proyecto debe implementar Onion Architecture, aplicada de manera correcta y consistente al 100%. Cualquier error en su aplicación será considerado como una implementación incorrecta."*

1. **`Core.Domain`** no tiene referencias a ningún otro proyecto ni paquetes externos pesados. Solo contiene Entidades puras, Enums y Settings.
2. **`Core.Application`** solo conoce a `Core.Domain`. Aquí viven las interfaces (`IRepository`, `IService`), Servicios, ViewModels, DTOs y AutoMapper. **Prohibido** referenciar `EntityFrameworkCore` (DbContext) o `Identity` directamente en los controladores saltándose `Application`.
3. **Los Controladores de `WebApp` (MVC) y `WebApi`:**
   * **NUNCA** inyectan un `Repository` ni el `DbContext`.
   * **SIEMPRE** inyectan Interfaces de Servicios (`IPropertyService`, `IAccountService`, etc.).
   * **NUNCA** reciben ni devuelven Entidades (`Property`) hacia las vistas o endpoints JSON; siempre reciben/devuelven **ViewModels** (en MVC) o **DTOs** (en WebApi).
4. **Precios y Filtros Globales:**
   * Todos los precios se muestran formateados en **Pesos Dominicanos (`DOP` / `RD$`)**.
   * Todas las vistas que listan propiedades deben incluir el componente/formulario de filtros definido en el Home.

---

## 6. Checklist Antes de Abrir un Pull Request (PR)

Antes de ejecutar `gh pr create`, verifica estos 5 puntos:
- [ ] ¿Hice `git pull origin main` en mi rama para asegurar que no hay conflictos con lo último que subió mi compañero?
- [ ] ¿Ejecuté `dotnet build` y compila sin errores?
- [ ] ¿Mis controladores usan únicamente Servicios y ViewModels/DTOs (cero Entidades o Repositorios en `Presentation`)?
- [ ] ¿Coloqué las validaciones con los mensajes en español sugeridos por el documento dentro de los ViewModels/DTOs?
- [ ] ¿El nombre de mi rama y mi commit siguen el estándar de este documento?