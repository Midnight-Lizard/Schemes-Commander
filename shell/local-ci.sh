#!/bin/sh
set -e
TAG=$(date +"%Y-%m-%d--%H-%M-%S")
PROJ=schemes-commander
REGISTRY=localhost:5000
IMAGE=$REGISTRY/$PROJ:$TAG
eval $(docker-machine env default --shell bash)
docker build -t $IMAGE \
    --build-arg DOTNET_CONFIG=Build \
    --build-arg INSTALL_CLRDBG="apt-get update \
        && apt-get install -y --no-install-recommends unzip \
        && rm -rf /var/lib/apt/lists/* \
        && curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg" \
    ../
kubectl config set-context minikube
docker push $IMAGE
./helm-deploy.sh -i $IMAGE -r $PROJ -c ../kube/$PROJ \
    -s env.ASPNETCORE_ENVIRONMENT=Development \
    -s env.IDENTITY_URL=http://localhost:7002/ \
    -s env.SCHEMES_QUEUE_CONFIG="{\"TopicName\":\"schemes-requests-a\",\"ProducerSettings\":{\"bootstrap.servers\":\"bootstrap.kafka:9092\"}}"