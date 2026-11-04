#!/usr/bin/env bash
set -e

# ==============================================================================
# CONFIGURACIÓN DE VARIABLES (EDITA ESTOS VALORES ANTES DE EJECUTAR)
# ==============================================================================
REPO_NAME="RealEstateApp"
DEV_A="Ronelmambru6"          # Tu username de GitHub
DEV_B="IbnFrias"   # Username de GitHub de tu compañero
VISIBILITY="--private"             # Cambia a "--public" si lo prefieres público

echo -e "\033[1;36m=== 1. CREANDO SOLUCIÓN .NET 9 CON ONION ARCHITECTURE ===\033[0m"

dotnet new gitignore
dotnet new sln -n "$REPO_NAME"

# Capa Core
dotnet new classlib -n "$REPO_NAME.Core.Domain" -o "src/Core/$REPO_NAME.Core.Domain" -f net9.0
dotnet new classlib -n "$REPO_NAME.Core.Application" -o "src/Core/$REPO_NAME.Core.Application" -f net9.0

# Capa Infrastructure
dotnet new classlib -n "$REPO_NAME.Infrastructure.Persistence" -o "src/Infrastructure/$REPO_NAME.Infrastructure.Persistence" -f net9.0
dotnet new classlib -n "$REPO_NAME.Infrastructure.Identity" -o "src/Infrastructure/$REPO_NAME.Infrastructure.Identity" -f net9.0
dotnet new classlib -n "$REPO_NAME.Infrastructure.Shared" -o "src/Infrastructure/$REPO_NAME.Infrastructure.Shared" -f net9.0

# Capa Presentation
dotnet new mvc -n "$REPO_NAME.Presentation.WebApp" -o "src/Presentation/$REPO_NAME.Presentation.WebApp" -f net9.0
dotnet new webapi -n "$REPO_NAME.Presentation.WebApi" -o "src/Presentation/$REPO_NAME.Presentation.WebApi" -f net9.0

# Agregar todos los proyectos a la solución
find src -name "*.csproj" -print0 | xargs -0 dotnet sln add

# Configurar referencias de Onion Architecture
dotnet add "src/Core/$REPO_NAME.Core.Application" reference "src/Core/$REPO_NAME.Core.Domain"
dotnet add "src/Infrastructure/$REPO_NAME.Infrastructure.Persistence" reference "src/Core/$REPO_NAME.Core.Domain" "src/Core/$REPO_NAME.Core.Application"
dotnet add "src/Infrastructure/$REPO_NAME.Infrastructure.Identity" reference "src/Core/$REPO_NAME.Core.Application"
dotnet add "src/Infrastructure/$REPO_NAME.Infrastructure.Shared" reference "src/Core/$REPO_NAME.Core.Application"
dotnet add "src/Presentation/$REPO_NAME.Presentation.WebApp" reference \
    "src/Core/$REPO_NAME.Core.Domain" \
    "src/Core/$REPO_NAME.Core.Application" \
    "src/Infrastructure/$REPO_NAME.Infrastructure.Persistence" \
    "src/Infrastructure/$REPO_NAME.Infrastructure.Identity" \
    "src/Infrastructure/$REPO_NAME.Infrastructure.Shared"
dotnet add "src/Presentation/$REPO_NAME.Presentation.WebApi" reference \
    "src/Core/$REPO_NAME.Core.Domain" \
    "src/Core/$REPO_NAME.Core.Application" \
    "src/Infrastructure/$REPO_NAME.Infrastructure.Persistence" \
    "src/Infrastructure/$REPO_NAME.Infrastructure.Identity" \
    "src/Infrastructure/$REPO_NAME.Infrastructure.Shared"

echo -e "\033[1;36m=== 2. INICIALIZANDO FEATURE BRANCH WORKFLOW Y SUBIENDO A GITHUB ===\033[0m"

git init
git branch -M main
git add .
git commit -m "chore: initial solution structure with Onion Architecture (.NET 9)"

# Crear repositorio en GitHub y hacer push de main
gh repo create "$REPO_NAME" "$VISIBILITY" --source=. --remote=origin --push

# Obtener nombre completo del repo (owner/repo)
REPO_FULL_NAME=$(gh repo view --json nameWithOwner -q ".nameWithOwner")

echo -e "\033[1;36m=== 3. CREANDO LABELS (ETIQUETAS) ===\033[0m"

gh label create "capa: dominio-aplicacion" --color "1D76DB" --description "Core.Domain y Core.Application" --force
gh label create "capa: infraestructura-identity" --color "0E8A16" --description "EF Core, Identity, JWT, Email" --force
gh label create "capa: webapp-mvc" --color "5319E7" --description "Vistas Razor, Controladores MVC y ViewModels" --force
gh label create "capa: web-api" --color "D93F0B" --description "Controladores REST, DTOs y Swagger" --force
gh label create "prioridad: alta" --color "B60205" --description "Bloqueante o crítico para el sprint" --force
gh label create "tipo: fix" --color "FBCA04" --description "Corrección de errores sobre main" --force

