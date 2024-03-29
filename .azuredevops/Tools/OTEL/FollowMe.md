helm repo add open-telemetry https://open-telemetry.github.io/opentelemetry-helm-charts

helm install opentelemetry-collector open-telemetry/opentelemetry-collector --debug --set mode=deployment  -f  .\cmb-otel-collecter-deployment-values.yaml

k apply -f .\otel-dep.yaml


