# 📌 Comandos del CLI de .NET

## 🔹 Comandos generales

- `dotnet --info`  
  Muestra información del SDK/Runtime instalados y del entorno.  

- `dotnet --version`  
  Muestra la versión del SDK que usa `dotnet`.  

- `dotnet --list-runtimes`  
  Lista los runtimes instalados.  

- `dotnet --list-sdks`  
  Lista los SDKs instalados.  

- `dotnet -h | --help`  
  Muestra ayuda general o de un comando (`dotnet <comando> --help`).  

- `dotnet build`  
  Compila el proyecto y sus dependencias (genera binarios).  

- `dotnet build-server`  
  Interactúa con servidores iniciados por procesos de build (caching, servidores de compilación).  

- `dotnet clean`  
  Limpia los outputs de build (elimina `bin/` y `obj/`).  

- `dotnet exec <dll>`  
  Ejecuta un `.dll` de .NET directamente.  

- `dotnet help`  
  Abre la referencia/ayuda para un comando específico.  

- `dotnet migrate` *(legacy)*  
  Migra proyectos de versiones antiguas a formato SDK.  

- `dotnet msbuild`  
  Acceso directo a MSBuild (opciones avanzadas de compilación).  

- `dotnet new <template>`  
  Crea un nuevo proyecto/archivo desde plantillas (`console`, `webapi`, `classlib`, `sln`, etc).  

- `dotnet pack`  
  Empaqueta el proyecto como paquete NuGet (`.nupkg`).  

- `dotnet publish`  
  Compila y publica el proyecto listo para despliegue.  
  Ej: `dotnet publish -c Release`  

- `dotnet restore`  
  Restaura paquetes NuGet declarados en el proyecto.  

- `dotnet run`  
  Compila y ejecuta la aplicación desde el código fuente.  

- `dotnet sdk check`  
  Verifica si SDKs/Runtimes instalados están actualizados.  

- `dotnet sln`  
  Operaciones sobre archivos `.sln` (agregar, quitar, listar proyectos).  

- `dotnet store` *(menos común hoy)*  
  Almacena ensamblados en el runtime package store.  

- `dotnet test`  
  Ejecuta tests unitarios (compila y ejecuta test runners).  

---

## 🔹 Comandos para referencias de proyecto

- `dotnet reference add`  
  Agrega una referencia de proyecto (`<ProjectReference>`).  

- `dotnet reference list`  
  Lista las referencias de proyecto.  

- `dotnet reference remove`  
  Quita una referencia de proyecto.  

---

## 🔹 Comandos para paquetes NuGet (simples)

- `dotnet package add`  
  Agrega un paquete NuGet al proyecto.  

- `dotnet package list`  
  Lista paquetes NuGet referenciados en el proyecto.  

- `dotnet package remove`  
  Quita un paquete NuGet del proyecto.  

- `dotnet package search`  
  Busca paquetes NuGet.  

---

## 🔹 Comandos `dotnet nuget` (fuentes y publicación)

- `dotnet nuget delete`  
  Borra o despublica un paquete del servidor.  

- `dotnet nuget push`  
  Publica un paquete en un feed (ej: NuGet.org).  

- `dotnet nuget locals`  
  Limpia/lista caches locales de NuGet (`http-cache`, `temp`, `global-packages`).  

- `dotnet nuget add source` / `disable source` / `enable source` / `list source` / `remove source` / `update source`  
  Gestiona las fuentes de NuGet configuradas en la máquina.  

---

## 🔹 Workloads (SDK workload)

- `dotnet workload install <workload>`  
  Instala workloads opcionales (ej: MAUI, wasm tools).  

- `dotnet workload list`  
  Lista workloads instaladas.  

- `dotnet workload repair`  
  Repara las workloads instaladas.  

- `dotnet workload search`  
  Busca workloads disponibles.  

- `dotnet workload uninstall <workload>`  
  Desinstala una workload.  

- `dotnet workload update`  
  Reinstala/actualiza todas las workloads.  

---

## 🔹 Herramientas (`dotnet tool`)

- `dotnet tool install`  
  Instala una herramienta (global/local) desde NuGet.  
  Ej: `dotnet tool install -g dotnet-ef`  

- `dotnet tool list`  
  Lista las herramientas instaladas (global, local o por ruta).  

- `dotnet tool search`  
  Busca herramientas en NuGet.org.  

- `dotnet tool uninstall`  
  Desinstala una herramienta.  

- `dotnet tool update`  
  Actualiza una herramienta instalada.  

---

## 🔹 Herramientas adicionales incluidas en el SDK

- `dev-certs`  
  Administra certificados de desarrollo (HTTPS).  

- `ef`  
  Entity Framework Core CLI (`dotnet ef` → migrations, db update, scaffold).  

- `user-secrets`  
  Maneja el Secret Manager para ASP.NET Core.  

- `watch`  
  Observador de archivos que reinicia o aplica *hot reload* (`dotnet watch run`).  

---

## 🔹 Comandos para usar con Entity Framework

- `dotnet ef migrations add <name-of-migration>`
  Creamos una migracion con el nombre name-of-migration.

- `dotnet ef migrations add <name-of-migration> -c <name-of-context> -s <name-of-dir-where-startup-is>`
  Es importante destacar que estos comandos se tienen que ejecutar en la base del proyecto que contiene las migraciones habitualmente este proyecto es el DAL -> La capa de acceso a datos. 
  Pero tambien hay que destacar que nos puede dar un error si tenemos mas de un contexto de la base de datos y si el archivo startup no está en el mismo directorio que las migraciones. 
  Para tales situaciones se puede adaptar el comando anterior de la manera detallada arriba.
  La -c tambien se puede usar como --context
  La -s hace referencia al archivo startup

---

## 🔹 Ejemplos de uso

- `dotnet ef migrations add <name-of-migration>`  
  Para poder agregar migraciones.

- `dotnet ef database update`  
  Ejecuta las migraciones.

- `dotnet new list`  
  Te permite ver las distintas opciones de proyecto que existen para crear.