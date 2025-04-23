# 🚚 GAC Warehouse Management System (GAC.WMS)

**GAC.WMS** is a modern, extensible, and high-performance **Warehouse Management System** built with **.NET 9**, designed to streamline operations such as order processing, customer handling, and integration with external systems.

---

### 🌟 Features

- 💡 **Extensible & Configurable**: Easily adaptable for new customers.
- ⏰ **CRON-Based Job Scheduling**: Background job execution with customizable frequency.
- 🔁 **Robust Retry Logic**: Automatic retries for failed jobs to ensure data consistency.
- 🚀 **Rate Limiting & Async Support**: Handles high-volume traffic smoothly.
- 🔐 **JWT Authentication & Secure APIs**.

---

## 🧱 Clean Architecture

This solution embraces **Clean Architecture** principles to ensure testability, maintainability, and separation of concerns.

### Layers

1. **Domain** - Core business entities (`Customer`, `Product`, `SaleOrder`) and logic.
2. **Application** - Use cases, services, DTOs, and validation logic.
3. **Infrastructure** - EF Core data access, Hangfire job processing, external system integration.
4. **API** - Web API controllers, middleware, authentication, and request rate-limiting.
5. **Integration Engine** - Handles file-based data ingestion via CRON jobs.

---

## 🗂️ Project Structure

### 📁 GAC.WMS.Application
- Business logic and DTOs
- AutoMapper, FluentValidation, interfaces

### 📁 GAC.WMS.Infrastructure
- File parsing (XML/CSV), job scheduling, services
- EF Core, Hangfire, authentication

### 📁 GAC.WMS.Domain
- Business entities, enums, and custom exceptions

### 📁 GAC.WMS.IntegrationEngine
- Scans folders, dispatches files to appropriate handlers
- Handles success and error scenarios robustly

### 📁 GAC.WMS.Api
- Controllers with async endpoints
- Middleware, JWT authentication, Swagger, rate-limiter

### 📁 GAC.WMS.UnitTests
- MSTest, Moq, and EF Core InMemory testing
- Organized by feature/folder

---

## ⚙️ Prerequisites

- ✅ **Docker**: [Get Docker](https://docs.docker.com/get-docker/)
- ✅ **PowerShell**: [Install PowerShell](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell)

---

## 🛠️ Installation & Setup

1. **Create shared directory**: `C:\SharedFolder`
2. **Add customer folders**: `Customer1`, `Customer2`, etc.
3. **Share folder**: Right-click → Properties → Share with "Everyone"
4. **Copy XML files** from solution's document folder to C:\SharedFolder\Customer1\
5. **Clone the repository**:

```bash
git clone https://github.com/mahureakshay1/GAC.WMS.git
```

6. **Open PowerShell** in the root folder (where dockerdeploy.ps1 preset).
7. **Run deployment script** (Make sure docker is up and running):

```powershell
.\dockerdeploy.ps1
```

   📦 This script will:
   - Build the .NET 9 solution
   - Run unit tests
   - Start SQL Server container & run EF Core migrations
   - Deploy the API
8. Check docker containers **gacwms-db-1** and **gacwms-srver-1** is running or not if not running, run manually.
9. If both containers are running, verify xml files are processed, that are copied in step 4.
   - Go to path "C:\SharedFolder\Customer1\" files should be moved to **Success** folder if processed succusfully or **Error** folder.
   - Logs information is available in **gacwms-srver-1** container log.

---

## 🌐 Access Endpoints

- 🧪 Swagger: `http://localhost:8080/swagger/index.html`
- 🛠️ Hangfire Dashboard: `http://localhost:8080/jobs`
- 🔁 API (Postman, etc.): `http://localhost:8080`
- 📂 Place your XML files under: `C:\SharedFolder\Customer1\`

---

## 🧾 Sample `appsettings.json`

```json
{
  // Application section is use by Cron job for import 
  "Application": {
    "Url": "http://localhost",
    "Port": 8080, // change port number to 44352 if running using visual studio
    "Username": "admin",
    "Password": "admin"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=<SERVER_NAME>;Database=GacWms;User Id=sa;Password=<PASSWORD>;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "OgCOPWBPWideOqs-gmHPqjb6eJkxezLnF0mKZmG-r0o=",
    "Issuer": "localhost",
    "Audience": "localhost",
    "ExpiresInMinutes": 60
  },
  "FileIntegration": {
    "CronExpression": "<CRON_EXPRESSION>",
    "Customers": [
      { "Name": "Customer1", "Path": "/app/shared/Customer1" }, // Change this path \\<SERVER>\SharedFolder\Customer1 to if running on visual studio
      { "Name": "Customer2", "Path": "/app/shared/Customer2" }, // Change this path \\<SERVER>\SharedFolder\Customer1to if running on visual studio
    ]
  }
}
```

---
