# Demo Notes

The Hello, World! sample for aspire will create a web site, an API and a redis cache. To get the sample running you first need to make sure you have the following installed:

    dotnet new aspire-starter --use-redis-cache --output AspireSample

This will create the sample project in a folder called AspireSample. You need to trust the ASP.NET Core localhost certificate but running the following.

    dotnet dev-certs https --trust

You can the run the sameple by running

    dotnet run --project AspireSample/AspireSample.AppHost

You will seen an output similar to the following, which will include a link to the dashboad with a token

    info: Aspire.Hosting.DistributedApplication[0]
        Aspire version: 8.2.1+137e8dcae0a7b22c05f48c4e7a5d36fe3f00a8d7
    info: Aspire.Hosting.DistributedApplication[0]
        Distributed application starting.
    info: Aspire.Hosting.DistributedApplication[0]
        Application host directory is: /Users/carl/Work/Temp/Aspire/AspireSample/AspireSample.AppHost
    info: Aspire.Hosting.DistributedApplication[0]
        Now listening on: https://localhost:17106
    info: Aspire.Hosting.DistributedApplication[0]
        Login to the dashboard at https://localhost:17106/login?t=9e94015d17f13869c997383b73846ef4
    info: Aspire.Hosting.DistributedApplication[0]
        Distributed application started. Press Ctrl+C to shut down.

Browse to the dashboard to see the running application. The dashboard will show the running services and the logs for the services.

## References

- https://learn.microsoft.com/en-us/dotnet/aspire/get-started/build-your-first-aspire-app?pivots=dotnet-cli
- 