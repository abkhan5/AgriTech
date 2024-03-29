kubectl create namespace monitoring

helm repo add prometheus-community https://prometheus-community.github.io/helm-charts

helm install prometheus prometheus-community/prometheus --debug

helm upgrade prometheus prometheus-community/prometheus --debug -n monitoring  -f .\prom-values.yaml --debug

k expose service prometheus-server --type=LoadBalancer --target-port=9090 --name=promserver

*************************************************

