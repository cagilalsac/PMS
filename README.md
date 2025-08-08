# Project Development Roadmap

Note: Project development roadmap can also be found at: https://github.com/cagilalsac/PMS/tree/master/PMS.AppHost/Roadmap.pdf

Note: In order to open the links, you must download and open the pdf file with a browser.  

Note: For database, you can either use SQL Server LocalDB if you use Windows and Visual Studio Community, or SQLite (https://www.sqlite.org/) 
if you use an operating system other than Windows, or SQL Server with Docker if you use an operating system other than Windows.

## 1. Environment and Tools

1. Visual Studio Community installation for Windows:  
   https://need4code.com/DotNet/Home/Index?path=.NET%5C00_Files%5CVisual%20Studio%20Community%5CInstallation.pdf

2. Rider for MAC:  
   https://www.jetbrains.com/rider

3. Docker Desktop installation for MAC:  
   https://need4code.com/DotNet?path=.NET%5C00_Files%5CDocker%5CDocker%20Microsoft%20SQL%20Server.pdf
      
4. The E-R Diagram of the project:  
   https://need4code.com/DotNet?path=.NET%5C01_SQL%5CDemos%5C10%20-%20Project%20Management%20System%5CE-R%20Diagram.jpg

## 2. Solution Setup

5. Create a .NET Aspire Empty App project.

6. Name the solution as your project name. If you want to change the solution folder in Location, "Create in new folder" option must be checked.

7. Select .NET 8.0 as the Framework, check Configure for HTTPS and select .NET Aspire version as 8.2.

## 3. CORE Project

8. In Solution Explorer, create a new project called CORE (Class Library, .NET 8.0).

9. Set Nullable to Disable for all class library projects (via project properties or XML).

10. Create the folders and classes under the CORE project as in:  
    https://github.com/cagilalsac/PMS/tree/master/CORE  
    - APP/Domain/Entity.cs  
    - APP/Services/ServiceBase.cs  
    - APP/Models/Request.cs  
    - APP/Models/QueryResponse.cs  
    - APP/Models/CommandResponse.cs

## 4. Projects.APP Project

11. Create a new project under your solution as Class Library and name it Projects.APP.
 
12. Set Nullable to Disable for Projects.APP.
 
13. Create the Tag entity class under the Domain folder:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Domain/Tag.cs
 
14. Create the ProjectsDb DbContext class under the Domain folder:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Domain/ProjectsDb.cs
 
15. Install Microsoft.EntityFrameworkCore.SqlServer NuGet package (latest version 8.x).
 
16. For SQLite, install System.Data.SQLite.Core and Microsoft.EntityFrameworkCore.Sqlite NuGet packages (latest version 8.x).  
    (Optional) Install "SQLite and SQL Server Compact Toolbox" extension for Visual Studio for SQLite management.

## 5. Projects.API Project

17. Create a new project named Projects.API (ASP.NET Core Web API, .NET 8, no authentication, HTTPS, OpenAPI, controllers, Aspire orchestration).
 
18. Install Microsoft.EntityFrameworkCore.Tools NuGet package (latest version 8.x). Set Projects.API as Startup Project.
 
19. Create your database using migrations:  
    - Open Package Manager Console  
    - Set Projects.APP as Default Project  
    - Run:  
      add-migration v1  
      update-database  
    (For Rider, use the UI as described in JetBrains documentation.)

20. View your created database in SQL Server Object Explorer or find your SQLite database file under Projects.API.

## 6. Projects.APP Features

21. Install MediatR NuGet package in Projects.APP.
 
22. Create a Services folder and add ProjectsDbService:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Services/ProjectsDbService.cs
 
23. Under Features/Tags, add TagQueryHandler:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Tags/TagQueryHandler.cs
 
24. Add TagCreateHandler and TagCreateRequest:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Tags/TagCreateHandler.cs
 
25. Add TagUpdateHandler and TagUpdateRequest:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Tags/TagUpdateHandler.cs
 
26. Add TagDeleteHandler and TagDeleteRequest:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Tags/TagDeleteHandler.cs

## 7. Projects.API Configuration

27. Configure dependency injection for ProjectsDb and IMediator in Program.cs:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/Program.cs
 
28. Use the connection string from appsettings.json in ProjectsDb (remove OnConfiguring):  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Domain/ProjectsDb.cs  
 
    Add the connection string to appsettings.json:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/appsettings.json
 
29. Create and modify TagsController:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/Controllers/TagsController.cs

## 8. Project and Work Entities

30. Create Project and ProjectTag entities:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Domain/Project.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Domain/ProjectTag.cs  
 
    Add navigation property to Tag entity:  
    public List<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
 
31. Add DbSet<Project> and DbSet<ProjectTag> to ProjectsDb:
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Domain/ProjectsDb.cs
 
32. Run migration for new tables:  
    add-migration v2  
    update-database

## 9. Project and Work Handlers

33. Add ProjectQueryHandler, ProjectCreateHandler, ProjectUpdateHandler, ProjectDeleteHandler under Features/Projects:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Projects/ProjectQueryHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Projects/ProjectCreateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Projects/ProjectUpdateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Projects/ProjectDeleteHandler.cs

34. Add WorkQueryHandler, WorkCreateHandler, WorkUpdateHandler, WorkDeleteHandler under Features/Works:  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Works/WorkQueryHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Works/WorkCreateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Works/WorkUpdateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.APP/Features/Works/WorkDeleteHandler.cs

## 10. API Controller Scaffolding

35. Scaffold ProjectsController and WorksController using Visual Studio:  
    Scaffolding templates can be found at:
    https://need4code.com/DotNet/Home/Index?path=.NET%5C00_Files%5CScaffolding%20Templates%5CTemplates.7z
    Extract the Templates folder under your WebApi Project folders.
    
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/Controllers/ProjectsController.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/Controllers/WorksController.cs

## 11. Generic Service and User Management

36. Implement a base generic service class in CORE/APP/Services:  
    https://github.com/cagilalsac/PMS/blob/master/CORE/APP/Services/Service.cs
 
37. Create UserService and user/role/skill handlers in Users.APP:  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Services/UserService.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Users/UserCreateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Users/UserDeleteHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Users/UserQueryHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Users/UserUpdateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Roles/RoleCreateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Roles/RoleDeleteHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Roles/RoleQueryHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Roles/RoleUpdateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Skills/SkillCreateHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Skills/SkillDeleteHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Skills/SkillQueryHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Skill/SkillUpdateHandler.cs

38. Add AppSettings and configure JWT authentication in Users.APP and Users.API:  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/AppSettings.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.API/appsettings.json  
    https://github.com/cagilalsac/PMS/blob/master/Users.API/Program.cs

39. Implement token and refresh token logic in Users.APP/Features/Users:  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Users/TokenHandler.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.APP/Features/Users/RefreshTokenHandler.cs

40. Apply [Authorize] and [AllowAnonymous] attributes in API controllers:  
    https://github.com/cagilalsac/PMS/blob/master/Users.API/Controllers/UsersController.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.API/Controllers/RolesController.cs  
    https://github.com/cagilalsac/PMS/blob/master/Users.API/Controllers/SkillsController.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/Controllers/ProjectsController.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/Controllers/TagsController.cs  
    https://github.com/cagilalsac/PMS/blob/master/Projects.API/Controllers/WorksController.cs

## 12. API Gateway

41. Create Gateway.API project (ASP.NET Core Web API, .NET 8, no OpenAPI, no controllers, Aspire orchestration).
 
42. Install Ocelot NuGet package.
 
43. Configure Program.cs and add ocelot.json:  
    https://github.com/cagilalsac/PMS/blob/master/Gateway.API/Program.cs  
    https://github.com/cagilalsac/PMS/blob/master/Gateway.API/ocelot.json  
    Use launchSettings.json to set correct downstream URLs.

44. Access all endpoints via the gateway URL.  
    Example:  
    https: https://localhost:7212  
    http: http://localhost:5260

Note: Swagger is not available for the gateway with this configuration. Use Postman or client apps to test gateway endpoints.