# SalesAI Copilot

**SalesAI Copilot** is a modern, AI-first Sales Automation Platform built with .NET 10 and Angular 19. It goes beyond traditional CRMs by actively helping sales teams qualify leads, automate repetitive tasks, generate personalized outreach, and extract actionable insights through integrated AI capabilities (Google Gemini).

## Features

*   **Lead & Deal Management:** Intuitive CRUD operations, interactive Kanban boards, and data-rich list views.
*   **AI Copilot Integration:**
    *   **AI Lead Scoring:** Automatically score and categorize leads (Hot, Warm, Cold) based on behavior and profile.
    *   **Sales Playbook:** Generates tailored sales strategies, suggested channels, and approaches.
    *   **AI Email Generation:** Draft personalized outreach emails dynamically based on lead context and selected tone.
*   **Analytics & Reporting:** Real-time dashboards, pipeline funnels, win rate calculations, and AI-driven growth insights.
*   **Task & Calendar Management:** Keep track of calls, emails, meetings, and follow-ups.
*   **Background Automation:** Hangfire integration for automated lead assignment and follow-up reminders.

## Tech Stack

*   **Backend:** .NET 10, ASP.NET Core Web API, Clean Architecture, MediatR, CQRS, Entity Framework Core, SQL Server, Hangfire, Serilog.
*   **AI Integration:** Google Gemini API (`Microsoft.SemanticKernel` or direct REST).
*   **Frontend:** Angular 19, Tailwind CSS v4, Angular Material Components.
*   **Deployment:** Docker & Docker Compose ready.

## Getting Started

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/OmarElzarka/AI-sales-automation-platform.git
    cd AI-sales-automation-platform
    ```

2.  **Configure API Keys:**
    Set up your Gemini API Key via .NET User Secrets in the API project:
    ```bash
    cd src/SalesAI.API
    dotnet user-secrets init
    dotnet user-secrets set "Gemini:ApiKey" "YOUR_API_KEY_HERE"
    ```

3.  **Run with Docker Compose:**
    ```bash
    docker-compose up -d
    ```
    This will spin up SQL Server and run the migrations automatically.

4.  **Run the Frontend:**
    ```bash
    cd src/SalesAI.UI
    npm install
    npm run start
    ```

5.  Navigate to `http://localhost:4200` to access the SalesAI Copilot dashboard.

## Project Structure

*   `SalesAI.Domain`: Enterprise entities, enums, exceptions, and interfaces.
*   `SalesAI.Application`: Use cases, CQRS Handlers, DTOs, and Validators.
*   `SalesAI.Infrastructure`: Database context, Hangfire configuration, Gemini AI service implementations.
*   `SalesAI.API`: Controllers, Middlewares, and Program configuration.
*   `SalesAI.UI`: Angular 19 SPA powered by Tailwind CSS.

---
*Built with ❤️ and AI.*