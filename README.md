# Debugged — Bug Knowledge Base API

A personal bug knowledge base built as an ASP.NET Core Web API.
Instead of deleting resolved bugs, they are archived with their solution
and made searchable. When a developer logs a new issue, the API checks
for similar resolved issues and surfaces them — turning past mistakes
into a learning resource.

> **Status:** 🚧 Under active development. School project!
---

## Core Concept

The standout feature is **Similar Issues matching**. When you log a new
issue with tags and an error message, the API searches your archived
issues and returns the closest matches — so you instantly see "you've
solved this kind of problem before, here's how."

Matching is based on:
- Shared tags (many-to-many)
- Error message keyword search

---

## Tech Stack

- **.NET 10** — ASP.NET Core Web API
- **Clean Architecture** — 4 layers (API, Application, Domain, Infrastructure)
- **CQRS** with **MediatR** — separated read/write paths
- **Entity Framework Core** with SQL Server
- **AutoMapper** — entity ↔ DTO mapping
- **FluentValidation** — request validation via MediatR pipeline behavior
- **JWT Bearer authentication** with role-based access control (Admin / User)
- **Swagger / OpenAPI** — interactive API docs

---

## Project Structure

\`\`\`
src/
├── Debugged.API/             # Controllers, Program.cs, middleware
├── Debugged.Application/     # Commands, Queries, Handlers, DTOs, Validators
├── Debugged.Domain/          # Entities, Enums, domain interfaces
└── Debugged.Infrastructure/  # DbContext, Repositories, Migrations, JWT
\`\`\`

Dependencies point inward toward \`Domain\`. \`Domain\` depends on nothing.

---

## Status

- [x] Solution and Clean Architecture project structure
- [x] CI pipeline (GitHub Actions)
- [x] Branch protection on \`main\`
- [ ] Domain layer — entities and enums
- [ ] Infrastructure — DbContext, configurations, migrations
- [ ] Application — MediatR, AutoMapper, FluentValidation
- [ ] CRUD endpoints (Projects, Issues, Tags)
- [ ] Similar Issues query
- [ ] JWT authentication + RBAC
- [ ] Seed data and demo
- [ ] Final documentation

---

## Running Locally

> Documentation will be added once the project is runnable.

