# Mobility.Payments.Api
This project manages payment services for Mobility.

## Getting Started
This project is designed for a microservice architecture using .NET Core. It aims to provide efficient and secure handling of user authentication, transactions, and other payment-related processes.
 
### Source and Documentation
`
https://github.com/IsaacValiente/Mobility.Payments/tree/main
`

### Prerequisites 
- [SQL Server](https://www.microsoft.com/en-us/sql-server) or [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-editions-express).
- [.NET 8 SDK](https://dotnet.microsoft.com/download).
- [Visual Studio](https://visualstudio.microsoft.com/).
- [SQL Server Management Studio](https://aka.ms/ssmsfullsetup).
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for running the application in Docker containers).

#### Running the project
The application can be run in Docker containers or normally.

#### Running the project with Docker
To run the project using Docker, follow these steps:

1. **Install Docker**: Make sure Docker Desktop is installed and running on your machine.

2. **Environment Variables**:
Docker Compose uses the following environment variables. You can set them in an .env file or directly in your terminal before running Docker Compose:
```
KESTREL_CERT_PASSWORD=<your-certificate-password>
JWT_SECRET=<your-jwt-secret>
CONNECTION_STRING=Server=sql,1433;Database=MobilityPayments;User ID=sa;Password=<your-sql-server-password>;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=true;
API_KEY=<your-api-key>
MSSQL_SA_PASSWORD=<your-sql-server-password>
```
Notes:
- `"KESTREL_CERT_PASSWORD"` This value represents the password certificate for Kestrel. Type Guid
- `"CONNECTION_STRING"` The value password <your-sql-server-password> must be replaced with the value of `{MSSQL_SA_PASSWORD}`.
- `"API_KEY"` This value represents the API Key, and it must be sent in the `X-API-Key` header when making requests to the API. Type Guid
- `"JWT_SECRET"` This can be any string, but it must be long enough to ensure security (e.g., `this-will-be-a-super-secret-key-for-jwt-configuration`)
- `"MSSQL_SA_PASSWORD"` password for the sa db user

3. **Build and run containers using Docker Compose**:
   In the project root folder `Mobility.Payments`, where your `docker-compose.yml` file is located, open a terminal and run the following command:
   
   ```
   docker-compose up -d
   ```
   
By default, the application will be available at: `http://localhost:8081`
You can access swagger interface on `https://localhost:8081/swagger/index.html`

You can stop the containers at any time by running:
   ```
   docker-compose down
   ```

#### Running the project
### Installing

Clone repository and open `.sln` file in Visual Studio 2022. Build Solution.

#### Setting up SQL Server

1. Download SQL Server Developer if you haven't.
2. After installing, write down your server name (Such as `localhost` or `MSSQLSERVER`).

#### Creating an admin user in SQL Server

1. Download SQL Server Management Studio (SSMS) if you haven't.
2. Open it, and type the server name you wrote down earlier. Use Windows Authentication.
3. In the `Object Explorer` window to the left side, open your server and it'll show up some folders. Open `Security` and right click on `Logins` folder. Click on `New Login...`.
4. In `Login Name`, type a username.
5. Select `SQL Server authentication` radio button and type a password.
6. `Default Database`: `master`.
7. Click on `Server Roles` (Under `Select a page` section).
8. Check the following: `sysadmin`, `serveradmin`, `securityadmin`, `processadmin`, `diskadmin`, `dbcreator`, `bulkadmin`.
9. Click on `User Mapping` (Under `Server Roles`).
10. Check `MobilityPayments` (if available) and check `db_owner` in the Database role membership.
11. Click `OK`.

At this point there's an extra step to be followed, as the server authentication by default is with Windows Authentication. 

To enable both SQL Server and Windows Authentication mode:

1. Right click on your server (That's inside the `Object Explorer` window).
2. Select `Properties`.
3. Under the `Select a page` section, click on `Security`.
4. Check `SQL Server and Windows Authentication mode` radio button under `Server authentication` .
5. Click `OK`.

Now that both SQL Server and Windows Authentication are allowed, we can disconect from the server:

1. Right click on your server.
2. Click `Disconnect`.

Now, try to connect again to your server using the new credentials. 
Remember to change authentication to `SQL Server Authentication`.
If you can connect with your new credentials, then this step is finished.

### `secrets.json` structure

```
{
  "ConnectionStrings": {
    "MobilityConnection": "Server=localhost;Database=MobilityPayments;User ID={replace-with-db-username};Password={replace-with-db-password};Trusted_Connection=False;Encrypt=True;TrustServerCertificate=true;"
  },
    "ApiKey": {
    "value": "api-key-value"
  },
  "JwtConfiguration": {
    "Secret": "jwt-configuration-secret"
  }
}
```

Notes:
- `"ConnectionStrings-MobilityConnection"` the values ID and password must be replaced with the values obtained in the creation of the db user.
- `"ApiKey-value"` This value represents the API Key, and it must be sent in the `X-API-Key` header when making requests to the API. Type Guid
- `"JwtConfiguration-Secret"` This can be any string, but it must be long enough to ensure security (e.g., `this-will-be-a-super-secret-key-for-jwt-configuration`).

## Running the project without docker containers

### Through Visual Studio

Select `Mobility.Payments.Api` from the dropdown list in the Text Editor Toolbar and press `F5` to start debugging (Press `Ctrl+F5` to run without debugging).

### Through Command Window

1. Open a Command Window inside `Mobility.Payments/Mobility.Payments.Api/`.

2. Type `dotnet run .\Mobility.Payments.Api.csproj --launch-profile Mobility.Payments.Api`.


By default, the application will be available at: `https://localhost:7001`
You can access swagger interface on `https://localhost:7001/swagger/index.html`
## Running the tests

There are two ways two run tests: Through Visual Studio or by Command Window.

### Running tests with Visual Studio

#### Running all tests

Press `Ctrl+R, A`, or go to `Test` -> `Run All Tests`.

#### Running a project's test

Right click on any project with a `.Test` suffix, click on `Run Tests`.

#### Running a single test

Right click on any file inside a Test project, click on `Run Tests`.

### Running tests by Command Window

#### Running all tests

1. Open a Command Window in the root folder, where the `.sln ` file is.
2. Type `dotnet test .\Mobility.Payments.sln -c Release` and press Enter.

#### Running a project's test

1. Open a command window or terminal inside a project with a `.Tests` suffix.
2. Type `dotnet test Mobility.Payments.<name-of-project>.Tests.csproj -c -Release` and press Enter.

## Authors

Isaac Gonzalez

## License

This project  is licensed under the MIT License - see the [LICENSE](LICENSE) file for details 
