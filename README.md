# 🚀 Task Management API (.NET 10 & SQL Server)

A robust, production-ready Task Management API built with **Minimal APIs** and **Entity Framework Core**. This project demonstrates a containerized architecture using **Docker Compose** to orchestrate a C# backend and a Microsoft SQL Server database.

## 🛠 Tech Stack

- **Framework:** .NET 10 (C# 14)
- **Database:** Microsoft SQL Server 2022
- **ORM:** Entity Framework Core (SQL Server Provider)
- **Containerization:** Docker & Docker Compose
- **Testing:** xUnit with FluentAssertions
- **Safety:** Global Exception Handling & Problem Details

## 🏗 Project Architecture

The solution is organized into a professional "Vertical Slice" structure to ensure clear separation of concerns:

- `TaskApi/`: The core API project containing models, database context, and middleware.
- `TaskApi.Tests/`: Unit test suite utilizing xUnit and FluentAssertions to verify business logic.
- `Infrastructure/`: Containerized SQL Server 2022 instance.

## 🚀 Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.

### Installation & Execution

1.  **Clone the repository:**

    ```bash
    git clone [https://github.com/yourusername/c-sharp-task-management-api.git](https://github.com/yourusername/c-sharp-task-management-api.git)
    cd c-sharp-task-management-api
    ```

2.  **Spin up the environment:**
    This command builds the API image, pulls SQL Server, and handles all database migrations and seeding automatically.

    ```bash
    docker-compose up --build
    ```

3.  **Verify the API:**
    Once the logs show `Application started`, the API is available at:
    `GET http://localhost:5000/tasks`

## 🧪 Running Tests

To run the automated test suite locally, ensure you have the .NET 10 SDK installed and run:

```bash
dotnet test
```
