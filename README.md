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
- Real-time group chat using SignalR
- Dashboard with project and task statistics
- User profile management

### In Development

- Calendar and scheduling features
- Mindmap-based project planning

## Screenshots

### Dashboard
![Canopy Dashboard](Screenshots/dashboard.png)

### Project Management
![Canopy Project Management](Screenshots/projects.png)

### Calendar
![Canopy Calendar](Screenshots/calendar.png)

### Group Chat
![Canopy Group Chat](Screenshots/chat.png)



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

flowchart TD
    A["Frontend<br/>Bootstrap · Metronic · jQuery"]
    B["ASP.NET Core Web API<br/>Authentication · Business Logic · REST Endpoints"]
    C["Entity Framework Core<br/>Data Access · ORM"]
    D[("SQL Server<br/>Database")]

    A <-->|"HTTP Requests / Responses"| B
    B <--> C
    C <--> D

Canopy uses a client-server architecture in which the frontend communicates with an ASP.NET Core Web API through HTTP requests.

The API handles authentication, application logic, and RESTful endpoints. Entity Framework Core manages data access and database operations, with SQL Server serving as the application's persistent data store.

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
- [x] Real-time chat
- [x] Reminders
- [x] Email Verification 
- [x] Calendar system
- [x] Dashboard analytics
- [ ] Mindmap planning tools
- [ ] Mobile App
- [ ] Group board

## Status
Canopy is actively being developed and new features are continuously being added.
