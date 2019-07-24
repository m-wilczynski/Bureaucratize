## Bureaucratize

Sourcecode for my engineer degree thesis: ***"Processing documents filled in by hand with example usage in web applications"***.

Not all functionalities might be up to date with final version but it should be enough for showing general idea behind the work.

For Polish version of instruction or any inquiries, questions or change requests please email me via: michal@wilczynski.pro

### Technologies
- CNTK (CNN) + Accord.NET
- ASP.NET Core with WebSockets
- Akka.NET
- Entity Framework Core with SQL Server

### Requirements
- Windows or Windows Server with IIS 8.0 or greater (with WebSockets support)
- NVIDIA GPU if you want to run Bureaucratize.MachineLearning solution
- SQL Server; for development SQL Server Express with LocalDB is enough: https://www.microsoft.com/pl-pl/sql-server/sql-server-editions-express

### How to build
- Get Visual Studio 2017 - https://www.visualstudio.com/downloads/
- Get .NET Framework 4.6.2 - https://www.microsoft.com/net/download/dotnet-framework-runtime/net462
- Get .NET Core 2.0 SDK (or later, ie. 2.1) - https://www.microsoft.com/net/download/windows
- Add local nuget source to point at **./src/local-nuget** folder in Visual Studio (Tools -> Options -> NuGet Package Manager -> Package Sources)
- Run chosen solution:
  * **./src/Bureaucratize/Bureaucratize.sln** for main WebApp and AkkaHost
  * **./src/Bureaucratize.FileStorage/Bureaucratize.FileStorage.sln** for FileStorage service
  * **./src/Bureaucratize.MachineLearning/Bureaucratize.MachineLearning.sln** for machine learning ConsoleApp (requires GPU for NVIDIA CUDA acceleration!)
  
### How to run in development mode
- Ensure that you have local Nuget feed configured properly (see my instruction: https://github.com/m-wilczynski/env-utils/blob/master/notes/local-nuget-feed.md#create-directory-for-local-nuget-feed)
- Ensure that you have local SQL Server instance with LocalDB; if not, install SQL Server Express and choose LocalDB during installation; installation package for SQL Server Express: https://www.microsoft.com/pl-pl/sql-server/sql-server-editions-express
- Configure connecting strings for all applications (it can be the same database), in configs:
  - **Web App:** .\src\Bureaucratize\Bureaucratize.Web\appsettings.json
  - **Akka Host:** .\src\Bureaucratize\Bureaucratize.Console.Host\App.config
  - **File Storage Service:** .\src\Bureaucratize.FileStorage\Bureaucratize.FileStorage.Service\appsettings.json
- Open Visual Studio 2017 and run **./src/Bureaucratize.FileStorage/Bureaucratize.FileStorage.sln** and hit "IIS Express" to run it in developement mode; browser window will open and you will get service address (by default it's http://localhost:50499)
- Configure address of FileStorage service in rest of the applications (they are configured to use http://localhost:50499 by default) , ie:
  - **Web App:** .\src\Bureaucratize\Bureaucratize.Web\appsettings.json
  - **Akka Host:** .\src\Bureaucratize\Bureaucratize.Console.Host\App.config
- Open NEW instance of Visual Studio 2017 and run **./src/Bureaucratize/Bureaucratize.sln**; then hit "Start"; both Web App and Akka Host should launch together;
  
### Known issues
- Sometimes WebApp (**Bureaucratize.Web**) or AkkaHost (**Bureaucratize.ImageProcessing.Host**) will fail to build due to mismatch of libraries between .NET Framework and .NET Core when you build **entire solution**; to fix this, REBUILD only chosen application project (sometimes you might need to REBUILD and then - immediately after - BUILD chosen application .csproj);
- if ImageProcessing.Host doesn't start first, Bureaucratize.Web won't connect properly (ever). You need to restart AppPool where Bureaucratize.Web is installed so that it will connect properly to ActorSystem for the first time; if you're running it via Visual Studio (via IIS Express), just stop the web app and run it again;

### Project has been curated under PJATK

![](http://www.pja.edu.pl/templates/pjwstk/images/logo-lg-md.png)

**Website:** http://www.pja.edu.pl
