# Job Application Management System

A backend REST API for managing job postings and candidate applications.

## Features

* User Registration & Login
* Job Creation and Management
* Job Applications
* Application Cancellation
* JWT Authentication
* Role-Based Authorization
* Background Notifications using Hangfire

## Technologies

* C#
* ASP.NET Core Web API
* Clean Architecture
* CQRS & MediatR
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT
* Hangfire
* Scalar / OpenAPI

## Architecture

The project follows **Clean Architecture** with four layers:

* **Domain** – Entities and business rules
* **Application** – CQRS, MediatR, DTOs, and interfaces
* **Infrastructure** – Database, repositories, Identity, and services
* **API** – Controllers, authentication, and API configuration

## Background Jobs

**Hangfire** is used to process background notification jobs for events such as:

* Closing a job
* Cancelling an application

## API Documentation

The API is documented using **Scalar / OpenAPI**.

## Getting Started

1. Clone the repository.
2. Configure the SQL Server connection string in `appsettings.json`.
3. Configure the JWT settings.
4. Apply the Entity Framework Core migrations.
5. Run the application.
6. Open the Scalar API documentation.

## Project Structure

```text
JobApplication
│
├── JobApplication.API
├── JobApplication.Application
├── JobApplication.Domain
└── JobApplication.Infrastructure
```
