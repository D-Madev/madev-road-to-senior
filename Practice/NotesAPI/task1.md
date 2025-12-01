# Explicación del flujo de la aplicación

Se ha implementado el patrón de diseño **Repository** junto con **Dependency Inversion** (a través de la interfaz `INotesRepository`) y **Dependency Injection** (a través de `Program.cs`).

## 🚦 Application Flow (Flujo de la Aplicación)

El flujo se divide en dos momentos principales: **Start-up** (inicio de la aplicación) y **Request** (cada vez que un usuario hace una petición).

### 1. ⚙️ Start-Up (El Inicio: Program.cs)

This happens **only once** when you press the "Run" button.

1.  **Read the Settings:** `Program.cs` reads all the files (`appsettings.json`, environment variables).
2.  **Define Services (DI):** The line `builder.Services.AddSingleton<INotesRepository, StaticNotesRepository>();` is executed.
    * **What it means:** You tell the system: "When someone asks for the **contract** (`INotesRepository`), always give them the **concrete class** (`StaticNotesRepository`). Keep only **one copy** (`Singleton`) of this class for the entire application."
3.  **Build the Host:** The system builds the server (**Kestrel**) and the **Middleware Pipeline** (the `Configure` steps).



---

### 2. ⚡ Request Flow (Cada Petición HTTP)

This happens **every time** a user opens the URL `/Notes`.

1.  **Kestrel Receives:** The **Web Server (Kestrel)** receives the HTTP Request (GET `/Notes`).
2.  **Routing (The Map):** The **Routing Middleware** looks at the URL and compares it with your Controller's attributes (`[Route("[controller]")]`, `[HttpGet]`).
    * **Decision:** "Ah, this request needs to go to the `Get()` method inside the `NotesController` class."
3.  **Controller Creation:** The system sees: `public NotesController(INotesRepository repository)`.
    * The system says: "I need an instance of `NotesController`, but first, I need an `INotesRepository`."
4.  **Dependency Injection (DI):** The system goes to the list of services defined in `Program.cs` (Step 1.2).
    * It finds the **Singleton** instance of `StaticNotesRepository` and **injects** (passes) it into the Controller's constructor.
5.  **Execution:** The `Get()` method runs:
    * `return Ok(_repository.GetAll());`
    * It calls the `GetAll()` method on the injected `StaticNotesRepository`.
    * The Repository returns the fixed list of three notes.
6.  **Response:** The Controller wraps the list inside an HTTP **200 OK** response, and **Kestrel** sends the final data back to the user's browser.

---

## ✅ Summary of Best Practices

| Component | Role | Best Practice Applied |
| :--- | :--- | :--- |
| **Model** (`Note`) | The data structure. | Clean POCO (Plain Old C# Object). |
| **Interface** (`INotesRepository`) | The **Contract** (The Rule). | **Dependency Inversion Principle**. The Controller depends on the **Rule**, not the concrete implementation. |
| **Repository** (`StaticNotesRepository`) | The **Implementation** (The Worker). | Hides the data logic (where the notes come from: memory, file, or DB). |
| **Controller** | The **Entry Point** (The Manager). | Uses **DI** (asks for the Repository) and focuses only on HTTP logic. |

---
<br>

# Estándar **RESTful** que debes seguir:

## 🛣️ Convención de Routing (RESTful Standard)

El `NotesController` ya tiene la ruta base: `[Route("Notes")]`.

| Acción (Método) | Verbo HTTP | ¿Necesita `[Route]`? | URL de Acceso | Concepto |
| :--- | :--- | :--- | :--- | :--- |
| `Get()` (Traer todo) | `[HttpGet]` | **No** (Hereda la base) | `host/Notes` | Trae la colección completa. |
| `Get(int id)` (Traer uno) | `[HttpGet]` | **Sí** (Especifica el parámetro) | `host/Notes/5` | Trae un recurso específico. |
| `Post()` (Crear) | `[HttpPost]` | **No** (Hereda la base) | `host/Notes` | Crea un nuevo recurso. |
| `Put(int id)` (Actualizar) | `[HttpPut]` | **Sí** (Especifica el parámetro) | `host/Notes/5` | Actualiza un recurso existente. |

### 🎯 Cuándo Usar `[Route]` en el Método

Solo usas el atributo `[Route]` en un método si necesitas especificar **parámetros** o si la ruta **NO** es la base del Controller.

#### 1\. Para Parámetros (El Patrón más Común)

Para el método que trae una sola nota por su ID (lo que harías en una **Tarea 2.5**):

```csharp
[HttpGet("{id}")] // Esto añade /{id} a la ruta base /Notes
public IActionResult Get(int id)
{
    // ...
}
// URL Final: host/Notes/{id}
```

#### 2\. Para Acciones Específicas

Si tuvieras un método que no encaja en el CRUD estándar (ej: "Contar notas"):

```csharp
[HttpGet("count")] // Esto añade /count a la ruta base /Notes
public IActionResult Count()
{
    // ...
}
// URL Final: host/Notes/count
```

**Resumen:** Deja el `[Route]` principal en la clase y solo especifica el **fragmento de ruta** que va después de la base en los métodos. ¡Así tu código es más limpio y sigue el estándar REST\!

----

## 🎯 El Gran Concepto: Acoplamiento Débil

**Acoplamiento Débil** (*Loose Coupling*), es el objetivo principal de este tipo de arquitectura.

1.  **Patrón Repository + Interfaz (`INotesRepository`)**
    * Este es el **Contrato** (la regla). Tu *Controller* acepta este contrato en su constructor.
    * **Principio Clave:** **Principio de Inversión de Dependencias (DIP)**. Tu *Controller* depende de la **abstracción** (la Interfaz), no de la **implementación** (la Clase real).

2.  **Inyección de Dependencias (DI) en `Program.cs`**
    * Aquí es donde tú decides qué **implementación concreta** (la clase real) quieres usar para cumplir el contrato.
    * Si estás en desarrollo, usas: `builder.Services.AddSingleton<INotesRepository, DevNotesRepository>();` (Una lista *hardcodeada*).
    * Si estás en producción, usas: `builder.Services.AddScoped<INotesRepository, ProductionDatabaseRepository>();` (Una conexión a SQL Server).

3.  **El Resultado: Intercambiabilidad Total**
    * Como dijiste, el código de tu *Controller* (`Get()`) no necesita saber si está hablando con una lista en memoria o una base de datos.
    * Tu *Controller* simplemente confía en que `_repository.GetAll()` le devolverá la data.
    * Solo cambias **una línea** en `Program.cs`, y la fuente de datos de la aplicación cambia por completo.


¡Con esto hemos completado el objetivo de entender la **Fase 1.5**! Este es un conocimiento crucial de nivel Intermedio/Senior.