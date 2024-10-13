# Docker Compose Sample

This sample application consists of several containers to create a website and 

## Notes

To build and run the local SQLDb container, use the following command:

    docker build --tag=sqldb ./SQLDb
    docker run -d -p 1433:1433 --name sqldb sqldb    