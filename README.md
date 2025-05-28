# 📁 Document Storage System

## Overview
Document Storage System is a secure and efficient web application built with ASP.NET Core 8.0 for managing documents with user authentication, versioning, and secure file access. It provides a RESTful API for seamless integration and a simple frontend for user interaction. The system ensures private document access, stores files as BLOBs in a database, and includes comprehensive unit tests to ensure reliability.

## 🚀 Features
- 🔐 **User Authentication**: Secure registration and login using JWT-based authentication.
- 📄 **Document Management**: Upload, store, and retrieve documents with version control.
- 🔒 **Private Document Access**: Users can only access their own uploaded files.
- 🔄 **Versioning Support**: Store multiple versions of documents with the same name; retrieve the latest version or specific versions using a revision query parameter.
- 🌐 **RESTful API**: Well-documented endpoints for integration.
- 🧪 **Unit Testing**: Comprehensive test suite using xUnit for reliability.
- 🖥️ **Bonus UI**: Simple HTML and JavaScript-based frontend for interacting with the API.

## 🛠 Technologies Employed
- **ASP.NET Core 8.0**: High-performance framework for building scalable RESTful APIs.
- **Entity Framework Core 8.0**: ORM for data access with SQL Server and in-memory database for testing.
- **SQL Server**: Relational database for persistent storage of documents as BLOBs.
- **JWT Authentication (Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0)**: Token-based authentication for secure access.
- **BCrypt.Net-Next 4.0.3**: Password hashing for secure credential storage.
- **Swashbuckle.AspNetCore 6.6.2**: Swagger/OpenAPI for interactive API documentation.
- **xUnit 2.9.3**: Testing framework for unit and integration tests.
- **xUnit Runner VisualStudio 3.1.0**: Test runner for Visual Studio integration.
- **HTML & JavaScript**: Lightweight frontend for user interaction.

## 🧰 Prerequisites
- Visual Studio 2022 or VS Code with .NET SDK 8.0 or later.
- SQL Server instance (local or remote).
- .NET CLI for command-line operations.
- Git for cloning the repository.

## ⚙️ Installation Steps
1. **Clone the Repository**  
   Clone or download the project to your local machine:
   ```bash
   git clone https://github.com/ShwetankRise/DocumentStorageSystem
   ```

2. **Configure Database Connection**  
   Open `appsettings.json` and update the `ConnectionStrings:DefaultConnection` to match your SQL Server configuration. Example:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=your_server;Database=DocumentStorageDB;Trusted_Connection=True;"
   }
   ```

3. **Apply Database Migrations**  
   In Visual Studio’s Package Manager Console or a terminal, run:
   ```bash
   dotnet ef database update
   ```

4. **Restore Dependencies**  
   If packages are missing, run:
   ```bash
   dotnet restore
   ```

5. **Run the Application**  
   Start the application in Visual Studio (press F5) or via CLI:
   ```bash
   dotnet run
   ```

6. **Access the Application**  
   Open your browser and navigate to:  
   `https://localhost:7117`  
   Swagger UI is available at:  
   `https://localhost:7117/swagger`

## 👨‍💻 Using the Application
- **Register**: Create a user account via the `/api/auth/register` endpoint or the UI registration page.
- **Log In**: Authenticate via `/api/auth/login` to receive a JWT token.
- **Upload Documents**: Use the UI or `/api/documents/upload` endpoint to upload files (JWT required).
- **View/Download Documents**: Access your documents via the UI or `/api/documents/{filename}` endpoint. Use `?version={number}` to retrieve specific versions.
- **Example Workflow**:
  - Upload `review.pdf` → Stored as version 1.
  - Upload another `review.pdf` → Stored as version 2.
  - Access `/api/documents/review.pdf` → Returns latest version (2).
  - Access `/api/documents/review.pdf?version=0` → Returns first version (1).

