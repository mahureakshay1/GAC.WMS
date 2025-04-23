# GAC's Warehouse Management System

GAC.WMS is a .NET 9-based Warehouse Management System designed to streamline warehouse operations, including order processing, customer management, and integration with external systems.

## Clean Architecture

This project follows the principles of **Clean Architecture**, which emphasizes:

1. **Separation of Concerns**: Divides the system into layers, each with a specific responsibility.
2. **Dependency Inversion**: High-level modules do not depend on low-level modules. Both depend on abstractions.
3. **Testability**: Each layer can be tested independently.

### Layers in the Project

1. **Domain Layer**:
   - Contains core business logic and entities such as `SaleOrder`, `Customer`, and `Product`.
   - Independent of any external frameworks or technologies.

2. **Application Layer**:
   - Contains use cases and business rules.
   - Includes services like `SaleOrderService` and DTOs like `SellOrderDto`.

3. **Infrastructure Layer**:
   - Handles external concerns such as database access, background jobs, and authentication.
   - Uses libraries like `EntityFrameworkCore` and `Hangfire`.
   *** Persistance Layer *** :
   - Responsible for data storage.

## Project Structure

### 1. **GAC.WMS.Application**
- Contains business logic and integration models.
- ***Folders structure***
     - Interfaces
     - Dto's
     - Input validation
     - Mapping of objects
- Key Libraries:
  - `AutoMapper` for object mapping.
  - `FluentValidation` for input validation.

### 2. **GAC.WMS.Infrastructure**
- Manages database access and external service integrations.
-  ***Folder structure***
     - File parser (XML,CSV etc)
     - Integration of cron job in system
     - Job
     - Mapping of objects xml to dto
     - Persistance layer with EF core
     - Services implimentation 
- Key Libraries:
  - `EntityFrameworkCore` for database interactions.
  - `Hangfire` for background job processing.
  - `Microsoft.AspNetCore.Authentication.JwtBearer` for authentication.

### 3. **GAC.WMS.Domain**
- Defines core domain entities such as `SaleOrder`, `Customer`, and `Product`.
- ***Folder structure***
   - Entities
   - Enums
   - Exceptions

### 4. **GAC.WMS.IntegrationEngine (Cron Job)**
1. **File Processing and Dispatching**:
   - The `IntegrationDispatcher` class is responsible for scanning a specified directory for files and delegating their processing to appropriate handlers (`IIntegrationHandler` implementations).
   - This enables the system to dynamically handle different types of files based on the logic defined in the handlers.

2. **Extensibility via Handlers**:
   - The use of the `IIntegrationHandler` interface allows for a plug-and-play architecture where new file types or processing logic can be added by implementing the interface.
   - Handlers determine if they can process a file (`CanHandleAsync`) and then execute the processing logic (`ProcessAsync`).

3. **Integration with External Systems**:
   - By processing files, the `IntegrationEngine` facilitates communication with external systems, such as importing data into the application or exporting data for external use.

4. **Error and Success Handling**:
   - The `IIntegrationHandler` interface includes methods for handling errors (`HandleError`) and successes (`HandleSuccess`), ensuring robust and traceable file processing.

### 5. **GAC.WMS.Api**
- Provides controllers async methods.
- Rate limiter (Token bucket)
- Jwt token authentication
- Exception middleware
- ***Folder structure***
    - Controllers
    - Middleware

### 5. **GAC.WMS.UnitTests**
- Provides unit tests for application and infrastructure layers.
- Key Libraries:
  - `MSTest` for testing.
  - `Moq` for mocking dependencies.
  - `EntityFrameworkCore.InMemory` for in-memory database testing.
  - ***Folder structure***
    - Application.Tests

## Prerequisites

Before running the application, ensure the following are installed on your system:

1. **Docker**: Used for containerizing and running the application.
   - [Install Docker](https://docs.docker.com/get-docker/)
   - Verify installation:  docker --version
2. **PowerShell**: Required for running dockerdeploy.ps1 script.
   - [Install PowerShell](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell)
   - Verify installation: pwsh --version

## Installation
1. Create folder in C drive "C:\SharedFolder"
2. Add any number of customer folder (Customer1, Customer2....)
   ![image](https://github.com/user-attachments/assets/2fbdaf88-1dc3-4f44-9e08-a58af1ef4e17)
3. Share "SharedFolder" (parent to Customers folders) folder to everyone.
   ![image](https://github.com/user-attachments/assets/de0735bc-9e8b-4f27-a138-9aadc1a14324)
5. This customer1,customer2... folder contains XML files form customer to import in system (right now only xml support is implimented)
   
6. Clone the repository: git clone https://github.com/your-repo/GAC.WMS.git
7. Navigate to the solution directory:
8. Open power shell in root directory where dockerdeploy.ps1 is present
9. Execute dockerdeploy.ps1.(Docker desktop must be up and running)
   - This script build project
   - Run unit test cases
   - Create sql server image and run migration (add test data as well)
   -Deploy application.
10. Check both container db-1 and server-1 inside gacwms are running or not if server-1 not running manualy run it.
11. Accsess API using URL : [http://localhost:8080](http://localhost:8080) (using postman).
12. Access [Hanfire dashbord URL:[http://localhost:8080/jobs](http://localhost:8080/jobs)
13. Access Swagger UI URL: [http://localhost:8080/swagger/index.html]([http://localhost:8080/jobs](http://localhost:8080/swagger/index.html))
14. Copy xml files prent in docuemnts folder of solution into "C:\SharedFolder\Customer1\"
    - Job process those file one by one and move to Success folder or into Error folder depending opon operation result.



   
    
   
