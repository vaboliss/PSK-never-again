# PSK-never-again

DevBridge project using ASP .NET Core  
.NET Core 3.1

## Starting project

### To create a local DB:
1. Open Package Manager console
2. Select Default project: "EducationSystem"
3. Create a migration if there isn't one:  
	1. Run command `add-migration "Migration name"`
4. Run command `update-database`

### To delete local DB:
1. Open Package Manager console
2. Run command `sqllocaldb stop`
3. Run command `sqllocaldb delete`

More documentation is in the google drive
