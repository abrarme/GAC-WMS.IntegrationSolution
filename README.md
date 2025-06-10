# 📦 GAC_WMS Integration Solution – Technical Documentation

## ✅ Project Purpose

To integrate **external customer ERP systems** with **GAC’s Warehouse Management System (WMS)** through:

- 🔄 **Real-time RESTful API ingestion**
- 📂 **Scheduled file-based polling from legacy ERP systems**
- ✅ **Data transformation, validation, persistence, and push to WMS**

## 🏗️ High-Level Architecture

```
+-------------------+           +--------------------+
| External ERP APIs | <--POST-- |  Real-Time API     |
+-------------------+           |  (.NET Web API)     |
                                +---------+----------+
                                          |
                                          v
+-------------------+           +---------+----------+         +-----------------+
| File Polling Jobs | --------> | File Processor     | -----> | IWmsClient      |
| (Quartz Scheduler)|           | (CSV/XML/JSON)     |        | (REST Push)     |
+-------------------+           +---------+----------+         +--------+--------+
                                          |                            |
                                          v                            v
                                +--------------------+        +-------------------+
                                | EF Core Repository  | ----> | GAC WMS API       |
                                +--------------------+        +-------------------+
                                          |
                                          v
                                +--------------------+
                                | SQL Server / MySQL |
                                +--------------------+
```

## 🧱 Project Modules

### 1. Real-Time REST API
- Built with **ASP.NET Core Web API**
- Handles POST requests for:
  - `/api/customers`
  - `/api/products`
  - `/api/purchaseorders`
  - `/api/salesorders`
- Validates and persists data to SQL DB using EF Core

### 2. File-Based Polling
- Uses **Quartz.NET Scheduler**
- Configurable via CRON expressions
- Polls file system (shared folder or SFTP in extended version)
- Detects file type (CSV, JSON, XML)
- Passes file to `IFileProcessorFactory`

### 3. File Processor
- Implements strategy pattern:
  - `CsvFileProcessor`
  - `JsonFileProcessor`
  - `XmlFileProcessor`
- Parses file, validates entities
- Calls **WMS Client** for POST to WMS APIs

### 4. WMS Client
- HTTP Client abstraction using `IHttpClientFactory`
- Sends data in JSON to GAC’s WMS APIs
- Implements retry logic for transient failures

### 5. Persistence Layer
- Uses **Entity Framework Core**
- Repositories for `Customer`, `Product`, `PurchaseOrder`, and `SalesOrder`
- Supports **bulk insert with duplicate filtering**
- Logs duplicates using a logging provider (e.g., Serilog)


### 6. Retry Strategy with Polly
  -  Retry logic applied to all outbound WMS API calls:
 ```bash
.AddPolicyHandler(Policy
  .Handle<HttpRequestException>()
  .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt))));
```

### 7. Swagger Documentation
-  Swagger UI is auto-generated with Swashbuckle
-  Lists all endpoints and validation rules
-  Available at /swagger/index.html

## ⚙️ Technical Stack

| Area                    | Tech |
|-------------------------|------|
| Language                | C# (.NET 9) |
| Web Framework           | ASP.NET Core Web API |
| Database                | SQL Server / MySQL |
| ORM                     | Entity Framework Core |
| Scheduler               | Quartz.NET |
| File Parser             | CsvHelper, System.Text.Json, System.Xml |
| HTTP Client             | IHttpClientFactory with RetryPolicyHandler |
| Testing                 | xUnit, Moq |
| Logging                 | Serilog |
| Docs                    | Markdown |
| API Docs	              |Swagger / Swashbuckle|
| Retry	                  |	Polly|
| Validator	              |	FluentValidation|
| Notification	          |	SMTP (SmtpClient)|
| Containerization	      |	Docker (for packaging, building, and deploying the application)|
| JWT Bearer Authentication	      |	Secure token-based authentication for API access)|
	

## 📁 Directory Structure

```
GAC_WMS.IntegrationSolution/
├── Controllers/
├── Jobs/
├── Models/
├── Services/
├── Repositories/
├── Validators/
├── Data/
├── Utils/
├── Program.cs
├── appsettings.json

```

## 🔄 Data Flow Summary

1. **Real-time**
   - API receives request
   - Validates with FluentValidation
   - Saves to DB
   - Pushes to WMS

2. **File-based**
   - Quartz triggers job via CRON
   - Files read from folder
   - Processor parses and transforms
   - Inserts unique records to DB
   - Pushes list to WMS endpoint

## 🧪 Unit Testing

- Test file processors for all formats
- Validate data before insert
- Mock WMS client to test retry logic
- Unit tests for repository logic with in-memory DB

## 🔐 Security

- JWT Bearer Authentication
- Secure endpoints with token-based access
- Auth controller generates and returns tokens

## 📦 DevOps & Deployment

- Docker + Docker Compose - Containerized services: API, SQL Server
- Health Checks - SQL Server startup validated before API runs


## 📝 Local Development Setup

### Prerequisites
- .NET 9 SDK
- SQL Server or MySQL
- Visual Studio or VS Code

### Steps

```bash
git clone https://github.com/your-repo/GAC_WMS.IntegrationSolution.git
cd GAC_WMS.IntegrationSolution
dotnet restore
dotnet ef database update
dotnet run
```

- API available at `https://localhost:5001/api/products`
- Test CRON polling by placing files into the configured folder

## ⏱️ CRON Job Configuration

```json
"FilePolling": {
  "Directories": [
    {
      "Path": "C:\\LegacyFiles\\Products",
      "Endpoint": "https://localhost:44310/api/products/bulk"
    }
  ],
  "ArchiveDirectory": "C:\\LegacyFiles\\Archive",
  "ErrorDirectory": "C:\\LegacyFiles\\Error",
  "Schedule": "0 */1 * * * ?"
}
```

## 📧 Email Notification on Polling Failure

-Sends alert email via SmtpClient if file job fails

-Admin email is configured in appsettings.json

```json
"Smtp": {
  "Host": "smtp.example.com",
  "Port": "587",
  "User": "noreply@example.com",
  "Pass": "password",
  "From": "noreply@example.com",
  "To": "admin@gac.com"
}

```

## 🔐 Authentication & JWT Setup

This API uses **JWT (JSON Web Token)** bearer authentication to protect secured endpoints. Below are the setup details and credentials for testing.

---

### 🧪 Sample User for Testing

Use the following credentials to log in and receive a JWT token:

| Username | Password     |
|----------|--------------|
| `admin`  | `Password@123` |

> ⚠️ These credentials are for **testing only** and should not be used in production environments.

---

### 🛠️ Login Endpoint

**POST** `/api/auth/login`

#### Request Body (JSON):

```json
{
  "username": "admin",
  "password": "Password@123"
}

