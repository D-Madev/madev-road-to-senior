# 🚀 The Mini-Project: Note/To-Do API

Este proyecto es el "Road-to-Senior" práctico para la arquitectura **ASP.NET Core**. El objetivo principal es construir una API simple de notas/tareas (`CRUD`) y, de manera incremental, refactorizarla para aplicar conceptos de nivel intermedio y avanzado.

* **Project Name Idea:** `NotesAPI`
* **Goal:** Crear una API que solo sepa **Crear, Leer, Actualizar y Borrar** notas (`POST /notes`, `GET /notes/{id}`, etc.).

---

## 📌 Estado Actual y Próximo Paso (Quick Start)

Esta tabla indica dónde pausaste y qué necesitas hacer a continuación. **ACTUALÍZALA al inicio de cada sesión de trabajo.**

| Estado | Última Tarea Completada | Tarea Actual | Concepto Clave a Repasar |
| :--- | :--- | :--- | :--- |
| **✅** | N/A (Inicio de Proyecto) | 1. Crear el `Note` Model | **Estructura básica** y modelos de datos. |
| **✅** | 1. Crear el `Note` Model | 2. Crear el `NotesController` | **Q9: Routing** (Rutas de la API). |
| **✅** | 2. Crear el `NotesController` | 3. Implementar el `Repository` y uso de DI | **Q19/Q4: Lifetimes** (usa `AddSingleton` para la lista fija). |
| **✅** | 3. Implementar el `Repository` | **Finalizamos etapa 1.5** | **Q7, Q9, Q19 (DI)**	Setup Inicial y Routing |
| **✅** | **Finalizamos etapa 1.5** | 4. Instalar Entity Framework Core (EF Core) y configurar el DbContext. | **Q12:** EF Core (Configuración de Base de Datos). |
| **✅** | 4. Instalar Entity Framework Core (EF Core) y configurar el DbContext. | 5. Implementar async y await en el método Get() y crear datos iniciales. | **Q14** (Programación Asíncrona) |
| **✅** | 5. Implementar async y await en el método Get() y crear datos iniciales. | 6. Implementar async y await en el método Post(). | **Q14** (Programación Asíncrona) |
| **✅** | 6. Implementar async y await en el método Post(). | **Finalizamos etapa 2** | **Q12, Q14, Q10** |
| **✅** | **Finalizamos etapa 2** | 7. Implementar un Global Exception Handler. | **Q23:** Global Exception Handling (Middleware). |
---

## 📋 Plan de Acción Incremental (Roadmap)

El proyecto está dividido en tres fases que cubren los principales temas de las entrevistas .NET Core.

| Fase | Título | Preguntas Clave Cubiertas | Tarea Principal |
| :--- | :--- | :--- | :--- |
| **Fase 1.5** | **The Foundation** | Q7, Q9, Q19 (DI) | **Setup Inicial y Routing** |
| **Fase 2** | **The Mechanics** | Q12, Q14, Q10 | **Data, Asincronía, y Servicios** (El Core de la App) |
| **Fase 3** | **The Architecture** | Q18, Q20, Q23, Q16 | **Estabilidad, Monitoreo y Escalabilidad** (Senior) |

---

## 🎯 To-Do List Detallada

Empieza creando una nueva solución **ASP.NET Core Web API**.

### I. Configuración y Primer Endpoint (Fase 1.5)

| ✅ | Tarea (Task) | Concepto Cubierto | Estado |
| :--- | :--- | :--- | :--- |
| **✅** | 1. Crear un `Note` **simple model** (solo `Id` y `Title`). | Estructura básica de la aplicación. | |
| **✅** | 2. Crear un `NotesController` **simple** y un método `Get()` que devuelva una lista **fija** (hardcoded). | **Q9: Routing** (Rutas de la API). | |
| **✅** | 3. **(Opcional pero recomendado)**: Usa **Dependency Injection** (**DI**) para inyectar la lista fija en el `Controller` (ej: `List<Note>`). | **Q19/Q4: Lifetimes** (usa `AddSingleton` para la lista fija). | |

### II. Data y Asincronía (Fase 2: The Mechanics)

| ✅ | Tarea (Task) | Concepto Cubierto | Estado |
| :--- | :--- | :--- | :--- |
| **✅** | 4. Instalar **Entity Framework Core** (EF Core) y configurar el `DbContext` con una base de datos local (ej: **SQLite** o **InMemory**). | **Q12: EF Core** (Configuración de Base de Datos). | |
| **✅** | 5. Registrar tu `DbContext` en `Program.cs` / `Startup.cs` usando el **Lifetime** `services.AddScoped`. | **Q19: Scoped Lifetime** (Fundamental para DB Context). | |
| **✅** | 6. Migrar el método `Get()` del `Controller` para usar **EF Core** y obtener las notas. | **Q14: Asynchronous Programming** (Empieza a usar `ToListAsync()`). | |
| **✅** | 7. Crea el método `POST` (Crear Nota) asegurándote de usar **Async** (`CreateAsync`, `SaveChangesAsync`). | **Q14: Async** (Mejora la **Performance**). | |

### III. Arquitectura y Estabilidad (Fase 3: Advanced)

| $\square$ | Tarea (Task) | Concepto Cubierto | Estado |
| :--- | :--- | :--- | :--- |
| $\square$ | 8. Implementar un **Global Exception Handler** para manejar errores como 404 (Not Found). | **Q23: Global Exception Handling** (Middleware). | |
| $\square$ | 9. Configurar **Health Checks** para el servicio. La *health check* debe verificar el estado de la base de datos. | **Q20: Health Checks** (Monitoreo). | |
| $\square$ | 10. Configurar una librería de **Logging** (ej: Serilog) para enviar los logs a la consola o a un archivo. | **Q18: Centralized Logging** (Observabilidad). | |
| $\square$ | 11. **(Microservices/Desarrollo)**: Refactorizar la lógica de negocio a un **separado Service Layer** (fuera del Controller). | **Q16: Microservices** (Prepara la arquitectura). | |

---

### ⏸️ Plan de Acción para Pausas

1.  **Stop:** Siempre detente después de completar una tarea (`$\square$`).
2.  **Commit:** Haz un **commit** en tu repositorio de Git con el paso que terminaste.
3.  **Update:** Actualiza la tabla **📌 Estado Actual** en este README indicando la próxima tarea.