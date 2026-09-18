# Payment Orders

Esqueleto académico de una API de órdenes de pago en .NET 8, C#, Entity Framework Core y SQLite local. La solución aplica Clean Architecture:

```text
Api → Application → Domain ← Infrastructure
```

La API, persistencia, reglas de dominio, estados e idempotencia están cableadas. Los patrones Factory, Builder y Strategy + Factory permanecen intencionalmente incompletos para que el estudiante los implemente.

## Actividad en dos clases

La actividad está diseñada para estudiantes de quinto semestre y se resuelve en dos sesiones. El trabajo se limita a los patrones creacionales pendientes: la persistencia local, los endpoints y la arquitectura ya están entregados.

- Clase 1: comprender la solución y completar Factory y Builder.
- Clase 2: completar Strategy + Factory, ejecutar pruebas y preparar la explicación individual.

Pueden usar inteligencia artificial para entender conceptos, revisar errores y formular preguntas. Cada estudiante debe escribir e integrar su propia solución; no puede sustituirla por una respuesta completa generada por IA ni modificar las pruebas.

La actividad tiene doble nota, con el mismo peso:

- 50%: implementación funcional del equipo y cumplimiento de las pruebas.
- 50%: sustentación individual de las decisiones de diseño.

Las comisiones que debe calcular el ejercicio son: nacional 1%, internacional 3% y programada 0.5%. Redondea el resultado a dos decimales.

Consulta la [guía de la actividad en dos clases](docs/actividad-2-clases.md) antes de comenzar.

## Estructura

```text
src/
  PaymentOrders.Api/             REST, Swagger y composición
  PaymentOrders.Application/     Casos de uso, DTOs y puertos
  PaymentOrders.Domain/          Agregado, invariantes y ejercicios
  PaymentOrders.Infrastructure/  EF Core, SQLite y repositorios
tests/
  PaymentOrders.Domain.Tests/        Invariantes y estados
  PaymentOrders.ArchitectureTests/  Dependencias prohibidas de Domain
  PaymentOrders.ContractTests/      Muestras públicas que fallan hasta completar ejercicios
migrations/                      Nota sobre la base de datos local
docs/                            Diseño y plan de implementación
```

## Ejecución local

No necesitas Docker ni PostgreSQL. La API usa SQLite local y aplica las migraciones de EF Core al iniciar.

```powershell
dotnet tool restore
dotnet ef database update --project src/PaymentOrders.Infrastructure/PaymentOrders.Infrastructure.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj
dotnet run --project src/PaymentOrders.Api/PaymentOrders.Api.csproj
```

Swagger queda disponible en `http://localhost:5205/swagger`.

Para crear una migración después de cambiar el modelo:

```powershell
dotnet ef migrations add <NombreDescriptivo> --project src/PaymentOrders.Infrastructure/PaymentOrders.Infrastructure.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj --output-dir Persistence/Migrations
```

## MySQL opcional

Si el equipo tiene MySQL instalado o accesible, puede usarlo como proveedor alternativo. No requiere Docker, pero sí una instancia MySQL disponible.

Configura el proveedor y la conexión en la misma sesión de PowerShell:

```powershell
$env:Database__Provider = "MySql"
$env:ConnectionStrings__PaymentOrders = "Server=localhost;Port=3306;Database=payment_orders;User=root;Password=tu_clave"
dotnet ef database update --project src/PaymentOrders.Infrastructure.MySqlMigrations/PaymentOrders.Infrastructure.MySqlMigrations.csproj --startup-project src/PaymentOrders.Api/PaymentOrders.Api.csproj
dotnet run --project src/PaymentOrders.Api/PaymentOrders.Api.csproj
```

La migración MySQL vive en `src/PaymentOrders.Infrastructure.MySqlMigrations`. Para volver a SQLite, cierra la sesión de PowerShell o ejecuta `$env:Database__Provider = "Sqlite"` y usa la migración del proyecto `PaymentOrders.Infrastructure`.

## Endpoints

