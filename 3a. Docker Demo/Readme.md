# Docker Compose Sample

This sample application consists of several containers to create a website <TODO: COMPLETE ME LATER!!!!!>

## Running individual Containers

### RabbitMQ

The RabbitMQ container is configured to use the default username and password of `guest`.This is not recommended for production use. To start the RabbitMQ container, use the following command:

    docker run -d -p 5672:5672 -p 15672:15672 --name rabbitmq rabbitmq:3-management

To run the management interface, navigate to `http://localhost:15672` in your browser. The default username and password are `guest`.

### SQLDb - Database Container

To build and run the local SQLDb container, use the following command:

 **NOTE : On windows, all the line endings need to be set to LF for the Dockerfile, setup.sql and startup.sh**

    docker build --tag=sqldb -f SQLDb/Dockerfile .
    docker run -d -p 1433:1433 --name sqldb sqldb    

### Application Containers

    docker build --tag=demoapi -f DemoAPI/Dockerfile .
    docker build --tag=message-processor -f MessageProcessor/Dockerfile .
    docker build --tag=umbwebsite -f UmbWebsite/Dockerfile .
    docker build --tag=analyticsapp -f AnalyticsApp/Dockerfile .

## Notes (mostly for myself)

### EF Core Migrations

To initialize the database, use the following command:

    dotnet ef migrations add InitialCreate
    dotnet ef database update

### Set-up Umbraco

    dotnet new install Umbraco.Templates::13.5.2 --force
    dotnet new umbraco --force -n "UmbWebsite" --friendly-name "Administrator" --email "admin@example.com" --password "1234567890" --connection-string "server=localhost;database=UmbracoDb;user id=sa;password='SQL_password123';TrustServerCertificate=true;" --connection-string-provider-name "Microsoft.Data.SqlClient"
    dotnet add "UmbWebsite" package Umbraco.TheStarterKit --version 13.0

For Aspire, we need to use SQLite

    dotnet new install Umbraco.Templates::13.5.2 --force
    dotnet new umbraco --force -n "UmbWebsite" --friendly-name "Administrator" --email "admin@example.com" --password "1234567890" --development-database-type SQLite
    dotnet add "UmbWebsite" package Umbraco.TheStarterKit --version 13.0

### Running the Application

    dotnet run --project UmbWebsite
    dotnet run --project DemoAPI
    dotnet run --project MessageProcessor
    dotnet run --project AnalyticsApp

## Running the Application in Docker Compose

To build the entire application, use the following command:

    docker-compose up --build

The apps are available at the following URLs:

- Analytics : [http://localhost:8084/](http://localhost:8084/)
- API : [http://localhost:8082/api/Analytics/pageviews](http://localhost:8082/api/Analytics/pageviews)
- RabbitMQ : [http://localhost:15672/](http://localhost:15672/)
- Umbraco : [http://localhost:8080/](http://localhost:8080/)