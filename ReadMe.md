EnsekTechTestSJ
To Get started - Open the project EnsekTechTestSJ

The solution is using Entity Framework Core - code first approach 
To create a db instance for testing modify the SqlConnectionString in appsettings.Development.json so that it points to your MSSql server, db instance , initial catalog etc 
I used SQL server authentication to connect to a MSSQLExpress DB instance

To initialaise the database 
Use Package Manager Console within the Visual Studio IDE
Select the Ensek.Data project

run the migration but with the following command 

update-database -context EnsekDbContext -verbose

If you wish to create a new migration after changing any of the Data Entities run the following Package Manager Console
add-migration <MigrationName> 
To apply the new migration call the update-database command again