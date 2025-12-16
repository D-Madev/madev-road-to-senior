# 🚀 The Mini-Project: Note/To-Do API

Este proyecto es el "Road-to-Senior" práctico para la arquitectura **ASP.NET Core**. El objetivo principal es construir una API simple de notas/tareas (`CRUD`) y, de manera incremental, refactorizarla para aplicar conceptos de nivel intermedio y avanzado.

* **Project Name Idea:** `NotesAPI`
* **Goals:** 
    - ✅ Crear una API que solo sepa **Crear, Leer, Actualizar y Borrar** notas (`POST /notes`, `GET /notes/{id}`, etc.).
    - $\square$ Convertirlo en un Microservicio listo para producción.

---

## ⏸️ Plan de Acción para Pausas

1.  **Stop:** Siempre detente después de completar una tarea (`$\square$`).
2.  **Commit:** Haz un **commit** en tu repositorio de Git con el paso que terminaste.
3.  **Update:** Actualiza la tabla **📌 Estado Actual** en este README indicando la próxima tarea.

---

## 📋 Plan de Acción Incremental (Roadmap)

El proyecto está dividido en tres fases que cubren los principales temas de las entrevistas .NET Core.

| Fase | Título | Preguntas Clave Cubiertas | Tarea Principal |
| :--- | :--- | :--- | :--- |
| **Fase 1.5** | **The Foundation** | Q7, Q9, Q19 (DI) | **Setup Inicial y Routing** |
| **Fase 2** | **The Mechanics** | Q12, Q14, Q10 | **Data, Asincronía, y Servicios** (El Core de la App) |
| **Fase 3** | **The Architecture** | Q18, Q20, Q23, Q16 | **Estabilidad, Monitoreo y Escalabilidad** (Senior) |
| **Fase 4** | **API Surface & Flow** | Q3, Q11, Q15, Q17 | CORS, Pipeline, y Contextos |
| **Fase 5** | **Quality Assurance** | N/A (Fundamental Senior) | Unit & Integration Testing |
| **Fase 6** | **Security** | N/A (Fundamental Senior) | JWT Authentication |
| **Fase 7** | **Production Readiness** | N/A (DevOps Senior) | Metrics (Prometheus) y Cloud Deployment |

---

## 📌 Estado Actual y Próximo Paso (Quick Start)

Esta tabla indica dónde pausaste y qué necesitas hacer a continuación. **ACTUALÍZALA al inicio de cada sesión de trabajo.**

| Estado | Tarea Actual | Concepto Clave a Repasar |
| :--- | :--- | :--- |
| **✅** | 1. Crear el `Note` Model | **Estructura básica** y modelos de datos. |
| **✅** | 2. Crear el `NotesController` | **Q9: Routing** (Rutas de la API). |
| **✅** | 3. Implementar el `Repository` y uso de DI | **Q19/Q4: Lifetimes** (usa `AddSingleton` para la lista fija). |
| **✅** | **Finalizamos etapa 1.5** | **Q7, Q9, Q19 (DI)**	Setup Inicial y Routing |
| **✅** | 4. Instalar Entity Framework Core (EF Core) y configurar el DbContext. | **Q12:** EF Core (Configuración de Base de Datos). |
| **✅** | 5. Implementar async y await en el método Get() y crear datos iniciales. | **Q14** (Programación Asíncrona) |
| **✅** | 6. Implementar async y await en el método Post(). | **Q14** (Programación Asíncrona) |
| **✅** | **Finalizamos etapa 2** | **Q12, Q14, Q10** |
| **✅** | 7. Concretizacion final de la ABM de la API. | Completamos todos los metodos de la API. |
| **✅** | 8. Implementar un Global Exception Handler en el Middleware Pipeline. | **Q23:** Global Exception Handling (Middleware). |
| **✅** | 9. Configurar Health Checks para el servicio, verificando el estado de la base de datos (NotesDbContext). | **Q20:** Health Checks (Monitoreo). |
| **✅** | 10. Configurar una librería de Logging (ej: Serilog). | **Q18:** Centralized Logging (Observabilidad). |
| **✅** | 11. Refactorizar la lógica a un Service Layer. | **Q16:** Service Layer (Separación de Intereses). |
| **✅** | **Finalizamos etapa 3** |
| **✅** | 12. Configurar CORS (Q11) y revisión del Pipeline. | Q11, Q15, Q17: Middleware Order, CORS. |
| **✅** | 13. Revisión del Pipeline: Discusión sobre el orden y rol de los middlewares: IApplicationBuilder vs IServiceCollection.  | Q3, Q15, Q17: Middleware, Pipeline Order, DI Lifetimes. |
| **✅** | **Finalizacion etapa 4**	| API Surface & Flow |
| **✅** | 14. Setup de Proyectos de Test: Crea NotesAPI.Tests (tipo xUnit) y añade dependencias (Microsoft.NET.Test.Sdk, Moq). | Testing Project Setup. |
| **✅** | 15. Unit Testing (Controller): Escribe tests para el NotesController usando Moq para simular (mockear) la interfaz INotesService con respuestas controladas. | FIRST Principle, Mocking. |
| **✅** | 16. Integration Testing (Service Layer): Escribe tests que usen WebApplicationFactory para probar el flujo completo: Controller -> Service -> DB real (InMemory/SQLite). | Integration Testing, WebApplicationFactory. |
| **✅** | 17. Integration Testing (E2E API): Tests con WebApplicationFactory para el flujo HTTP completo.	| WebApplicationFactory, E2E Testing. |
| **✅** | **Finalizacion etapa 5**	| Quality Assurance Testing |
| **✅** | 18. Configurar Secret Manager (Desarrollo Seguro) | Seguridad de claves. |
| **✅** | 19. JWT Authentication Setup: Configura la autenticación JWT en Program.cs y añade un dummy endpoint de login que genere un token. | JWT Authentication. |
| **✅** | 20. Authorization: Protege el endpoint POST /notes con el atributo [Authorize] para asegurar que solo usuarios autenticados puedan crear notas. | API Authorization. |
| **✅**	| **Finalizacion etapa 6**	| Security Authentication & Authorization |
| **IN PROGRESS**	| 21. Documentación (Swagger/OpenAPI): Instala Swashbuckle y configura la generación automática de documentación. | API Documentation, Q24. |
| $\square$	| 22. Metrics (Prometheus): Integra una librería para exponer métricas básicas (/metrics). | API Metrics, Q20 (Monitoreo). |
| $\square$ | 23. Performance/Load Testing: Diseña y ejecuta un test de carga (ej. con k6 o JMeter).	| Testing No Funcional, Optimización. |
| $\square$ | 24. Security Testing (DAST): Ejecuta un escaneo básico de vulnerabilidades (ej. con OWASP ZAP) en la API. | Vulnerability Scanning, OWASP Top 10. |
| $\square$ | 25. Chaos Testing: Simula fallos de la base de datos o latencia para probar la resiliencia del Global Exception Handler. | Resiliencia, Pruebas de Caos. |
| $\square$ | 26. Despliegue en Azure/AWS: Prepara la API como un Contenedor Docker y despliégala en un servicio de Cloud. | Cloud Infrastructure, DevOps. |
| $\square$	| **Finalizacion etapa 7**	| Production Readiness Production Readiness Observabilidad & Cloud |

---
