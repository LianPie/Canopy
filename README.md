# Canopy - Task Management API

A task management backend built with ASP.NET Core Web API.

## About
Personal task management system with JWT authentication. Create, read, update, and delete tasks.
Part of a larger project management application (groups, projects, chat, mindmap — coming soon).

## Current Features (MVP)
- User registration and login with JWT
- Remember me and session-based login
- Personal task CRUD (Create, Read, Update, Delete)
- Multi-language support (Persian & English)
- Secure HTTP-only cookies

## Technologies
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core (Code First)
- SQL Server
- JWT Authentication
- Cookie-based sessions
- Localization (i18n)


## How to Run
1. Clone the repo
2. Open in Visual Studio
3. Update connection string in `appsettings.json`
4. Run Package Manager Console:
      Add-Migration InitialCreate
      Update-Database
5. Press F5


## Roadmap
- [ ] Projects and groups
- [ ] SignalR group chat
- [ ] Calendar and mindmap views (frontend)
- [ ] Full i18n

## Status
Actively developing next features.
