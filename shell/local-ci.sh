#!/bin/sh
set -e
TAG=$(date +"%Y-%m-%d--%H-%M-%S")
PROJ=impressions-processor
REGISTRY=localhost:5000
IMAGE=$REGISTRY/$PROJ:$TAG
eval $(docker-machine env default --shell bash)
    #--no-cache \
docker build -t $IMAGE \
    --build-arg DOTNET_CONFIG=Build \
    --build-arg INSTALL_CLRDBG="apt-get update \
        && apt-get install -y --no-install-recommends unzip \
        && rm -rf /var/lib/apt/lists/* \
        && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg" \
    ../
kubectl config use-context minikube
docker push $IMAGE
helm upgrade --install --set image=$IMAGE \
    -s env.ASPNETCORE_ENVIRONMENT=Development \
    -s env.AGGREGATE_MAX_EVENTS_COUNT=1 \
    -s livenessProbe.initialDelaySeconds=90 \
    -s livenessProbe.periodSeconds=90 \
    -s livenessProbe.timeoutSeconds=60 \
    -s readinessProbe.initialDelaySeconds=20 \
    -s readinessProbe.periodSeconds=30 \
    -s readinessProbe.timeoutSeconds=30 \
    $PROJ ../kube/$PROJ