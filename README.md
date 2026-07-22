# SalesAI Platform

## Project Overview
SalesAI is an automated sales pipeline and CRM platform designed to streamline the lead management and conversion process. The system manages leads, tasks, and deals, and incorporates automated background processing to conduct company research, score inbound leads, and draft contextual email outreach without manual intervention.

## Problem Statement
Sales teams dedicate a significant portion of their time to administrative tasks, including manual data entry, lead qualification, and drafting initial outreach emails. SalesAI automates the research and qualification phases by evaluating inbound leads using artificial intelligence, generating company research dossiers, providing actionable playbooks, and drafting personalized outreach based on real-time data.

## Key Features
- **Lead Management**: Track and manage leads through their lifecycle from inbound to qualified.
- **Automated Workflow Automation**: Submitting inbound requests automatically triggers background workflows for research and evaluation.
- **AI Lead Scoring**: Evaluates leads based on their profile, company size, and intent signals.
- **AI Company Research**: Utilizes Wigolo to gather competitive intelligence and context before meetings.
- **AI Email Drafting**: Generates contextually aware emails based on the lead's profile and company data.
- **Deal Tracking**: Kanban-style pipeline management for tracking deals across pipeline stages.
- **Meeting Summarization**: Extracts key discussion points and action items from transcripts.

## Technology Stack
- **Backend**: .NET 10 (C# 14), ASP.NET Core Web API
- **Frontend**: Angular 19, Tailwind CSS
- **Database**: Entity Framework Core with SQL Server
- **Architecture**: Clean Architecture with CQRS (MediatR)
- **AI Integrations**: Google Gemini API, Wigolo Service
- **Background Processing**: Hangfire
- **Caching**: Redis
- **Messaging**: RabbitMQ

## High-Level Architecture
The application follows a Clean Architecture design pattern:
- **Domain**: Contains core enterprise entities, enums, and domain events (e.g., LeadCreatedEvent).
- **Application**: Contains business logic, MediatR handlers for queries and commands, and system interfaces.
- **Infrastructure**: Contains implementation details for external concerns such as database contexts (EF Core), AI service integrations (Gemini, Wigolo), background job scheduling, and message brokers.
- **API**: The entry point for the backend, exposing RESTful endpoints.

The frontend consists of Angular applications communicating with the API via standard HTTP requests secured with JWT-based authentication.

## Project Structure
- src/SalesAI.API: ASP.NET Core Web API entry point. Contains controllers and middleware.
- src/SalesAI.Application: CQRS commands, queries, and validation logic.
- src/SalesAI.Domain: Core entities, enums, and domain events.
- src/SalesAI.Infrastructure: Implementations for external services (EF Core, Gemini AI, Wigolo, Hangfire).
- src/SalesAI.UI: The primary Angular 19 frontend application for the sales dashboard.

## Prerequisites
- .NET 10 SDK
- Node.js (v20 or later)
- Angular CLI (v19)
- Docker Desktop (for backing services)
- Google Gemini API Key

## Installation
1. Clone the repository:
   `ash
   git clone https://github.com/KnockOutEZ/wigolo.git
   `
2. Install frontend dependencies:
   `ash
   cd src/SalesAI.UI
   npm install
   `
3. Restore backend dependencies:
   `ash
   cd src/SalesAI.API
   dotnet restore
   `

## Environment Configuration
The system requires configuration for the Gemini API. Use .NET User Secrets to securely store the key in the backend environment:
`ash
cd src/SalesAI.API
dotnet user-secrets init
dotnet user-secrets set "GeminiApi:ApiKey" "YOUR_API_KEY_HERE"
`

For development, ensure appsettings.Development.json is correctly configured:
- JwtSettings:Secret - Secret key for generating JWTs (min 32 characters).
- MessageBroker - Connection details for RabbitMQ.
- ConnectionStrings - Connection strings for SQL Server and Redis.

## Running the Backend
From the src/SalesAI.API directory:
`ash
dotnet run
`
The API will be available at http://localhost:8080.

## Running the Frontend(s)
To start the internal SalesAI dashboard, navigate to src/SalesAI.UI:
`ash
npm start
`
The application will be available at http://localhost:4200.

## Database Setup and Migrations
The project utilizes Entity Framework Core. To apply the latest migrations to the SQL Server database:
`ash
cd src/SalesAI.Infrastructure
dotnet ef database update --startup-project ../SalesAI.API
`

## RabbitMQ Configuration
RabbitMQ handles asynchronous domain events, such as triggering background research when a LeadCreatedEvent is published. By default, it connects to localhost on the standard AMQP port using guest/guest credentials. Configure connection details in the MessageBroker section of appsettings.json.

## Redis Configuration
Redis is utilized for distributed caching to optimize query performance. Ensure a local instance is running or configure the ConnectionStrings:Redis setting in appsettings.json to point to a valid Redis endpoint.

## Hangfire
Hangfire manages durable background jobs and scheduled tasks. It relies on the SQL Server database for job storage. The Hangfire dashboard is available at /hangfire when running in the Development environment.

## Gemini AI Configuration
The application integrates with the Google Gemini API (model: gemini-2.0-flash) for content generation and lead scoring. If the API returns rate-limit errors (429), the application gracefully falls back to mock data responses to prevent pipeline disruption in demonstration environments. Ensure GeminiApi:ApiKey is set as documented in the Environment Configuration section.

## Docker
Backing services (SQL Server, Redis, RabbitMQ) can be spun up using the provided docker-compose.yml file:
`ash
docker-compose up -d
`

## API Documentation (Swagger)
When running in the Development environment, Swagger UI is available at:
http://localhost:8080/swagger
This interface provides interactive documentation and testing for all REST endpoints.

## End-to-End Workflow
1. Start the backend, frontend, and backing services via Docker.
2. Submit a lead request.
3. The backend receives the request, creates the lead, and publishes a LeadCreatedEvent to RabbitMQ.
4. The LeadCreatedConsumer processes the event asynchronously:
   - Triggers the WigoloService to execute competitive intelligence research on the lead and company.
   - Triggers the GeminiAIService to draft a personalized email based on the research.
5. Log into the SalesAI dashboard (http://localhost:4200).
6. Navigate to the Leads section and open the newly created lead to view the generated AI Lead Score, Competitive Intelligence, and Drafted Email.

## Development Notes
- The project enforces .NET 10 and Angular 19. Do not downgrade these versions.
- The UI leverages Tailwind CSS with a standardized component library. Adhere to the existing design system for new components.
- The GeminiAIService is configured with a fallback mechanism; ensure any modifications to the JSON response structures are reflected in the fallback implementations.

## License
MIT License
