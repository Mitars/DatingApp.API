# DatingApp.API

Dating App API is the API and backend for the [Build an app with ASPNET Core and Angular from scratch](https://www.udemy.com/course/build-an-app-with-aspnet-core-and-angular-from-scratch) Udemy course project.

While it uses the original as a basis it expands on this with a variety of additions:
- Exposes API endpoints through Swagger
- Separates the models and data access code into different projects


## Installation

In order to run the application you can use `dotnet run` from a command line within the `DatingApp.API`.

After that, you can navigate to [http://localhost:5000/]() to get the hosted site or go to [http://localhost:5000/swagger/index.html]() to check all the available API endpoints.

## Configuration

In order to be able to run the project correctly you will need to add an appsettings.json with the following segments:
- AppSettings
  - Token - The secret key
- ConnectionStrings
  - DefaultConnection - The database to which the service will connect to (SQLite or SQL Server. For MySQL you will need to recreate the migrations)
- CloudinarySettings
  - CloudName - Name of the cloud provider
  - ApiKey - Name of the Cloudinary API key
  - ApiSecret - Name of the Cloudinary API secret key.

Sample:
```
{
  "AppSettings": {
    "Token": "secret key"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=datingapp.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "CloudinarySettings": {
    "CloudName": "cloudName",
    "ApiKey": "123456789012345",
    "ApiSecret": "apisecret-keyvalue12345"
  }
}

```

## API Access

In order to access specific API endpoints, you will need an authorization JWT. You can get this by using one of the two endpoints in the AuthController:
- [http://localhost:5000/api/auth/login]()
- [http://localhost:5000/api/auth/register]()

## Seeded Data

By default when running the .NET Core application you will get a database with seeded data for 10 users and 1 administrator. All of the users have the same password `password` which can be used for testing.

## Roles

There are 4 different available roles: Admin, Moderator, Member, VIP.

The administrator has access to additional options that allow user role management. While the moderator role gives the user the ability to access approve or reject photos added by members. The Member and VIP roles currently have no notable uses.


## Frontend

For the frontend component, you can download the [Dating App SPA](https://github.com/Mitars/DatingApp.SPA).

