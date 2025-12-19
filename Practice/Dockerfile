# ETAPA 1: Compilacion (build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos el archivo de proyecto y restauramos las dependencias
COPY ["NotesAPI/NotesAPI.csproj", "NotesAPI/"]
RUN dotnet restore "NotesAPI/NotesAPI.csproj"

# Copiamos todo lo demas y compilamos
COPY . .
WORKDIR "/src/NotesAPI"
RUN dotnet build "NotesAPI.csproj" -c Release -o /app/build/

# ETAPA 2: publicación (publish)
FROM build AS publish
RUN dotnet publish "NotesAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ETAPA 3: Runtime (Imagen final súper ligera)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Puerto por defecto en imágenes modernas de .NET
EXPOSE 8080
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "NotesAPI.dll"]


# --- Comandos utiles --- 
# CREAR IMAGEN: docker build -t nombre-de-la-imagen:tag .
# RUN CONTENEDOR: docker run -d -p puerto-contenedor:puerto-host --name nombre-contenedor -e Jwt__Key="ClaveTokenSecreta" -e ASPNETCORE_ENVIRONMENT=Development nombre-de-la-imagen:tag
# BORRAR CONTENEDOR: docker rm -f nombre-contenedor
# BORRAR IMAGEN: docker rmi nombre-de-la-imagen:tag
# LISTAR CONTENEDORES: docker ps
# LISTAR IMAGENES: docker images
# LISTAR LOGS: docker logs nombre-contenedor
