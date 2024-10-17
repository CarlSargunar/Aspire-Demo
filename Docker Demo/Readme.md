# Docker Compose Sample

This sample application consists of several containers to create a website and 

## Notes

To build and run the local SQLDb container, use the following command:

 **NOTE : On windows, all the line endings need to be set to LF for the Dockerfile, setup.sql and startup.sh**

    docker build --tag=sqldb ./SQLDb
    docker run -d -p 1433:1433 --name sqldb sqldb    

To build the entire application, use the following command:

    docker-compose up --build