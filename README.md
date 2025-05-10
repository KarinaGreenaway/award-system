# Award Management System

A full-stack web application that leverages AI to streamline the administration and management of awards programs. This system powers the behind-the-scenes workflows—including nomination review, judging rounds, event management, and feedback collection—while a companion mobile app (not included here) handles participant nominations and interactions.

## Tech Stack

- **Backend:** .NET 9 (C#), MVC, AutoMapper, custom API response wrapper  
- **Database:** PostgreSQL  
- **Frontend:** React 19 (Vite), Tailwind CSS, shadcn/ui  
- **File Storage:** Azure Blob Storage (file & image uploads)  
- **AI Integration:** Google Vertex AI (nomination analysis & feedback summarization)  

## Key Features

- **Nomination Management**  
  - Submit, review, and track nominations  
  - AI-powered summarization of nomination text  

- **Category & Question Configuration**  
  - Create and manage award categories  
  - Define custom nomination and feedback questions  

- **Event & RSVP Handling**  
  - Schedule award events  
  - Create RSVPs and post-event feedback forms  

- **Judging & Process Workflow**  
  - Configure multi-round judging processes  
  - Assign category sponsors  

- **Feedback Aggregation**  
  - Gather and summarize attendee feedback  
  - AI-driven insights for organizers  

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)  
- [Node.js](https://nodejs.org/) (v16+ recommended) + npm or yarn  
- PostgreSQL (v12+)  
- Azure Storage account & connection string  
- Google Cloud service account JSON key with Vertex AI enabled  

### 1. Clone the repository

```bash
git clone https://github.com/your-org/award-management-system.git
cd award-management-system
```

### 2. Configure the backend

#### a) appsettings.Development.json

Create or update `backend/appsettings.Development.json` with your database and Azure Blob settings:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=<HOST>;Database=<DB>;Username=<USER>;Password=<PASS>"
  },
  "AzureBlob": {
    "ConnectionString": "DefaultEndpointsProtocol=...;AccountName=...;AccountKey=...;EndpointSuffix=...",
    "ContainerName": "<your-container-name>"
  }
}
```

#### b) User-secrets for Vertex AI & Google credentials

From the `backend` folder, initialize (if not already) and set your secrets:

```bash
dotnet user-secrets init

dotnet user-secrets set "VertexAi:ProjectId"   "<your-gcp-project-id>"
dotnet user-secrets set "VertexAi:LocationId"  "<your-gcp-region>"
dotnet user-secrets set "VertexAi:ModelId"     "<your-model-id>"
dotnet user-secrets set "VertexAi:ApiEndpoint" "<region>-aiplatform.googleapis.com"
dotnet user-secrets set "Google:CredentialsPath" "C:/Users/Karin/WebstormProjects/award-system-github/backend-api/Credentials/GoogleSettings.json"
```

You can verify with:

```bash
dotnet user-secrets list
```

### 3. Initialize the database
Run your existing SQL scripts directly:

```bash
# 1. Create the database (if it doesn't exist)
createdb

# 2. Run the schema script
psql -f ./award-system-db-script.sql

# 3. Run all test-data scripts
for f in ./test-data-scripts/*.sql; do
  psql -f "$f"
done
```

### 4. Run the backend

```bash
cd backend
dotnet restore
dotnet build
dotnet run
# Will run on http://localhost:5188 (HTTP)
```

### 5. Run the frontend

```bash
cd ../frontend
npm install      # or yarn install
npm run dev
# Vite will serve at http://localhost:5173 (or the port shown in the console)
```

---

Thank you for checking out this application!  
— KG  
