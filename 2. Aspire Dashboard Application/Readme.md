# Running the Aspire Dashboard Standalonw

Start by copying all files from [1. Aspire App] to the current folder.

## Add Aspire Dashboard to the Solution

    dotnet new aspire-apphost -n AppHost
    dotnet new aspire-servicedefaults -n ServiceDefaults

### Add the new projects to the WeatherApi.sln

    dotnet sln add .\AppHost\AppHost.csproj
    dotnet sln add .\ServiceDefaults\ServiceDefaults.csproj

### Add the references to the WeatherApi project

Add a reference from the AppHost project to all the application projects, so the App Host can start them

    dotnet add .\AppHost\AppHost.csproj reference .\WeatherApi\WeatherApi.csproj
    dotnet add .\AppHost\AppHost.csproj reference .\AspireApp\AspireApp.csproj

Add a reference from the application projects to the service defaults project to setup Telemetry, Healthchecks and Service Discovery

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

Add a reference in the App Hosts Project to Redis Cache

    dotnet add .\AppHost\AppHost.csproj package Aspire.Cache.Redis

And add a reference to Aspire.StackExchange.Redis.OutputCaching to the AspireApp Project

    dotnet add .\AspireApp\AspireApp.csproj package Aspire.StackExchange.Redis.OutputCaching

### Add Aspire Config to AppHost

Update Program.cs

    // Add services to the container.
    var cache = builder.AddRedis("cache");

    var api = builder.AddProject<Projects.WeatherApi>("api");

    var app = builder.AddProject<Projects.AspireApp>("app")
        .WithReference(cache)
        .WithReference(api);

### Add refernce to the AspireApp

Update Program.cs

    // Add reference to cache service
    builder.AddRedisOutputCache("cache");


    // Add Redis Output Cache Middleware
    app.UseOutputCache();

## Run the AppHost

    dotnet run --project ./AppHost/AppHost.csproj --launch-profile "http"


# To Re-create this project from Scratch

Start by copying all files from [1. Aspire App] to the current folder, inc the SLN file

## Add Aspire Dashboard to the Solution

    dotnet new aspire-apphost -n AppHost
    dotnet new aspire-servicedefaults -n ServiceDefaults

### Add the new projects to the WeatherApi.sln

    dotnet sln add .\AppHost\AppHost.csproj
    dotnet sln add .\ServiceDefaults\ServiceDefaults.csproj

### Add the references to the WeatherApi project

Add a reference from the AppHost project to all the application projects, so the App Host can start them

    dotnet add .\AppHost\AppHost.csproj reference .\WeatherApi\WeatherApi.csproj
    dotnet add .\AppHost\AppHost.csproj reference .\AspireApp\AspireApp.csproj

Add a reference from the application projects to the service defaults project to setup Telemetry, Healthchecks and Service Discovery

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


## Run the AppHost


    dotnet run --project .\AppHost\AppHost.csproj
    


## Extra : Add cache

Add a reference in the App Hosts Project to Redis Cache

    dotnet add .\AppHost\AppHost.csproj package Aspire.Cache.Redis

And add a reference to Aspire.StackExchange.Redis.OutputCaching to the AspireApp Project

    dotnet add .\AspireApp\AspireApp.csproj package Aspire.StackExchange.Redis.OutputCaching

### Add Aspire Config to AppHost

Update Program.cs

    // Add services to the container.
    var cache = builder.AddRedis("cache");

    var api = builder.AddProject<Projects.WeatherApi>("api");

    var app = builder.AddProject<Projects.AspireApp>("app")
        .WithReference(cache)
        .WithReference(api);

### Add refernce to the AspireApp

Update Program.cs

    // Add reference to cache service
    builder.AddRedisOutputCache("cache");

    // Add Redis Output Cache Middleware
    app.UseOutputCache();

## Run the AppHost


    dotnet run --project .\AppHost\AppHost.csproj
    