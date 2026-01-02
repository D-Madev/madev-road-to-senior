#!/bin/bash
echo "🛑 Iniciando limpieza total del entorno..."

# 1. Borrar recursos de Kubernetes (esto borra los Pods y Services)
kubectl delete -f k8s/ --ignore-not-found=true

# 2. Limpiar volúmenes (IMPORTANTE: Esto borra tus notas de la DB)
echo "💾 Eliminando datos persistentes..."
kubectl delete pvc --all --ignore-not-found=true

# 3. Limpiar Docker Compose (Redis/DB)
docker-compose down -v --remove-orphans

# 4. Borrar imágenes (Solo si existen)
echo "🖼️  Limpiando imágenes de la API..."
IMAGES=$(docker images 'notes-api' -q)
if [ -n "$IMAGES" ]; then
    docker rmi $IMAGES --force
else
    echo "No hay imágenes para borrar."
fi

echo "✅ Sistema limpio."