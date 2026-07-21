# SalesAI

## Project Overview
SalesAI is an automated sales pipeline and CRM platform that integrates artificial intelligence to streamline the sales process. It manages leads, tasks, and deals, and leverages AI to score leads, research companies, summarize meetings, and generate contextual email outreach. 

## Problem the Project Solves
Modern sales teams spend a significant amount of time on manual data entry, lead qualification, and drafting emails. SalesAI automates these repetitive tasks by evaluating inbound leads using AI, automatically generating company research dossiers, providing actionable next steps (playbooks), and composing personalized outreach based on real data, allowing sales representatives to focus on building relationships and closing deals.

## Key Features
- **Lead Management**: Track and manage leads through their lifecycle from inbound to qualified.
- **AI Lead Scoring**: Automatically scores leads based on their profile, company size, and intent.
- **AI Company Research**: Generates summaries of a lead's company to provide context before meetings.
- **AI Playbook Generation**: Suggests specific engagement strategies and next steps for leads.
- **AI Email Composer**: Drafts contextual emails based on the lead's profile and desired tone.
- **Deal Tracking**: Kanban-style pipeline management for tracking deals across stages.
- **Meeting Summarization**: Extracts key points and action items from meeting transcripts.
- **Tasks and Calendar**: Manages upcoming tasks, calls, and meetings.
- **Global Search**: Search across leads and deals using a centralized endpoint.
- **Dashboard and Reports**: Visualizes KPIs, pipeline funnels, and exports data.

## Technology Stack
- **Backend**: .NET 10 (C# 14), ASP.NET Core Web API
- **Frontend**: Angular 19, Tailwind CSS
- **Database**: Entity Framework Core (In-Memory for demonstration purposes, easily swappable to SQL Server/PostgreSQL)
- **Architecture**: Clean Architecture with CQRS (MediatR)
- **AI Provider**: Google Gemini API
- **Background Processing**: Hangfire (configured for background jobs)
- **Caching**: Redis (simulated/configurable via ICacheService)
- **Messaging**: RabbitMQ (simulated for demonstration purposes)

## Architecture Overview
The application follows a Clean Architecture approach:
- **Domain**: Contains enterprise logic and entities.
- **Application**: Contains business logic, MediatR handlers, and interfaces.
- **Infrastructure**: Contains external concerns such as database context, AI service integrations, and third-party APIs.
- **API**: The entry point for the backend, exposing REST endpoints.

The frontend is a standalone Angular application communicating with the API via standard HTTP requests with JWT-based authentication.

## Project Structure
- `src/SalesAI.API`: The ASP.NET Core Web API entry point. Contains controllers and middleware.
- `src/SalesAI.Application`: CQRS commands, queries, and validation logic.
- `src/SalesAI.Domain`: Core entities and enums.
- `src/SalesAI.Infrastructure`: Implementation of external services (EF Core, Gemini AI, Hangfire).
- `src/SalesAI.UI`: The Angular 19 frontend application.

## Prerequisites
- .NET 10 SDK
- Node.js (v18 or later)
- Angular CLI (v19)
- Google Gemini API Key

## Installation
1. Clone the repository.
2. Navigate to the frontend directory (`src/SalesAI.UI`) and run `npm install`.
3. Navigate to the backend directory (`src/SalesAI.API`) and run `dotnet restore`.

## Configuration
### AI Configuration (Gemini API key)
The system uses Google Gemini for its AI capabilities. You must configure your API key in the backend. 
Use .NET User Secrets to securely store the key:
```bash
cd src/SalesAI.API
dotnet user-secrets init
dotnet user-secrets set "Gemini:ApiKey" "YOUR_API_KEY_HERE"
```

### Environment Variables
For development, you can also set the following environment variables or modify `appsettings.Development.json`:
- `Jwt:Key` - The secret key used for generating JWTs.
- `Jwt:Issuer` - The issuer of the JWT.
- `Jwt:Audience` - The audience of the JWT.

## How to run the backend
From the root directory or `src/SalesAI.API`:
```bash
cd src/SalesAI.API
dotnet run
```
The API will be available at `http://localhost:8080`.

## How to run the frontend
From the `src/SalesAI.UI` directory:
```bash
cd src/SalesAI.UI
npm start
```
The frontend will be available at `http://localhost:4201`.

## Docker setup
Currently, the application runs natively. Future updates will include `Dockerfile` and `docker-compose.yml` for containerized deployment of the API, Angular app, Redis, and RabbitMQ.

## Database setup and migrations
The project currently uses an In-Memory database (`SalesAIDb`) for demonstration purposes, so no initial migrations are required to run the project. To switch to a persistent store (e.g., SQL Server), update the `DbContext` configuration in `DependencyInjection.cs` and run standard EF Core migrations.

## RabbitMQ
Messaging is abstracted via the `IMessagePublisher` interface. The current implementation provides a direct simulation for demonstration. A RabbitMQ provider can be implemented and injected to scale message processing.

## Redis
Caching is abstracted via the `ICacheService` interface. The current implementation uses an in-memory dictionary. A Redis provider can be plugged in by registering `StackExchange.Redis` in the infrastructure layer.

## Hangfire
Hangfire is configured and registered in the infrastructure layer to handle background tasks and scheduled jobs (e.g., syncing data, recurring email sequences). The dashboard is not exposed by default but can be enabled in `Program.cs`.

## API documentation (Swagger)
When running in the Development environment, Swagger UI is available at:
`http://localhost:8080/swagger`
This provides interactive documentation and testing for all REST endpoints.

## Demo workflow
1. Start the backend and frontend.
2. Open the frontend and log in with any credentials (authentication is simulated for the demo).
3. Navigate to the **Leads** page and click "New Lead" to create a lead.
4. Click on the lead to open the details page and click **AI Score** or **AI Research** to see the Gemini integration in action.
5. Create a Deal in the **Deals** pipeline and test the meeting summarization feature.
6. Use the **Outreach** tab to generate an AI-composed email.
7. Use the global search bar to quickly find leads or deals.

## Development notes
- The project enforces .NET 10 and Angular 19. Do not downgrade these versions.
- The UI leverages Tailwind CSS with a custom color palette. Adhere to the existing design system for any new components.

## Future improvements
- Implement persistent SQL Server database via EF Core.
- Add complete Docker Compose environment (SQL Server, Redis, RabbitMQ).
- Implement WebSockets for real-time notifications when AI tasks complete.
- Expand tests coverage (xUnit for backend, Jasmine/Karma for frontend).

## License
MIT License
