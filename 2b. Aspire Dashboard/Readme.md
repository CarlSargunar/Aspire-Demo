# Running the Aspire Dashboard Standalonw

Start by copying all files from [1. Aspire App] to the current folder.

## Add Aspire Dashboard to the Solution

    dotnet new aspire-apphost -n AppHost
    dotnet new aspire-servicedefaults -n ServiceDefaults

### Add the new projects to the WeatherApi.sln

    dotnet sln add .\AppHost\AppHost.csproj
    dotnet sln add .\ServiceDefaults\ServiceDefaults.csproj

### Add the references to the WeatherApi project

    dotnet add .\AppHost\AppHost.csproj reference .\WeatherApi\WeatherApi.csproj
    dotnet add .\AppHost\AppHost.csproj reference .\AspireApp\AspireApp.csproj
    dotnet add .\WeatherApi\WeatherApi.csproj reference .\ServiceDefaults\ServiceDefaults.csproj
    dotnet add .\AspireApp\AspireApp.csproj reference .\ServiceDefaults\ServiceDefaults.csproj


### Configure the Aspire App

Update the Program.cs for the AppHost app

    // Add services to the container.

    var api = builder.AddProject<Projects.WeatherApi>("api");

    var app = builder.AddProject<Projects.AspireApp>("app")
        .WithReference(api);


### Configure ServiceDefaults

In the Program.cs of each app you need to ensure there is a line

    builder.AddServiceDefaults();


## Extra : Add cache

Add a refernce in the App Hosts Project to Redis Cache

    dotnet add .\AppHost\AppHost.csproj package Aspire.Cache.Redis

And add a reference to Aspire.StackExchange.Redis.OutputCaching to the AspireApp Project

### Add Aspire Config to AppHost

    // Add services to the container.
    var cache = builder.AddRedis("cache");

    var api = builder.AddProject<Projects.WeatherApi>("api");

    var app = builder.AddProject<Projects.AspireApp>("app")
        .WithReference(cache)
        .WithReference(api);


    