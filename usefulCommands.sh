dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialVersion --context PostgresContext
dotnet ef migrations script InitialVersion --output Script/script.sql
dotnet ef database update
