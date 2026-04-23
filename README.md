# HospitalAPI

Backend API for a hospital appointment management system, built with ASP.NET Core, Entity Framework Core and PostgreSQL.

## Tech Stack

- ASP.NET Core / .NET 10
- Entity Framework Core
- PostgreSQL
- JWT authentication
- Swagger / OpenAPI
- Docker and Docker Compose

## Requirements

- Docker Desktop

## Run With Docker

From the project root, run:

```bash
docker compose up --build
```

The API will be available at:

```txt
http://localhost:5095
```

Swagger UI:

```txt
http://localhost:5095/swagger
```

## Services

The Docker Compose setup starts two services:

- `hospital-api`: ASP.NET Core API
- `hospital-postgres`: PostgreSQL database

PostgreSQL is exposed locally at:

```txt
Host: localhost
Port: 5433
Database: HospitalDb
Username: postgres
Password: postgres
```

Inside Docker, the API connects to PostgreSQL using the Compose service name:

```txt
Host=postgres;Port=5432;Database=HospitalDb;Username=postgres;Password=postgres
```

## Useful Commands

Run services in the foreground:

```bash
docker compose up --build
```

Run services in the background:

```bash
docker compose up --build -d
```

View running services:

```bash
docker compose ps
```

View logs:

```bash
docker compose logs
```

Stop services:

```bash
docker compose down
```

Reset the database volume:

```bash
docker compose down -v
```

Use `docker compose down -v` only when you intentionally want to delete the database data.

## Database Migrations

The project includes Entity Framework Core migrations in the `Migrations` folder.

If running the API locally with `dotnet run`, apply migrations with:

```bash
dotnet ef database update
```

When running with Docker Compose, the API uses the PostgreSQL container defined in `docker-compose.yml`.

## Development URLs

- API: `http://localhost:5095`
- Swagger: `http://localhost:5095/swagger`
- PostgreSQL from host: `localhost:5433`

