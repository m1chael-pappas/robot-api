# Robot Controller API

A backend REST API that provides resources for the Moon robot simulator. Built with ASP.NET Core (.NET 10) and PostgreSQL.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL 14+](https://www.postgresql.org/download/)

## Getting Started

1. **Set up the database**

   Create a PostgreSQL database and run the schema file to set up tables and seed data:

   ```bash
   psql -U postgres -d your_database -f db-schema.sql
   ```

2. **Configure the connection string**

   Update `appsettings.Development.json` with your PostgreSQL connection details.

3. **Run the application**

   ```bash
   dotnet run
   ```

   The API will be available at `https://localhost:5172`. Swagger UI is served at the root URL.

## API Endpoints

| Resource         | Endpoints                                      | Auth Required |
| ---------------- | ---------------------------------------------- | ------------- |
| Maps             | `GET/POST /api/maps`, `GET/PUT/DELETE /api/maps/{id}` | Admin (write), User (read) |
| Robot Commands   | `GET/POST /api/robot-commands`, `GET/PUT/DELETE /api/robot-commands/{id}` | Admin (write), User (read) |
| Users            | `GET/POST /api/users`, `GET/PUT/DELETE /api/users/{id}` | Admin only |

Additional endpoints:
- `GET /api/maps/square` -- filter square maps only
- `GET /api/maps/{id}/{x}-{y}` -- check map coordinate occupancy
- `GET /api/robot-commands/move` -- filter move commands only

## Authentication

The API uses **HTTP Basic Authentication** with role-based authorization:

- **Admin** -- full access to all endpoints
- **User** -- read access to maps and robot commands

## Project Structure

```
robot-api/
├── Authentication/     # Basic auth handler
├── Controllers/        # API controllers (Maps, RobotCommands, Users)
├── Models/             # Data models
├── Persistence/        # Data access layer (Npgsql)
├── tests/              # Postman collections and environment config
├── wwwroot/            # Static files (Swagger UI theme, favicon)
├── db-schema.sql       # PostgreSQL schema with seed data
└── Program.cs          # Application entry point
```

## Running Tests

Tests are defined as Postman collections in the `tests/` folder. Run them with [Newman](https://www.npmjs.com/package/newman):

```bash
newman run tests/postman-request-collection.json \
  -e tests/postman-environment-variables.json \
  -r htmlextra
```

Test results and Newman reports are generated output and are git-ignored.

## Author

Michael Pappas -- s225071597@deakin.edu.au
