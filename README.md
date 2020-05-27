# Employee education system for DevBridge

Functional webapp used to store and manage company employee information, self-study topics, subtopics, self-study dates, goals.
Employees can choose to assign any topic as a goal, which they can later assign to a particular date, letting their team members and managers know that they're studying or have studied a particular topic.
Managers have access to the same functionality as regular employees as well as additional functionality including a calendar used for managing and viewing their subordinates' self-study topics and dates.

### For developers
Developed using ASP .NET Core 3.1 on Microsoft SQL Server
Visual Studio 2019 IDE *strongly* recommended.
Further info is only relevant if using Visual Studio 2019 IDE.

#### To create a local DB:
1. Open Package Manager console
2. Make sure default project is set to "EducationSystem"
3. Create a migration if there isn't one:  
	1. Run `add-migration "Migration name"`
4. Run `update-database`

#### To delete local DB:
1. Open Package Manager console
2. Run `sqllocaldb stop` and `sqllocaldb delete`
4. Delete local database file in `users\{YourUser}`, file extension .mdf