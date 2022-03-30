# Actor Model based on Azure Service Bus

Actor Model based on Azure Service Bus presentation plus source codes.

**NOTE** to run the source code you need to have access to Azure Service Bus standard version minimum.

## Infrastructure

The required topics:

- `actor/donor`
- `actor/necessitous`
- `actor/need-balancer`
- `web-client`

All topics should have `inbox` subscription created.
Each `inbox` subscription must have `Requires session` option switched on.

## Service Bus Connection string

Service bus connection string is set by environment veriable `ActorSystem:ServiceBusConnectionString` see
`src\MrMatrix.Net.ActorOnServiceBus.Worker\appsettings.json` file. The good idea is to
follow article
[Enable secret storage](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows#enable-secret-storage)

If you run following set of commands application should works fine.

```bash
cd 'src\MrMatrix.Net.ActorOnServiceBus.Worker'
dotnet user-secrets set "ActorSystem:ServiceBusConnectionString" "<Your service bus connection string>"
```
