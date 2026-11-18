# Matriz de pruebas integrales (QA)

Contraseña de todos los usuarios de prueba: `Pa$$word123!`. Marca cada fila con ✅ o ❌ al ejecutarla.

## 1. Acceso y seguridad (WebApp)

| # | Prueba | Resultado esperado | Estado |
| :-- | :--- | :--- | :--- |
| 1.1 | Iniciar sesión con `adminuser` | Redirige al Dashboard del administrador | |
| 1.2 | Iniciar sesión con `agentuser` | Redirige al panel del agente | |
| 1.3 | Iniciar sesión con `clientuser` | Redirige al Home con botones de favorita | |
| 1.4 | Iniciar sesión con `devuser` | Se bloquea con mensaje (solo API) | |
| 1.5 | Contraseña incorrecta | Mensaje "Usuario o contraseña incorrectos." | |
| 1.6 | Registrar un Agente | Queda inactivo; no puede iniciar sesión hasta que el administrador lo active | |
| 1.7 | Registrar un Cliente | Llega correo de activación; al confirmar puede iniciar sesión | |
| 1.8 | Registro con usuario o correo repetido | Error en español, sin crear la cuenta | |
| 1.9 | Sin sesión, abrir `/Admin`, `/Agents`, `/Administrators`, `/Developers`, `/PropertyTypes`, `/SaleTypes`, `/Improvement` | Redirige al login | |
| 1.10 | Como cliente, abrir `/Admin` | Pantalla "Acceso denegado" | |
| 1.11 | Como agente, abrir `/Favorite` | Pantalla "Acceso denegado" | |
| 1.12 | Cerrar sesión | Vuelve al login y se pierde el acceso | |

## 2. Administrador

| # | Prueba | Resultado esperado | Estado |
| :-- | :--- | :--- | :--- |
| 2.1 | Dashboard | Muestra conteos de propiedades, agentes, clientes y desarrolladores | |
| 2.2 | Activar/inactivar un agente | El estado cambia en el listado | |
| 2.3 | Eliminar un agente | Se elimina con sus propiedades | |
| 2.4 | CRUD de administradores y desarrolladores | Validaciones en español; no puede inactivarse a sí mismo | |
| 2.5 | CRUD de tipos de propiedades y de ventas | Validaciones; al eliminar se avisa del efecto en propiedades | |
| 2.6 | CRUD de mejoras | Nombre único; al eliminar solo se quita la asociación con las propiedades | |

## 3. Agente

| # | Prueba | Resultado esperado | Estado |
| :-- | :--- | :--- | :--- |
| 3.1 | Crear propiedad con imágenes y mejoras | Se genera código único de 6 dígitos | |
| 3.2 | Editar y eliminar propiedad | Solo las propias | |
| 3.3 | Editar mi perfil | Se guardan nombre, teléfono y foto | |
| 3.4 | Recibir y responder oferta (aceptar/rechazar) | Al aceptar, la propiedad pasa a Vendida | |
| 3.5 | Bandeja de mensajes | Lista clientes con conversaciones y permite responder | |

## 4. Cliente y público

| # | Prueba | Resultado esperado | Estado |
| :-- | :--- | :--- | :--- |
| 4.1 | Home con filtros (código, tipo, venta, habitaciones, baños, precio) | Lista solo propiedades disponibles, precios en RD$ | |
| 4.2 | Detalle de propiedad | Slider, mejoras y datos del agente | |
| 4.3 | Marcar y quitar favorita | El botón cambia; no se duplica | |
| 4.4 | "Mis propiedades" | Solo favoritas disponibles, con filtros | |
| 4.5 | Hacer una oferta | Se registra y se ve su estado | |
| 4.6 | Chat con el agente desde el detalle | El mensaje aparece en el hilo y en la bandeja del agente | |
| 4.7 | Nuestros agentes (sin sesión) | Solo activos, orden alfabético y búsqueda por nombre/apellido | |
| 4.8 | Propiedades de un agente | Solo disponibles, con filtros | |
| 4.9 | Propiedad sin imagen / usuario sin foto | Muestra imagen por defecto (no imagen rota) | |

## 5. Web API (Swagger)

Iniciar sesión en `POST /api/v1/Account/login` y pegar el token en **Authorize**.

| # | Prueba | Resultado esperado | Estado |
| :-- | :--- | :--- | :--- |
| 5.1 | Login con `adminuser` / `devuser` | 200 con token | |
| 5.2 | Login con `agentuser` o `clientuser` | 401 | |
| 5.3 | `GET /Properties` | 200 con lista, o 204 si no hay | |
| 5.4 | `GET /Properties/{id}` inexistente | 404 | |
| 5.5 | `GET /Properties/code/{code}` | 200 / 404 | |
| 5.6 | `GET /Agents`, `GET /Agents/{id}` | 200 / 204 / 404 | |
| 5.7 | `GET /Agents/{id}/properties` | 200 / 204 / 404 | |
| 5.8 | `PATCH /Agents/{id}/status` con `{ "isActive": false }` como administrador | 204 | |
| 5.9 | `PATCH /Agents/{id}/status` como desarrollador | 403 | |
| 5.10 | Cualquier endpoint sin token | 401 | |
| 5.11 | CRUD de tipos de propiedades, tipos de ventas y mejoras | 200/201/204/400/404 según el caso | |

## 6. Interfaz (Bootstrap)

| # | Prueba | Estado |
| :-- | :--- | :--- |
| 6.1 | Todas las pantallas se ven bien en escritorio y móvil (menú colapsable) | |
| 6.2 | Los mensajes de validación aparecen en español junto a cada campo | |
| 6.3 | Los precios se muestran con formato RD$ | |
| 6.4 | No quedan textos en inglés ni enlaces rotos | |
| 6.5 | `dotnet build` de la solución sin errores | |
