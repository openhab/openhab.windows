# Using Autorest to generate client libaries based on swagger json schema

- Download AutoRest https://github.com/Azure/autorest
- Run command: autorest --input-file=swagger.json --csharp --namespace=openHAB.Core.Client --add-credentials --override-client-name=OpenHABClient
- Copy generated folder content to project root "openHAB.Core.Client"