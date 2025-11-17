Command for migrations:

run in main solution folder

```bash
dotnet ef migrations add {migrationName} --project Restaurants.Infrastructure/Restaurants.Infrastructure.csproj --startup-project Restaurants.API/Restaurants.API.csproj

```