echo -e "\033[1;36m=== 4. CREANDO MILESTONES (SPRINTS) ===\033[0m"

gh api "repos/$REPO_FULL_NAME/milestones" -f title="Sprint 1: Arquitectura, BD y Seguridad" -f due_on="2026-11-05T23:59:59Z" -f description="3 nov - 5 nov: Onion Architecture, EF Core, Identity y Login/Registro."
gh api "repos/$REPO_FULL_NAME/milestones" -f title="Sprint 2: Admin y Modulos Publicos" -f due_on="2026-11-09T23:59:59Z" -f description="6 nov - 9 nov: Dashboard Admin, Mantenimientos y Vistas Públicas."
gh api "repos/$REPO_FULL_NAME/milestones" -f title="Sprint 3: Modulos de Agente y Cliente" -f due_on="2026-11-13T23:59:59Z" -f description="10 nov - 13 nov: Propiedades, Favoritos, Chat y Ofertas."
gh api "repos/$REPO_FULL_NAME/milestones" -f title="Sprint 4: Web API, JWT, Swagger y QA" -f due_on="2026-11-16T23:59:59Z" -f description="14 nov - 16 nov: Endpoints API protegidos con JWT, Swagger y cierre."

echo -e "\033[1;36m=== 5. CREANDO ISSUES Y ASIGNANDO TAREAS ===\033[0m"

create_issue() {
    local title="$1"
    local milestone="$2"
    local labels="$3"
    local assignee="$4"
    local body="$5"
    echo -e "\033[1;32mCreando: $title\033[0m"
    gh issue create --title "$title" --milestone "$milestone" --label "$labels" --assignee "$assignee" --body "$body"
}

# SPRINT 1
create_issue "Issue #1: Estructura de la Solución en Onion Architecture" \
    "Sprint 1: Arquitectura, BD y Seguridad" "capa: dominio-aplicacion,prioridad: alta" "$DEV_A" \
    "Verificar referencias al 100% en Onion Architecture (.NET 9), paquetes NuGet base (EF Core, Identity, AutoMapper, JWT, Swagger) y configuración de Inyección de Dependencias."

create_issue "Issue #2: Modelado de Entidades y DbContext (Code First)" \
    "Sprint 1: Arquitectura, BD y Seguridad" "capa: infraestructura-identity,prioridad: alta" "$DEV_A" \
    "Crear entidades (Propiedades, Tipos de Propiedad, Tipos de Venta, Mejoras, Imágenes, Ofertas, Mensajes/Chat, Favoritos) y configurar relaciones en EF Core Code First."

create_issue "Issue #3: Repositorios Genéricos, Servicios Genéricos y AutoMapper" \
    "Sprint 1: Arquitectura, BD y Seguridad" "capa: dominio-aplicacion,prioridad: alta" "$DEV_B" \
    "Implementar GenericRepository, GenericService y configurar perfiles base de AutoMapper para Entities, ViewModels y DTOs."

create_issue "Issue #4: Configuración de ASP.NET Identity, Roles y Seeds" \
    "Sprint 1: Arquitectura, BD y Seguridad" "capa: infraestructura-identity,prioridad: alta" "$DEV_A" \
    "Configurar Identity con usuarios personalizados y crear Seeds por defecto para roles (Administrador, Agente, Cliente, Desarrollador) y usuarios activos iniciales."

create_issue "Issue #5: Servicio de Correo Electrónico y Subida de Imágenes" \
    "Sprint 1: Arquitectura, BD y Seguridad" "capa: infraestructura-identity" "$DEV_B" \
    "Implementar envío de correos SMTP en Infrastructure.Shared para activación de clientes y helper para subida/validación de imágenes (.jpg, .jpeg, .png)."

create_issue "Issue #6: Login, Registro (Únete a la app) y Control de Acceso Web" \
    "Sprint 1: Arquitectura, BD y Seguridad" "capa: webapp-mvc,prioridad: alta" "$DEV_B" \
    "Crear Login (redirección por rol, bloquear Desarrollador en WebApp), registro público de Cliente (envío de correo) y Agente (inactivo), y pantalla de Acceso Denegado."

# SPRINT 2
create_issue "Issue #7: Home del Administrador y Listado de Agentes" \
    "Sprint 2: Admin y Modulos Publicos" "capa: webapp-mvc" "$DEV_A" \
    "Dashboard con indicadores de propiedades y usuarios por rol/estado. Listado de agentes con activar/inactivar y eliminación en cascada de sus propiedades y dependencias."

create_issue "Issue #8: Mantenimiento de Administradores y Desarrolladores (Web)" \
    "Sprint 2: Admin y Modulos Publicos" "capa: webapp-mvc" "$DEV_A" \
    "CRUD de usuarios Administrador y Desarrollador con validaciones en ViewModels (cédula, correo y usuario únicos; impedir auto-edición o auto-inactivación del admin actual)."

