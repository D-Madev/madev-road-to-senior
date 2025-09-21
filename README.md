# Madev - Road to Senior - .NET & C#

![Road to Senior](https://img.shields.io/badge/Status-InProgress-orange)
![Language](https://img.shields.io/badge/Language-C%23-8A2BE2)
![Framework](https://img.shields.io/badge/Framework-.NET%208-8A2BE2)
![License](https://img.shields.io/badge/License-MIT-lightgrey)
![Build Status](https://img.shields.io/github/actions/workflow/status/D-Madev/madev-road-to-senior/ci-main.yml?branch=main)
![Coverage](https://img.shields.io/badge/Coverage-0%25-red) 
![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen)

## Descripción

Este repositorio contiene mi progreso en **cursos de C# y .NET**, así como ejercicios, prácticas y recursos adicionales que voy generando para avanzar hacia un perfil **Senior .NET Developer**.  
Aquí encontrarás desde CRUDs básicos hasta la aplicación de **patrones de diseño, buenas prácticas, y estrategias de branching con Git**.

---

## Estructura de carpetas

```yaml
road-to-senior/
│
├─ CourseProjects/   # Proyectos realizados como parte del curso
│ ├─ CRUD_Test/      # Proyecto CRUD de ejemplo
│ └─ BlogCore/       # Proyecto de blog con EF Core y MVC
│
├─ Practice/         # Ejercicios y experimentos propios
│ ├─ Patterns/       # Implementaciones de patrones (Repository, UnitOfWork, Strategy, etc.)
│ ├─ LINQ-Samples/   # Ejemplos de LINQ y consultas complejas
│ └─ Async-Demos/    # Ejercicios con async/await y pitfalls
│
├─ Docs/             # Documentación general
│ ├─ Cheatsheets/    # Diagramas, cheatsheets y guías rápidas
│ └─ Notes/          # Markdown de seguimiento de aprendizaje diario
│
├─ GitFlow/          # Ejemplos de branching, pull requests y flujos de trabajo
│
├─ LICENSE           # Archivos de licencia
├─ .gitignore        # Archivos/carpetas ignoradas por Git
└─ README.md         # Este archivo

```

---

## Badges y Links Útiles

- **Build Status:** Integración continua con GitHub Actions (dotnet.yml).  
- **Coverage:** Cobertura de pruebas unitarias (pendiente de implementar).  
- **Cheatsheets:** Documentos de patrones, EF Core, async/await, DTO vs Entity, DI Lifetimes.  


---

## Convenciones

- **Commits:** Se siguen los [Conventional Commits](https://www.conventionalcommits.org/)  
  Ejemplos:  

> ```yaml
> feat(login): agregar autenticación con JWT
> fix(crud): corregir eliminación de usuarios
> chore(gitignore): actualizar carpetas ignoradas
> ```


- **Branching:**  
- `main` / `prod`: siempre estable, listo para producción.  
- `dev`: integración de features en curso.  
- `feature/<nombre>`: nuevas funcionalidades temporales.  
- **Pull Requests:** Simular flujo profesional aunque sea autoaprobado. 
---

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- Visual Studio 2022 / VS Code  
- SQL Server / SQLite para pruebas de EF Core (dependiendo del proyecto)  

---

## Buenas prácticas

- Evitar exponer entidades EF Core directamente a la vista; usar DTOs.  
- Mantener **thin controllers** y lógica en servicios o repositorios.  
- Seguir patrones de diseño cuando sea posible (`Repository`, `UnitOfWork`, `Strategy`, `CQRS`).  
- Revisar commits antes de hacer PRs, mantener mensajes claros.  

---

## Cómo contribuir

Este es un repositorio **personal de seguimiento**, pero puedes:  
1. Clonar el repositorio.
2. Crear tus propias ramas de práctica (`feature/<nombre>`).
3. Hacer PRs a `dev` aunque sean autoaprobados para simular flujo de trabajo real.
4. Mantener tu progreso documentado en `Docs/Notes`.

---

## Licencia

MIT License © Matias Seba Mallo (D-Madev)
