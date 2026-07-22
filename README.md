# Canopy

A collaborative project management platform built with ASP.NET Core, designed to help teams organize projects, manage tasks, and collaborate in one place.

> Currently under active development.

## Overview

Canopy is a full-stack project management application focused on team collaboration and productivity.
Canopy was created to explore real-world project management workflows, including team collaboration, task organization, and productivity tools.

The platform allows users to create groups, manage projects, assign tasks, and organize their workflow. The application is built using ASP.NET Core, with a structured separation between application logic, data access, and user interface components.

The goal of Canopy is to provide a flexible workspace where teams can plan, track, and collaborate on projects efficiently.

## Features

### Implemented

- User registration and authentication
- JWT-based authentication
- Secure HTTP-only cookie handling
- Remember me functionality
- Group creation and member management
- Project management
- Task creation and assignment
- Task status tracking
- Multi-language support (Persian & English)
- Database management using Entity Framework Core

### In Development

- Real-time group chat using SignalR
- Calendar and scheduling features
- Mindmap-based project planning
- Dashboard with project and task statistics
- User profile management

## Screenshots

_Coming soon_

Dashboard and application screenshots will be added after completing the main UI components.

## Tech Stack

### Backend

- ASP.NET Core (.NET 8)
- Entity Framework Core (Code First)
- SQL Server
- JWT Authentication
- RESTful API design

### Frontend

- Bootstrap
- Metronic 8
- Jquery

### Other

- Localization (i18n)
- Database migrations
- Responsive dashboard UI using Metronic


## Architecture

Canopy follows a client-server architecture:
```text
Frontend
    |
    |
ASP.NET Core API
    |
    |
Entity Framework Core
    |
    |
SQL Server
```

The API handles authentication, business logic, and data management while the frontend focuses on user interaction and application experience.

## Running the Project

### Requirements

- .NET 8 SDK
- SQL Server
- Visual Studio 2022 (recommended)

### Setup

1. Clone the repository
2. Update the database connection string in: appsettings.json
3. Run database migrations:
      Add-Migration InitialCreate
      Update-Database
4. Run the project
   
## Highlights
### Some of the key technical aspects of Canopy include:

- Building a RESTful application using ASP.NET Core
- Designing database relationships using Entity Framework Core Code First
- Implementing authentication and authorization flows
- Managing sessions securely with HTTP-only cookies
- Supporting multiple languages through localization
- Designing features around real-world team collaboration workflows

## Roadmap
- [x] Authentication system
- [x] User management
- [x] Groups and projects
- [x] Task management
- [ ] Real-time chat
- [ ] Calendar system
- [ ] Mindmap planning tools
- [ ] Dashboard analytics

## Status
Canopy is actively being developed and new features are continuously being added.
