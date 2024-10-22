# Docker Compose Sample

This sample application consists of several containers to create a website <TODO: COMPLETE ME LATER!!!!!>

## Setting Environment Variables

The following environment variables are used in the application. You can set up a `.env` file in the root of the project to set these variables. 

**NOTE: The `.env` file is not included in the repository, it is ignored deliberately. You need to pick the lined relevant to your operating system - Windows or macOS/Linux**


    # Windows (APPDATA typically set automatically)
    USER_SECRETS_PATH=${APPDATA}/Microsoft/UserSecrets
    HTTPS_CERT_PATH=${APPDATA}/ASP.NET/Https

    # macOS/Linux
    USER_SECRETS_PATH=${HOME}/.microsoft/usersecrets
    HTTPS_CERT_PATH=${HOME}/.aspnet/https

Or the alternative is to set a universal dockerfile. Needs to be tested

    USER_SECRETS_PATH=${APPDATA:-${HOME}/.microsoft/usersecrets}
    HTTPS_CERT_PATH=${APPDATA:-${HOME}/.aspnet/https}


## Notes

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

## Configuration for building the application

The docker compose file is configured to use the following environment variables:

- DOCKER_REGISTRY - The registry to use for the images. If null or not set, the images will be built locally.
- USER_SECRETS_PATH - The path to the user secrets directory.
- HTTPS_CERT_PATH - The path to the HTTPS certificate directory.

To build the entire application, use the following command:

    docker-compose up --build


## EF Core Migrations

To initialize the database, use the following command:

    
    

