# migrations, maincontext
dotnet ef migrations add AddInitialMigrationMain --context=MainContext --output-dir Migrations/Main

To update the database:
dotnet ef database update --context=MainContext

# migrations, sharedcontext
dotnet ef migrations add InitialCreate --context SharedContext --output-dir Migrations/Shared

# updating databases. First template, then CustomerA and CustomerB Don´t do this in production.
dotnet ef database update --context SharedContext 
dotnet ef database update --context SharedContext --connection "Data Source=Databases/CustomerA.db;"
dotnet ef database update --context SharedContext --connection "Data Source=Databases/CustomerB.db;"

# view example page
dotnet run -> navigate to endpoint -> https://localhost:xxxx/index.html