create_issue "Issue #9: Mantenimientos de Tipos de Propiedades y Tipos de Ventas (Web)" \
    "Sprint 2: Admin y Modulos Publicos" "capa: webapp-mvc" "$DEV_B" \
    "CRUD de Tipos de Propiedades y Tipos de Ventas mostrando cantidad de propiedades asociadas y eliminando en cascada las propiedades vinculadas y sus datos al borrar un tipo."

create_issue "Issue #10: Mantenimiento de Mejoras (Web)" \
    "Sprint 2: Admin y Modulos Publicos" "capa: webapp-mvc" "$DEV_B" \
    "CRUD de Mejoras. Al eliminar una mejora, remover únicamente la asociación con las propiedades sin eliminar las propiedades."

create_issue "Issue #11: Home Público, Filtros, Búsqueda por Código y Detalle" \
    "Sprint 2: Admin y Modulos Publicos" "capa: webapp-mvc,prioridad: alta" "$DEV_A" \
    "Listar propiedades Disponibles (más reciente primero, precios en DOP). Búsqueda por código y filtros combinables. Pantalla de Detalle con slider, mejoras y datos del agente."

create_issue "Issue #12: Módulo Público de Agentes y sus Propiedades" \
    "Sprint 2: Admin y Modulos Publicos" "capa: webapp-mvc" "$DEV_B" \
    "Listar agentes activos ordenados alfabéticamente con búsqueda por nombre/apellido. Vista de propiedades disponibles del agente seleccionado con filtros."

# SPRINT 3
create_issue "Issue #13: Home del Agente y Mi Perfil" \
    "Sprint 3: Modulos de Agente y Cliente" "capa: webapp-mvc" "$DEV_A" \
    "Listar todas las propiedades del agente (Disponibles y Vendidas con etiqueta visual) y filtros. Pantalla Mi Perfil para actualizar nombre, apellido, teléfono y foto."

create_issue "Issue #14: Mantenimiento de Propiedades del Agente" \
    "Sprint 3: Modulos de Agente y Cliente" "capa: webapp-mvc,prioridad: alta" "$DEV_A" \
    "CRUD de propiedades disponibles del agente. Generación automática de código único de 6 dígitos, carga de 1 a 4 imágenes y validación de existencia de tipos y mejoras."

create_issue "Issue #15: Home del Cliente y Gestión de Favoritos (Mis Propiedades)" \
    "Sprint 3: Modulos de Agente y Cliente" "capa: webapp-mvc" "$DEV_B" \
    "Marcar/desmarcar favoritas en Home del Cliente. Pantalla Mis Propiedades listando solo favoritas en estado Disponible con filtros."

create_issue "Issue #16: Sistema de Chat (Cliente y Agente)" \
    "Sprint 3: Modulos de Agente y Cliente" "capa: webapp-mvc,prioridad: alta" "$DEV_B" \
    "Chat en detalle de propiedad: el cliente envía mensajes al agente; el agente ve listado de clientes con conversaciones y responde a cada hilo."

create_issue "Issue #17: Sistema de Ofertas y Cambio a Vendida (Cliente y Agente)" \
    "Sprint 3: Modulos de Agente y Cliente" "capa: webapp-mvc,prioridad: alta" "$DEV_A" \
    "Cliente: crear oferta y ver historial (bloquear si hay pendiente o aceptada). Agente: listar clientes ofertantes y Aceptar/Rechazar oferta (al aceptar, rechazar las demás pendientes y marcar propiedad como Vendida)."

# SPRINT 4
create_issue "Issue #18: Seguridad API, JWT, Swagger y AccountController" \
    "Sprint 4: Web API, JWT, Swagger y QA" "capa: web-api,prioridad: alta" "$DEV_A" \
    "Configurar JWT Bearer y Swagger. AccountController con Login (bloquear Cliente/Agente), Registro de Desarrollador y Registro de Administrador (solo Admin)."

create_issue "Issue #19: Controladores API de Propiedades y Agentes" \
    "Sprint 4: Web API, JWT, Swagger y QA" "capa: web-api" "$DEV_B" \
    "PropertiesController (List, GetById, GetByCode) y AgentsController (List, GetById, GetAgentProperty, ChangeStatus PATCH) con respuestas HTTP exactas."

create_issue "Issue #20: Controladores API de Tipos de Propiedades, Ventas y Mejoras" \
    "Sprint 4: Web API, JWT, Swagger y QA" "capa: web-api" "$DEV_A" \
    "PropertyTypesController, SaleTypesController e ImprovementsController (Create, Update, List, GetById, Delete) respetando permisos por rol y reglas de borrado."

create_issue "Issue #21: Pruebas Integrales (QA), Revisión UI Bootstrap y Entrega Final" \
    "Sprint 4: Web API, JWT, Swagger y QA" "capa: webapp-mvc,capa: web-api,prioridad: alta" "$DEV_B" \
    "Validar todos los ViewModels, probar flujos con usuarios Seed, verificar estilos Bootstrap y asegurar estabilidad en main."

echo -e "\033[1;33m=== ¡CONFIGURACIÓN COMPLETADA CON ÉXITO! ===\033[0m"