- `POST /api/payment-orders` — requiere el encabezado `Idempotency-Key`.
- `GET /api/payment-orders/{orderId}` — consulta una orden.
- `GET /api/payment-orders` — filtra por `orderType`, `status`, fecha y paginación.
- `POST /api/payment-orders/{orderId}/cancel` — cancela solo desde `Pending`.
- `POST /api/payment-orders/{orderId}/process` — lleva una orden de `Created` a `Pending`, `Processing` y `Completed`.
- `GET /api/payment-orders/{orderId}/events` — consulta la auditoría inmutable.

## Archivos que debe completar el estudiante

No modifiques los contratos públicos ni los tests. Implementa solamente los cuerpos marcados como ejercicio en:

- `src/PaymentOrders.Domain/Orders/Patterns/PaymentOrderFactory.cs`
- `src/PaymentOrders.Domain/Orders/Patterns/PaymentOrderBuilder.cs`
- `src/PaymentOrders.Domain/Orders/Patterns/CommissionStrategies.cs`
- `src/PaymentOrders.Domain/Orders/Patterns/CommissionStrategyFactory.cs`

Los contratos de muestra en `PaymentOrders.ContractTests` fallan hoy de forma deliberada hasta que estos patrones funcionen. Es una muestra pública reducida: la evaluación real incorpora una suite privada.

## Pruebas

Ejecuta toda la solución para observar el estado completo, incluidos los tres contratos pendientes:

```powershell
dotnet test PaymentOrders.slnx --no-restore
```

Mientras los ejercicios sigan pendientes, ejecuta las pruebas ya implementadas sin ocultar ni modificar los contratos:

```powershell
dotnet test PaymentOrders.slnx --no-restore --filter "FullyQualifiedName!~CreationalPatternContractSamples"
```

## Guía para la defensa técnica

Documenta tus respuestas aquí antes de la defensa:

### Por qué Factory

Se elige Factory porque la parte que arma las órdenes no debería tener que decidir cosas como cuál tipo de orden es esta y cómo la deberia construir por eso mismo le delegamos esa decisión a una pieza aparte, el cual su trabajo es decirnos qué tipo de orden necesita y esa pieza la prepara teniendo en cuenta lo necesario para construirla entonces así quien pide la orden nunca tiene que saber los detalles internos de cada tipo ya que solo pide y recibe

Y si mañana aparece un tipo de orden nuevo, solo esa pieza necesita aprender a manejarlo; el resto del sistema sigue igual.

### Por qué Builder

Se usa Builder ya que una orden necesita varios datos antes de poder siquiera existir, y no todos llegan al mismo tiempo ni de la misma forma, entonces en vez de exigir todo de una vez, vamos entregando los datos poco a poco, uno por uno, hasta que están completos, aca es lo fundamental ya que solo al final, cuando decimos ya termine, ahora construyela, se revisa que todo esté correcto y ahí sí nace la orden. Si algo obligatorio falta o está mal, se rechaza justo en ese momento final, no se rechaza antes ni después.

### Qué ocurriría sin ellos

Si no existiera esta manera organizada de crear las órdenes, cada parte del sistema que necesite una tendría que saber por su cuenta qué tipo de orden es, qué datos necesita cada una y cómo armarla correctamente esto mediante un unico switch.

Eso significa que ese conocimiento quedaría regado en varios lugares en vez de estar concentrado en uno solo lugar Y si algún día cambia una regla de cómo se arma una orden, tendrías que ir a corregirla en todos esos lugares y todo esto con el riesgo de olvidar alguno creando errores y extendiendo el tiempo de correcion.

### Dónde viven las reglas de negocio

todas las condiciones que hacen válida a una orden que el monto tenga sentido, que las cuentas sean distintas, que las internacionales traigan su código bancario, que las programadas tengan una fecha futura, y que solo pueda cambiar de estado en el orden correcto están concentradas en un solo lugar osea dentro del PaymentOrder

### Cómo se garantiza idempotencia

Cada solicitud trae un número de referencia único, y antes de crear una orden el sistema revisa si ya usó ese número antes. Si ya existe, devuelve la orden que ya había creado en vez de duplicarla.

Idempotency-Key (GetOrderIdAsync).

### Cómo se protegen las transiciones de estado

Una orden solo puede avanzar por sus etapas en el orden correcto, sin saltarse ninguna. Antes de cambiarla de etapa, el sistema revisa en cuál está actualmente, y si el cambio no tiene sentido desde ahí, lo rechaza.

