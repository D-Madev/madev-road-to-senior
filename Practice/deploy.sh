#!/bin/bash

echo "🚀 Iniciando despliegue en Kubernetes..."

# 1. Construir la imagen de la API (Docker)
echo "📦 Buildeando imagen de la API..."
docker build -t notes-api:latest .

# 2. Aplicar los manifiestos de Kubernetes
echo "☸️ Aplicando configuraciones de K8s..."
kubectl apply -f k8s/

# 3. Forzar el reinicio (Por si la imagen ya existía y K8s no se dio cuenta)
echo "♻️ Reiniciando pods para aplicar cambios..."
kubectl rollout restart deployment notes-api-deployment

# 4. Ver el estado final
echo "✅ Estado de los pods:"
kubectl get pods

echo "🌐 API disponible en: http://localhost/swagger"