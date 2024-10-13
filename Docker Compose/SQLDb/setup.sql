USE [master]
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'UmbracoDb')
BEGIN

    -- Create an empty database for the CMS
    CREATE DATABASE [UmbracoDb];
 
END;
GO

USE UmbracoDb;