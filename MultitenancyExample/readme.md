# migrations, maincontext
dotnet ef migrations add AddInitialMigrationMain --context=MainContext --output-dir Migrations/Main

To update the database:
dotnet ef database update --context=MainContext
