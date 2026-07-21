# SalesAI Automation Platform

## Project Overview

SalesAI is an automated, AI-driven sales and lead generation platform designed to capture incoming leads, enrich their profiles, and formulate custom outreach strategies using generative AI. The system operates autonomously upon lead creation, interacting with the Gemini API to score the lead, develop a tailored sales playbook, and draft personalized outreach emails.

## Problem

Traditional B2B sales processes involve significant manual overhead, including lead qualification, company research, and crafting personalized outreach. Sales teams often waste valuable time manually compiling context before the first point of contact. This platform automates the entire qualification and preparation pipeline, ensuring that every captured lead is instantly analyzed, scored, and prepared with actionable outreach material before a human sales representative ever looks at it.

## Key Features

*   **Automated Lead Scoring:** Assigns a numeric score and category (Cold, Warm, Hot) based on lead firmographics and interaction history.
*   **Dynamic Playbook Generation:** Constructs a customized sales strategy, including recommended channels, anticipated objections, and suggested next steps.
*   **Personalized Email Generation:** Drafts highly context-aware outreach emails tailored to the specific lead's industry and position.
*   **Event-Driven Architecture:** Uses RabbitMQ for decoupled, asynchronous processing of background tasks without blocking the main API thread.
*   **Background Job Processing:** Integrates Hangfire for resilient retry mechanisms and long-running job management.
*   **Dual Frontend Interface:** A public-facing web application for capturing lead data (NovaFlow demo), and an internal dashboard for sales representatives to review enriched leads and analytics.

## Technology Stack

*   **Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core (SQL Server)
*   **Frontend:** Angular 17, TypeScript, SCSS
*   **AI Integration:** Google Gemini API (gemini-3.5-flash)
*   **Message Broker:** RabbitMQ
*   **Caching & State:** Redis
*   **Background Jobs:** Hangfire
*   **Containerization:** Docker & Docker Compose

## Architecture Overview

The backend is built adhering to Clean Architecture principles. When a lead is captured via the public web interface, an HTTP request is made to the ASP.NET Core API. The API persists the lead to SQL Server and immediately publishes a `LeadCreatedEvent` to RabbitMQ.

A background consumer listens for this event and sequentially dispatches commands using MediatR to score the lead, generate a playbook, and draft an email via the Gemini API. This heavy processing happens entirely asynchronously. Hangfire provides a dashboard for monitoring these background activities, and Redis is utilized for distributed caching to optimize API response times.

## Project Structure

*   **`src/`**: Contains the .NET backend solution.
    *   **`SalesAI.API/`**: The presentation layer, containing API controllers, dependency injection configuration, and Swagger setup.
    *   **`SalesAI.Application/`**: Contains business logic, CQRS handlers (MediatR), interfaces, and validators.
    *   **`SalesAI.Domain/`**: The core domain model, including entities, value objects, and domain events.
    *   **`SalesAI.Infrastructure/`**: Implementations of infrastructure concerns, including the EF Core DbContext, RabbitMQ publishers, Redis caching, and the Gemini AI service implementation.
*   **`sales-ai.ui/`**: The internal Angular dashboard for sales representatives.
*   **`sales-ai.public-web/`**: The public-facing Angular application utilized for lead capture and demonstration purposes.
*   **`docker-compose.yml`**: Defines the required infrastructure dependencies (SQL Server, Redis, RabbitMQ) for local development.

## Prerequisites

*   .NET 8 SDK
*   Node.js (v18+)
*   Angular CLI (`npm install -g @angular/cli`)
*   Docker Desktop
*   A valid Google Gemini API Key

## Configuration

### Environment Variables & Settings

Update the `appsettings.Development.json` file located in `src/SalesAI.API/` with the required configuration.

### AI Configuration (Gemini API Key)

Provide your Gemini API key in the configuration:

```json
{
  "GeminiAI": {
    "ApiKey": "YOUR_API_KEY_HERE"
  }
}
```

The application is configured to utilize the `gemini-3.5-flash` model.

### Database Setup and Migrations

The application uses Entity Framework Core with SQL Server. Ensure the SQL Server container is running, then apply the migrations:

```bash
cd src/SalesAI.API
dotnet ef database update
```

## Installation and Execution

### Docker Setup

Start the required infrastructure services (SQL Server, Redis, and RabbitMQ):

```bash
docker-compose up -d
```

### How to run the backend

Navigate to the API project and start the server:

```bash
cd src/SalesAI.API
dotnet run
```

The API will be available at `https://localhost:5001` or `http://localhost:5000` depending on your local launch settings.

### API Documentation (Swagger)

When the backend is running in the Development environment, the Swagger UI is available at:
`http://localhost:5000/swagger`

### How to run the frontend(s)

**Internal Dashboard (SalesAI.UI):**
```bash
cd sales-ai.ui
npm install
npm start
```
The application will be accessible at `http://localhost:4200`.

**Public Web Interface (SalesAI.PublicWeb):**
```bash
cd sales-ai.public-web
npm install
npm start -- --port 4201
```
The application will be accessible at `http://localhost:4201`.

## Infrastructure Details

*   **RabbitMQ**: Accessible at `localhost:5672` (AMQP) and `localhost:15672` (Management UI, default credentials: `guest`/`guest`).
*   **Redis**: Accessible at `localhost:6379`.
*   **Hangfire**: The dashboard for background job monitoring is hosted by the API and accessible at `/hangfire`.

## Demo Workflow

1.  Ensure Docker containers, the backend API, and both frontend applications are running.
2.  Navigate to the Public Web interface (`http://localhost:4201`).
3.  Fill out the demonstration capture form to simulate an inbound prospect.
4.  Navigate to the Internal Dashboard (`http://localhost:4200`).
5.  Observe the newly captured lead. The AI analysis (Scoring, Playbook, Email) will initially display as pending.
6.  Wait a few moments as the background RabbitMQ consumers process the lead via the Gemini API.
7.  Refresh the lead view to see the dynamically generated score, sales strategy, and drafted email content.

## Development Notes

The `GeminiAIService` contains built-in deserialization fallbacks. If the Gemini API returns malformed JSON or encounters rate-limiting (e.g., `503 Service Unavailable` on free tiers), the application catches the `JsonException` and gracefully falls back to structured mock data to prevent application failure.

## Future Improvements

*   Implement WebSockets (SignalR) to push real-time AI processing updates to the UI rather than requiring a manual refresh.
*   Integrate email sending functionality to directly dispatch the generated emails via SMTP or SendGrid.
*   Expand integration to a production-grade external CRM (e.g., Salesforce or HubSpot).