## 📌 API Endpoints
| Method | Endpoint                    | Description                      | Request Body / Params                            |
| ------ | --------------------------- | -------------------------------- | ------------------------------------------------ |
| POST   | `/api/auth/register`        | Register a new user              | `{ "username": "string", "password": "string" }` |
| POST   | `/api/auth/login`           | Authenticate and get JWT token   | `{ "username": "string", "password": "string" }` |
| POST   | `/api/documents/upload`     | Upload a document (JWT required) | Multipart form-data with file                    |
| GET    | `/api/documents/{filename}` | Download document by filename    | Query: `?version={number}` (optional)            |

## 📁 Project Structure
```
DocumentStorageSystem/
├── Controllers/        # API controllers for authentication and document operations
├── Data/               # EF Core DbContext and migration files
├── Models/             # Data models (User, Document, DocumentVersion)
├── Services/           # Business logic for authentication, file storage, and versioning
├── Tests/              # xUnit tests for unit and integration testing
├── wwwroot/            # Static frontend assets (HTML, JS, CSS)
├── Program.cs          # Application startup and configuration
├── appsettings.json    # Configuration for database and JWT settings
```

## 🧪 Running Tests
To run the unit and integration tests:
1. Open a terminal or Package Manager Console.
2. Execute:
   ```bash
   dotnet test
   ```

## 🛠 Design Principles (SOLID)
The system is designed with adherence to SOLID principles to ensure maintainability, scalability, and robustness:

- **Single Responsibility Principle (SRP)**: Each class has a single responsibility. For example, `AuthService` handles only authentication (registration, login, JWT generation), while `DocumentService` manages document operations (upload, retrieval, versioning). Controllers like `AuthController` and `FilesController` focus solely on handling HTTP requests, delegating business logic to services.
- **Open/Closed Principle (OCP)**: The system is open for extension but closed for modification, achieved through dependency injection (DI) in `Program.cs`. Services like `DocumentService` can be extended or replaced (e.g., for cloud storage) without modifying existing code. The middleware pipeline allows adding new features like logging without altering the core application.
- **Liskov Substitution Principle (LSP)**: The system avoids complex inheritance, using composition instead. Services and the `AppDbContext` (inheriting from `DbContext`) behave consistently, ensuring that substituting implementations (e.g., in-memory database for tests) does not break functionality.
- **Interface Segregation Principle (ISP)**: Services and controllers are designed with focused responsibilities, ensuring clients only depend on relevant methods. For example, `FilesController` only uses `DocumentService` methods for file operations, avoiding unnecessary dependencies.
- **Dependency Inversion Principle (DIP)**: High-level modules (controllers) depend on abstractions via DI. Services are injected using ASP.NET Core’s DI container, and `IConfiguration` abstracts settings access. For example, `DocumentService` depends on `AppDbContext`, which can be swapped with different providers (e.g., in-memory for testing).


## 🛠 Troubleshooting
- **Missing Dependencies**: Run `dotnet restore` to resolve missing NuGet packages.
- **Database Connectivity**: Verify SQL Server is running and the connection string is correct.
- **Port Conflicts**: If `https://localhost:7117` is unavailable, check for port conflicts or update the port in `launchSettings.json`.
- **JWT Issues**: Ensure the JWT token is included in the `Authorization` header as `Bearer <token>` for protected endpoints.

## 🤝 Contributing
Contributions are welcome! Please submit a pull request or open an issue on the GitHub repository to discuss improvements or bug fixes.

## 📬 Contact
For queries, mailto:96shwetank@gmail.com


## ✅ Submission Notes
- The code is hosted in a public GitHub repository with a clean commit history showing incremental development.
- The project adheres to the assessment requirements, including secure authentication, private document access, versioning, BLOB storage, and unit testing.
- Best practices such as SOLID principles, dependency injection, and clean architecture are followed.
- The bonus UI is implemented using HTML and JavaScript for a seamless user experience.
