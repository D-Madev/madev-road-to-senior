## 🔑 Tarea 18: Configurar Secret Manager (Desarrollo Seguro)

### Paso 1: Inicializar Secret Manager

Abre tu terminal en la carpeta del proyecto `NotesAPI` y ejecuta:

```bash
dotnet user-secrets init
```

Esto añade un `<UserSecretsId>` al archivo `.csproj` (invisible para ti) que asocia tu proyecto con un archivo de secretos local en tu perfil de usuario.

### Paso 2: Guardar la Clave Secreta

Ahora, usa la herramienta de línea de comandos para guardar la clave. Usaremos la clave `Jwt:Key` para seguir el estándar de configuración por secciones.

```bash
// ¡Feliz Navidad para esta API segura! -(base 64)-> wqFGZWxpeiBOYXZpZGFkIHBhcmEgZXN0YSBBUEkgc2VndXJhIQ==

dotnet user-secrets set "Jwt:Key" "wqFGZWxpeiBOYXZpZGFkIHBhcmEgZXN0YSBBUEkgc2VndXJhIQ=="
```

---

## 🆚 `.env` vs. `Secret Manager` / Variables Nativas

| Característica | Archivo `.env` | Secret Manager / Variables de Entorno Nativas de .NET |
| :--- | :--- | :--- |
| **Manejo de Secretos en Dev** | Necesita una librería externa (ej. `DotNetEnv`) para ser cargado. | **Nativo.** El `builder` de ASP.NET Core carga `secrets.json` automáticamente si el entorno es `Development`. |
| **Manejo de Secretos en Prod** | Se requiere que el archivo `.env` se pase al contenedor Docker o al servidor (lo cual no es ideal). | **Nativo.** ASP.NET Core lee las variables de entorno del sistema operativo (Docker, K8s, Azure, etc.) sin librerías externas. |
| **Formato de Clave** | `CLAVE_SECRETA="valor"` | `Jwt:Key` (usa `:` para secciones jerárquicas). |
| **Seguridad / Git Ignore** | Debes recordar añadir el archivo `.env` al `.gitignore`. | El archivo `secrets.json` nunca se crea dentro de la carpeta del proyecto y se ignora por defecto. **Más seguro por accidente.** |

**Secret Manager (`secrets.json`):** Para manejar los secretos de forma segura en **Desarrollo**.

-----

## 🐳 Comandos de Docker Útiles

| Acción | Comando |
| :--- | :--- |
| **Levantar y Construir** | `docker-compose up -d --build` |
| **Detener Servicios** | `docker-compose down` |
| **Levantar Servicios** | `docker-compose up -d` |
| **Crear Imagen** | `docker build -t nombre-de-la-imagen:tag .` |
| **Ejecutar Contenedor** | `docker run -d -p 8080:80 --name nombre-contenedor -e Jwt__Key="ClaveTokenSecreta" -e ASPNETCORE_ENVIRONMENT=Development nombre-de-la-imagen:tag` |
| **Borrar Contenedor** | `docker rm -f nombre-contenedor` |
| **Borrar Imagen** | `docker rmi nombre-de-la-imagen:tag` |
| **Listar Contenedores** | `docker ps` |
| **Listar Imágenes** | `docker images` |
| **Ver Logs** | `docker logs nombre-contenedor` |