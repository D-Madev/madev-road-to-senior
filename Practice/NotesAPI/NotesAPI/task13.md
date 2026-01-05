## 1. ⚙️ ¿Qué es el Middleware? (Q3)

El **Middleware** es un *software* que se inserta en el *pipeline* de la aplicación para manejar peticiones HTTP y respuestas. Cada *middleware* tiene la capacidad de:

1.  **Inspeccionar** la solicitud entrante.
2.  **Modificar** la solicitud (ej. añadir datos de usuario).
3.  **Ejecutar** alguna lógica (ej. *logging* o compresión).
4.  **Cortocircuitar** el *pipeline* (ej. tu `ErrorHandler` o el *middleware* de Autenticación, si la petición es inválida, se detiene allí y devuelve una respuesta).
5.  **Inspeccionar y modificar** la respuesta saliente.

> **Ejemplo del ErrorHandler:** Funciona como un **"Try-Catch" global**. Se coloca al principio del *pipeline* para "envolver" todo el código siguiente. Si algún *middleware* posterior o *endpoint* falla, la ejecución se devuelve a tu *ErrorHandler* para que genere una respuesta `500 Internal Server Error` controlada.

---

## 2. 🛣️ Orden de Ejecución del Pipeline (Q15)

El *pipeline* de *middlewares* en ASP.NET Core funciona como una **cadena de montaje doble (cebolla)**:

1.  **Fase de Solicitud (Inbound):** La solicitud HTTP viaja **hacia adentro** (del exterior al *endpoint*). Los *middlewares* se ejecutan en el orden en que fueron definidos.
2.  **Fase de Respuesta (Outbound):** La respuesta viaja **hacia afuera** (del *endpoint* al exterior). Los *middlewares* se ejecutan en el **orden inverso** a como fueron definidos.

### La Regla de Seguridad (CORS/Auth)

El orden es vital para la seguridad y la funcionalidad:

* **`UseCors()`** debe ir antes de `UseRouting()` y `UseMapControllers()`. Si la petición es rechazada por CORS, no tiene sentido desperdiciar recursos buscando una ruta o intentando autenticar.
* **`UseAuthentication()`** debe ir antes de `UseAuthorization()` y `UseMapControllers()`. Primero debemos identificar **quién** es el usuario (Authentication), y luego verificar **si tiene permiso** para la ruta solicitada (Authorization).


---

## 3. 🧩 Roles en `Program.cs` (Q17)

### A. IServiceCollection (`builder.Services`)

* **Rol:** **Contenedor de Dependencias (DI).**
* **Qué define:** El **qué**. Define todas las clases, servicios, *middlewares* y configuraciones que la aplicación **necesita** y **cómo deben crearse** (ej. *lifetime* `Scoped`, `Transient`, `Singleton`).

### B. IApplicationBuilder (`app`)

* **Rol:** **Constructor del Pipeline.**
* **Qué define:** El **cómo**. Define la **secuencia** en la que los *middlewares* registrados en `IServiceCollection` serán invocados para manejar una petición. Cada llamada a `app.Use...` añade un nuevo componente al *pipeline*.

Esta distinción es crítica porque demuestra que entiendes la **Separación de Intereses (SoC)** dentro del *startup* de una aplicación .NET Core.