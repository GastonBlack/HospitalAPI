# HospitalAPI

Backend API for a hospital appointment management system, built with ASP.NET Core, Entity Framework Core and PostgreSQL.

## Project Status

This project is not being continued.

I built it as a learning project to practice backend development with ASP.NET Core, Entity Framework Core, authentication, authorization, Docker, PostgreSQL, migrations, services, controllers and automated tests. At this point I already learned what I needed from it, so I prefer to stop here and start a new project instead of continuing to add features.

The API works as a base hospital system, but it is not intended to be production-ready.

## What The Project Does

HospitalAPI manages three main user types:

- Admins
- Medics
- Patients

It also manages appointment tickets between patients and medics.

The project includes:

- Patient registration and profile management
- Medic management by admins
- Login with JWT authentication stored in an HTTP-only cookie
- Role-based authorization
- Appointment ticket creation by patients
- Ticket updates by admins and medics
- Basic ticket status transitions
- Active/inactive status handling for medics and patients
- PostgreSQL persistence with Entity Framework Core
- Database migrations
- Docker Compose setup
- Swagger UI for API testing
- Unit tests for selected services and helpers

## Tech Stack

- ASP.NET Core / .NET 10
- Entity Framework Core
- PostgreSQL
- JWT authentication
- Cookie-based token storage
- Swagger / OpenAPI
- Docker and Docker Compose
- xUnit

## Main Features

### Authentication

Users log in with document number and password.

The login endpoint returns the current user data and stores the JWT token in a secure cookie.

Available auth endpoints:

```txt
POST /api/Auth/login
POST /api/Auth/logout
GET  /api/Auth/me
```

### Patients

Patients can register themselves. Admins and medics can list patients. Patients can view or update only their own data.

Available patient endpoints:

```txt
GET   /api/Patient
GET   /api/Patient/{id}
POST  /api/Patient
PUT   /api/Patient/{id}
PATCH /api/Patient/{id}/status
```

### Medics

Admins can create, list and disable medics. Patients can list available medics for creating appointments.

Available medic endpoints:

```txt
GET   /api/Medic
GET   /api/Medic/available
GET   /api/Medic/{id}
POST  /api/Medic
PUT   /api/Medic/{id}
PATCH /api/Medic/{id}/status
```

### Tickets

Patients can create appointment tickets. Admins can see all tickets. Medics and patients can see only the tickets that belong to them.

Ticket statuses:

```txt
Pending
Confirmed
Completed
Cancelled
```

Available ticket endpoints:

```txt
GET  /api/Ticket
GET  /api/Ticket/{id}
GET  /api/Ticket/medic/{medicId}
GET  /api/Ticket/patient/{patientId}
POST /api/Ticket/{patientId}
PUT  /api/Ticket/{id}
```

## What Was Left Pending

The project works as a learning backend, but these parts were left unfinished:

- Add optional filters to `GET /api/Patient`
- Add optional filters to `GET /api/Medic`
- Add optional filters to `GET /api/Ticket` for admins
- Add pagination to list endpoints
- Add more complete tests for medics, auth and ticket update flows
- Improve validation and error response consistency
- Add refresh tokens or a fuller auth flow
- Add frontend integration
- Add production deployment configuration

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

## Tests

Run the test project with:

```bash
dotnet test
```

The tests currently cover only part of the behavior. They are useful as examples, but they are not complete enough to guarantee the whole API.

## Development URLs

- API: `http://localhost:5095`
- Swagger: `http://localhost:5095/swagger`
- PostgreSQL from host: `localhost:5433`

