You are a senior software architect and full-stack engineer.

I want to build a production-grade system called "DevInsight — Engineering Intelligence Platform".

This system must be designed as a real-world SaaS platform that analyzes GitHub repositories, processes source code using AST, and generates actionable engineering insights.

⚠️ Important constraints:

- Do NOT build a simple demo or chatbot
- Follow Domain-Driven Design (DDD) and Clean Architecture
- Use modular, scalable architecture
- Respect separation of concerns strictly

---

## 🧱 Project Structure (Monorepo)

The system must be structured as a monorepo with three main folders:

devinsight/
├── devinsight-backend/ (ASP.NET Core Web API)
├── devinsight-ai-engine/ (Python FastAPI)
├── devinsight-frontend/ (React + Vite)
├── docker-compose.yml
└── README.md

---

## 🧠 Backend Requirements (.NET)

The backend must follow my existing DDD structure with bounded contexts.

### Existing Context:

- IdentityAccessManagement (already implemented)

### New Context:

- EngineeringIntelligence (core of the system)

---

## 📦 EngineeringIntelligence Structure

Follow this structure strictly:

### Application Layer

- CommandServices
- QueryServices
- OutboundServices (interfaces for external integrations)
- Pipelines (for orchestration)

Examples:

- AnalyzeRepositoryCommandService
- ICodeAnalyzerService
- IGitHubService
- IAIInsightService
- AnalysisPipeline

---

### Domain Layer

- Aggregates:
  - Repository
  - Analysis
  - Insight

- Commands:
  - AnalyzeRepositoryCommand

- Queries:
  - GetAnalysisByRepositoryIdQuery

- Repositories:
  - IRepositoryRepository
  - IAnalysisRepository

---

### Infrastructure Layer

- GitHub integration (OAuth + API)
- Repository cloning service
- Code Analyzer using AST (Roslyn for C#)
- AI integration service (calls Python AI Engine)
- Persistence (Entity Framework Core)

---

### Interfaces Layer

- REST Controllers
- Resources (DTOs)
- Assemblers (Transformations)

---

## 🔍 Code Analysis Requirements

- MUST use AST (Roslyn for C#)
- Detect:
  - Long methods (>50 lines)
  - Deep nesting (>3 levels)
  - Large files (>300 lines)

- Return structured JSON (NOT plain text)

---

## 🤖 AI Engine (Python + FastAPI)

Responsibilities:

- Receive structured analysis results
- Generate:
  - Explanation of issues
  - Suggested improvements
  - Risk assessment

---

## 🔗 GitHub Integration

Use OAuth (NOT scraping or browser automation)

The system must:

- Authenticate users via GitHub
- Access repositories
- Clone repositories locally for analysis

---

## 🖥️ Frontend (React)

- Dashboard for repositories
- View analysis results
- Copilot-like interface for questions

---

## 🔁 System Flow

1. User logs in via GitHub
2. Selects a repository
3. Backend clones repository
4. Code Analyzer processes files
5. Results are sent to AI Engine
6. Insights are returned and stored
7. Frontend displays results

---

## 📦 Deliverables

1. Full backend folder structure (DDD)
2. Domain entities
3. API endpoints
4. GitHub OAuth implementation
5. Repository cloning logic
6. AST-based analyzer (Roslyn)
7. JSON output format
8. AI Engine communication
9. Sample request/response

---

## 🧠 Additional Requirements

- Use dependency injection properly
- Implement error handling and logging
- Keep code clean and maintainable
- Design for extensibility (multi-language future)
- Avoid overengineering but maintain scalability

---

## 🎯 Output format

Respond with:

- Architecture explanation
- Folder structure
- Code snippets
- Step-by-step implementation plan

Do NOT skip steps.
Act like you are building this for a real company.
