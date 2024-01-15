@echo OFF

kind create cluster --config kind-cluster-config.yaml -v 9 --wait 240s

dapr init -k --wait --timeout 150
dapr status -k


kind load docker-image -n dapr istdapi:latest
kind load docker-image -n dapr digitext:latest
kind load docker-image -n dapr digitsworker:latest
kind load docker-image -n dapr digitizer:latest

REM install redis with use of helm
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update
helm install redis bitnami/redis


REM install zipkin for tracing
kubectl create deployment zipkin --image openzipkin/zipkin
kubectl create service nodeport zipkin --tcp=9411:9411 --node-port=30411
REM kubectl expose deployment zipkin --type NodePort --port 9411 --target-port 9411

REM install dapr config
kubectl apply --cluster kind-dapr -f ../cfg/config.dapr.k8s.yaml
REM install dapr components
kubectl apply --cluster kind-dapr -f ../resources-k8s
REM create K8s resources needed for the app to run
kubectl apply --cluster kind-dapr -f .

kubectl get all -A
dapr components -k --all-namespaces

