# 🎨 FIGURASGEOMETRICASAPI

> **Proyecto:** FigurasGeometricasApi
> **Stack:** .NET 8 (C#), ASP.NET Core, EF Core (SQLite), Swagger (OpenAPI)
> **Objetivo:** Crear una API (Application Programming Interface) que guarda **figuras geométricas** (Círculo, Rectángulo, Triángulo), calcula su área y perímetro, y permite crear/leer/editar/borrar esas figuras.

---

<p align="center">
  <img alt="banner" src="https://img.shields.io/badge/Proyecto-Figuras%20Geometr%C3%ADcas-blueviolet?style=for-the-badge&logo=dotnet" />
</p>

---

## 📚 Índice

1.¿Qué es esta API y por qué sirve?
2. Cómo ejecutar el proyecto
3. Estructura de archivos
4. Explicación **línea por línea** de los archivos clave:

   * `Program.cs`
   * `Controllers/FigurasController.cs`
   * `Data/FigurasContext.cs`
   * `Models/*` (Figura, Circulo, Rectangulo, Triangulo)
   * `DTOs/*` (create, read, update)
   * `Services/IFiguraService.cs` y `Services/FiguraService.cs`
5. Cómo probar (ejemplos fáciles: Swagger, curl)
6. Ejemplos de uso paso a paso (creá una figura, listá, borrá)
7. Preguntas que te pueden hacer y cómo contestarlas
8. Qué mejorar después (ideas para seguir)
9. Licencia / agradecimientos

---

## 1) ¿Qué es esta API y por qué sirve?

Imaginá que tu API es como un **local donde guardás fichas** de figuras. Cada ficha tiene:

* un nombre,
* el tipo de figura (círculo, rectángulo, triángulo),
* sus medidas (radio, base+altura, o lados),
* y calculos automáticos (área y perímetro).

La API permite que otras apps (o vos desde la consola) pidan la lista de fichas, agreguen una nueva, editen una o la borren. Además la API guarda todo en una base de datos (un archivo `figuras.db`) usando **SQLite**.

---

## 2) Cómo ejecutar el proyecto (rápido)

1. Tenés que tener instalado .NET 8 SDK.
2. Abrí una terminal en la carpeta del proyecto (la que tiene `FigurasGeometricasApi.csproj`).
3. Ejecutá:

```bash
dotnet restore
dotnet run
```

4. Abrí en el navegador:

```
http://localhost:5038/swagger
```

Allí vas a ver la interfaz de **Swagger** para probar la API con formularios amigables.

---

## 3) Estructura de archivos (resumen)
```
/FigurasGeometricasApi
  ├─ Program.cs                      → Archivo principal que inicia toda la aplicación, configura Swagger, la base de datos y los servicios.
  │
  ├─ Controllers/
  │   └─ FigurasController.cs        → Controlador principal: maneja las rutas (GET, POST, PUT, DELETE) para trabajar con las figuras.
  │
  ├─ Data/
  │   └─ FigurasContext.cs           → Clase que conecta el código con la base de datos usando Entity Framework Core (EF Core).
  │
  ├─ Models/
  │   ├─ Figura.cs                   → Clase base (abstracta) de todas las figuras. Contiene las propiedades y métodos comunes (Área y Perímetro).
  │   ├─ Circulo.cs                  → Modelo que representa un círculo, con su radio y fórmulas para calcular área y perímetro.
  │   ├─ Rectangulo.cs               → Modelo que representa un rectángulo, con base y altura, y sus cálculos correspondientes.
  │   └─ Triangulo.cs                → Modelo del triángulo, con tres lados y cálculo del área usando la fórmula de Herón.
  │
  ├─ DTOs/
  │   ├─ FiguraCreateDto.cs          → Estructura de datos usada cuando el usuario crea una nueva figura (solo recibe los campos necesarios).
  │   ├─ FiguraReadDto.cs            → Estructura que devuelve la API cuando se consulta una figura (incluye tipo, área y perímetro).
  │   └─ FiguraUpdateDto.cs          → Estructura que se usa para actualizar una figura existente (permite modificar valores específicos).
  │
  ├─ Services/
  │   ├─ IFiguraService.cs           → Interfaz que define qué métodos debe tener el servicio (crear, listar, borrar, etc.).
  │   └─ FiguraService.cs            → Implementación de la lógica principal: decide qué tipo de figura crear, hace los cálculos y guarda los datos.
  │
  ├─ appsettings.json                → Archivo de configuración general (por ejemplo, la cadena de conexión a la base de datos).
  │
  └─ figuras.db                      → Archivo físico de la base de datos SQLite donde se guardan las figuras creadas.
```
---

## 4) Explicación línea por línea de los archivos clave

Voy a explicar las partes más importantes línea por línea. Para no hacerlo gigantesco, pongo las secciones con el código y después explico cada línea con palabras simples.

---

### 🟦 `Program.cs` — arranque de la app

```csharp
using FigurasGeometricasApi.Data;
using FigurasGeometricasApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=figuras.db";
builder.Services.AddDbContext<FigurasContext>(options =>
    options.UseSqlite(conn)
);

// Application services
builder.Services.AddScoped<IFiguraService, FiguraService>();

var app = builder.Build();

// Ensure DB created (best for dev; for production use migrations)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FigurasContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
```

#### Explicación línea por línea (fácil)

* `using ...` → estas líneas traen código que ya existe (como cajas de herramientas). Son necesarias para usar EF Core y tus propios archivos.
* `var builder = WebApplication.CreateBuilder(args);` → crea el *constructor* de la aplicación; prepara todo lo que hace falta antes de arrancar.
* `builder.Services.AddControllers();` → le dice al programa: "voy a tener controllers (las rutas de la API)".
* `builder.Services.AddEndpointsApiExplorer();` → prepara metadatos para Swagger.
* `builder.Services.AddSwaggerGen();` → agrega el generador de Swagger para que la API tenga documentación automática.
* `var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=figuras.db";` → intenta leer la cadena de conexión desde `appsettings.json`. Si no existe, usa el archivo `figuras.db`.
* `builder.Services.AddDbContext<FigurasContext>(options => options.UseSqlite(conn));` → conecta EF Core con la base de datos SQLite usando la conexión `conn`.
* `builder.Services.AddScoped<IFiguraService, FiguraService>();` → registra el servicio que contiene la lógica (esto permite usar `IFiguraService` en los controllers).
* `var app = builder.Build();` → arma la aplicación ya con toda la configuración.
* El bloque `using (var scope = ...) { ... }`:

  * Crea un *scope* temporario para pedir el `FigurasContext` y llamar `db.Database.EnsureCreated();`
  * `EnsureCreated()` crea el archivo de la base de datos y las tablas si no existen (práctico para desarrollo).
* `if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }` → si estás en modo desarrollo, habilita la interfaz de Swagger en `/swagger`.
* `app.UseHttpsRedirection();` → redirige tráfico a HTTPS (seguridad).
* `app.MapControllers();` → conecta los controllers para que las rutas funcionen.
* `app.Run();` → arranca el servidor y queda escuchando pedidos.

---

### 🟦 `Controllers/FigurasController.cs` — puertas de la API (endpoints)

```csharp
[ApiController]
[Route("api/[controller]")]
public class FigurasController : ControllerBase
{
    private readonly IFiguraService _service;
    private readonly ILogger<FigurasController> _logger;

    public FigurasController(IFiguraService service, ILogger<FigurasController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FiguraReadDto>>> GetAll()
    {
        var list = await _service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}", Name = "GetFiguraById")]
    public async Task<ActionResult<FiguraReadDto>> GetById(int id)
    {
        var f = await _service.GetByIdAsync(id);
        if (f == null) return NotFound();
        return Ok(f);
    }

    [HttpPost]
    public async Task<ActionResult<FiguraReadDto>> Create([FromBody] FiguraCreateDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtRoute("GetFiguraById", new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Creación inválida");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] FiguraUpdateDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    [HttpGet("totales")]
    public async Task<ActionResult> Totales()
    {
        var (area, perim) = await _service.TotalesAsync();
        return Ok(new { area, perimetro = perim });
    }
}
```

#### Explicación simple

* `[ApiController]` → indica que esto es un controller para una API (activa validaciones y comportamientos útiles).
* `[Route("api/[controller]")]` → la ruta base será `api/figuras` (porque el controller se llama `FigurasController`).
* El controller recibe por *inyección* un servicio (`IFiguraService`) y un logger, que usa para pedir datos y escribir advertencias.
* `GetAll()` → `GET /api/figuras` → devuelve la lista de figuras.
* `GetById(int id)` → `GET /api/figuras/{id}` → devuelve una figura específica o 404 si no existe.
* `Create(...)` → `POST /api/figuras` → crea una figura a partir del DTO enviado en el cuerpo (JSON). Si hay error devuelve `400`.
* `Update(...)` → `PUT /api/figuras/{id}` → actualiza una figura; si no existe devuelve `404`.
* `Delete(int id)` → `DELETE /api/figuras/{id}` → borra una figura; devuelve `204` si borró.
* `Totales()` → `GET /api/figuras/totales` → devuelve el área total y el perímetro total de todas las figuras.

---

### 🟦 `Data/FigurasContext.cs` — cómo EF Core mapea todo a la base de datos

```csharp
public class FigurasContext : DbContext
{
    public FigurasContext(DbContextOptions<FigurasContext> options) : base(options) { }

    public DbSet<Figura> Figuras { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Single Table Inheritance (TPH)
        modelBuilder.Entity<Figura>()
            .HasDiscriminator<string>("Tipo")
            .HasValue<Circulo>("Circulo")
            .HasValue<Rectangulo>("Rectangulo")
            .HasValue<Triangulo>("Triangulo");

        modelBuilder.Entity<Figura>().Property(f => f.Nombre).IsRequired().HasMaxLength(120);
        base.OnModelCreating(modelBuilder);
    }
}
```

#### Explicación fácil

* `FigurasContext` es la clase que maneja la conexión y le dice a EF Core cómo guardar y leer objetos.
* `DbSet<Figura> Figuras` → representa la tabla que se guarda en la base de datos.
* `HasDiscriminator("Tipo")` → EF guarda todas las subclases (`Circulo`, `Rectangulo`, `Triangulo`) en **una sola tabla** llamada `Figuras`. En esa tabla hay una columna `Tipo` que dice si la fila es `Circulo`, `Rectangulo` o `Triangulo`. Esto se llama **TPH** (table-per-hierarchy).
* `modelBuilder.Entity<Figura>().Property(...).IsRequired().HasMaxLength(120);` → le dice que `Nombre` es obligatorio y tiene máximo 120 caracteres.

---

### 🟦 `Models` — las figuras y sus fórmulas

#### `Figura.cs` (base)

```csharp
public abstract class Figura
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = null!;

    public abstract double CalcularArea();
    public abstract double CalcularPerimetro();

    public virtual void CargarDefaults() { /* para inicializar si hace falta */ }
}
```

* `abstract` → no se puede crear `new Figura()` directamente. Se usa `Circulo`, `Rectangulo` o `Triangulo`.
* `Id` → identificador único (clave primaria).
* `Nombre` → nombre de la figura (ej: "Mi círculo").
* `CalcularArea()` y `CalcularPerimetro()` → cada figura tiene su propia forma de calcularlas.

#### `Circulo.cs`

```csharp
public class Circulo : Figura
{
    public double Radio { get; set; }

    public Circulo() { }

    public Circulo(string nombre, double radio)
    {
        Nombre = nombre;
        Radio = radio;
    }

    public override double CalcularArea() => Math.PI * Radio * Radio;
    public override double CalcularPerimetro() => 2 * Math.PI * Radio;
}
```

* `Radio` → medida del círculo.
* Area = π * r². Perímetro = 2πr.

#### `Rectangulo.cs`

```csharp
public class Rectangulo : Figura
{
    public double Base { get; set; }
    public double Altura { get; set; }

    public Rectangulo() { }

    public Rectangulo(string nombre, double b, double h)
    {
        Nombre = nombre;
        Base = b;
        Altura = h;
    }

    public override double CalcularArea() => Base * Altura;
    public override double CalcularPerimetro() => 2 * (Base + Altura);
}
```

* Area = base * altura. Perimetro = 2*(base + altura).

#### `Triangulo.cs` (principio)

* Tiene `LadoA`, `LadoB`, `LadoC`.
* Area se calcula con **fórmula de Herón**:
  `s = (a + b + c) / 2`
  `area = sqrt(s*(s-a)*(s-b)*(s-c))`
* Antes de calcular el área hay que verificar que los tres lados **pueden formar un triángulo** (desigualdad triangular): cada lado < suma de los otros dos.

---

### 🟦 `DTOs` — qué se manda y qué se recibe

DTO = Data Transfer Object, son las estructuras que usa la API para recibir y mandar datos (separadas de las entidades de la base de datos).

#### `FiguraCreateDto.cs`

```csharp
public class FiguraCreateDto
{
    [Required]
    public string Nombre { get; set; } = null!;
    public double? Radio { get; set; }
    public double? Base { get; set; }
    public double? Altura { get; set; }
    public double? LadoA { get; set; }
    public double? LadoB { get; set; }
    public double? LadoC { get; set; }
}
```

* `double?` significa que puede venir vacío (null) si no aplica para ese tipo.
* Por ejemplo, para crear un círculo mandás `Nombre` y `Radio`; los demás quedan `null`.

#### `FiguraReadDto.cs`

```csharp
public class FiguraReadDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public double Area { get; set; }
    public double Perimetro { get; set; }
}
```

* Esto es lo que devuelve la API cuando pedís una figura: incluye `Area` y `Perimetro` ya calculados.

#### `FiguraUpdateDto.cs`

Muy parecido al `Create`, pero con validaciones para actualizar.

---

### 🟦 `Services/IFiguraService.cs` y `FiguraService.cs` — la lógica central

#### `IFiguraService.cs`

```csharp
public interface IFiguraService
{
    Task<IEnumerable<FiguraReadDto>> GetAllAsync();
    Task<FiguraReadDto?> GetByIdAsync(int id);
    Task<FiguraReadDto> CreateAsync(FiguraCreateDto dto);
    Task<bool> UpdateAsync(int id, FiguraUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<(double area, double perimetro)> TotalesAsync();
}
```

* Define qué operaciones tiene el servicio.

#### `FiguraService.cs` (resumen de comportamiento)

* `GetAllAsync()` → trae todas las figuras de la BD y las transforma en `FiguraReadDto`.
* `GetByIdAsync(int id)` → trae una por id.
* `CreateAsync(FiguraCreateDto dto)` → decide, según los campos del dto, qué clase crear:

  * si `Radio` existe → `Circulo`,
  * si `Base` y `Altura` existen → `Rectangulo`,
  * si `LadoA/B/C` existen → `Triangulo`,
  * si no hay datos suficientes → lanza error.
* `UpdateAsync(int id, FiguraUpdateDto dto)` → busca la figura por id, pregunta qué tipo es (instanceof), y actualiza los campos correspondientes. Devuelve `true` si se actualizó, `false` si no existe.
* `DeleteAsync(int id)` → borra la figura y retorna `true` si existía.
* `TotalesAsync()` → suma áreas y perímetros de todas las figuras.

---

## 5) Cómo probar (ejemplos claros y fáciles)

### Opción A: Usar Swagger (la más simple)

1. Ejecutá `dotnet run`.
2. Abrí `http://localhost:5038/swagger` en el navegador.
3. Encontrarás todas las rutas (`GET`, `POST`, `PUT`, `DELETE`) y un formulario para probar cada una.

   * Para crear una figura: elegí `POST /api/figuras`, hacé click en **Try it out**, pegá el JSON y **Execute**.
   * Para ver la lista: `GET /api/figuras` → Execute.

### Opción B: Usar `curl` (línea de comandos)

> Crear un rectángulo:

```bash
curl -X POST "http://localhost:5038/api/figuras" ^
 -H "Content-Type: application/json" ^
 -d "{\"nombre\":\"MiRect\",\"base\":5,\"altura\":3}"
```

(en Linux/Mac sacás el `^` y usás `\` o todo en una línea)

> Respuesta esperada (ejemplo):

```json
{
  "id": 1,
  "nombre": "MiRect",
  "tipo": "Rectangulo",
  "area": 15.0,
  "perimetro": 16.0
}
```

> Listar figuras:

```bash
curl http://localhost:5038/api/figuras
```

> Obtener por id:

```bash
curl http://localhost:5038/api/figuras/1
```

> Borrar:

```bash
curl -X DELETE http://localhost:5038/api/figuras/1
```

> Totales:

```bash
curl http://localhost:5038/api/figuras/totales
```

---

## 6) Ejemplos prácticos paso a paso (ideal para enseñar en clase)

### Ejemplo 1 — Crear, listar y borrar un rectángulo (paso a paso)

1. Abrís Swagger o usás `curl`.
2. Crear rectángulo (`POST`):

```json
{
  "nombre": "PizarraRect",
  "base": 10,
  "altura": 2
}
```

La API crea la figura y devuelve con `id` (por ejemplo `3`), área y perímetro calculados.
3. Listar (`GET /api/figuras`) → vas a ver la lista con "PizarraRect".
4. Obtener por id (`GET /api/figuras/3`) → ver detalles.
5. Borrar (`DELETE /api/figuras/3`) → ahora `GET` ya no la devuelve.

### Ejemplo 2 — Crear un círculo y ver totales

1. Crear círculo:

```json
{
  "nombre": "MiCirculo",
  "radio": 2
}
```

2. Crear rectángulo:

```json
{
  "nombre": "MiRect",
  "base": 4,
  "altura": 2
}
```

3. Obtener totales (`GET /api/figuras/totales`) → te devuelve la suma de áreas y perímetros de ambas figuras.

---

## 7) Preguntas que te pueden hacer en la presentación (y respuestas listas)

1. **¿Por qué usás DTOs en vez de enviar las entidades directamente?**
   → Para separar la forma en que guardo los datos (entidades) de cómo los expongo por la API. Los DTOs son más seguros y claros para quien consume la API.

2. **¿Qué es Swagger?**
   → Es una interfaz web que muestra los endpoints de la API y te permite probarlos con formularios. Está en `http://localhost:5038/swagger`.

3. **¿Por qué todas las figuras están en la misma tabla (TPH)?**
   → Porque es más simple: la mayoría de campos son compartidos y EF pone una columna `Tipo` para distinguirlas. Si cada figura tuviera campos muy diferentes, podríamos usar otra estrategia.

4. **¿Cómo se calcula el área del triángulo?**
   → Con la fórmula de Herón: `s = (a+b+c)/2`, luego `area = sqrt(s*(s-a)*(s-b)*(s-c))`. También verificamos que los lados formen un triángulo válido.

5. **¿Qué pasa si mando datos incompletos?**
   → El service valida y si los datos no alcanzan lanza un error que el controller transforma en `400 Bad Request` con un mensaje.

6. **¿Por qué SQLite y no otra base?**
   → SQLite es un archivo simple, ideal para ejemplos y pruebas locales. Para apps grandes usaría Postgres o SQL Server.

---

## 8) Ideas para mejorar (si querés seguir laburando)

* Validaciones más estrictas (ej.: comprobar desigualdad triangular antes de crear).
* Manejo de errores centralizado con middleware (para respuestas consistentes).
* Pruebas automáticas (xUnit) con una base de datos en memoria.
* Paginación en `GET /api/figuras` si la tabla crece.
* Seguridad: autenticación y autorización (token JWT).
* Versionado de la API con Swagger (`v1`, `v2`).

---

## 9) Licencia / agradecimientos

Proyecto de ejemplo para la clase. Si lo compartís, mencioná al autor: **Maty Anderegg**.
¡Gracias por usar este README! Si querés, te armo también el **PowerPoint** automático con estas diapositivas y notas del orador.

---

## ⚠️ Notas pedagógicas (cómo presentarlo a chicos de 12 años)

* Usá analogías: la API es una *biblioteca* donde cada libro es una figura.
* Mostrá Swagger en vivo: es gráfico y explica mucho.
* Hacé un demo: crear una figura con un JSON y mostrar la respuesta.
* Preguntas rápidas: ¿qué medida necesita un círculo? (respuesta: radio). ¿Qué es el perímetro? (respuesta: la “banda” alrededor).

---

Si querés, te lo dejo listo como archivo `README.md` para subir al repo (te lo pego ya formateado).
¿Querés también que te genere el **PowerPoint** con estas diapositivas y notas del orador para que lo uses en la presentación?
