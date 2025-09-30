# 🚀 AgileBoard - Agile Project Management API

[![.NET](https://img.shields.io/badge/.NET-8.0-purple?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Containerized-blue?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![JWT](https://img.shields.io/badge/Auth-JWT-green?style=for-the-badge&logo=jsonwebtokens)](https://jwt.io/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red?style=for-the-badge&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server/)
[![Swagger](https://img.shields.io/badge/API%20Docs-Swagger-orange?style=for-the-badge&logo=swagger)](https://swagger.io/)
[![Tests](https://img.shields.io/badge/Testing-NUnit-brightgreen?style=for-the-badge)](https://nunit.org/)

> **A modern, containerized project management API showcasing enterprise-grade .NET development practices**

## 🎯 Project Overview

AgileBoard is a comprehensive project management API built to demonstrate **software engineering skills**. This project serves as a **portfolio piece** showcasing expertise in:
The main goal is not to create an entire project management tool but to highlight best practices and knowledge in software development.

- ✅ **Clean Architecture** & **SOLID Principles**
- ✅ **Test-Driven Development (TDD)** with 95%+ coverage
- ✅ **JWT Authentication & Authorization**
- ✅ **Docker Containerization**
- ✅ **RESTful API Design** with OpenAPI documentation
- ✅ **Entity Framework Core** with Code-First approach
- ✅ **Automated Testing** & **CI/CD Ready**

## 🏗️ Architecture & Tech Stack

### Core Technologies
| Component | Technology |
|-----------|------------|
| **Language** | C# 12.0 |
| **Framework** | .NET 8 + ASP.NET Core Web API |
| **Database** | SQL Server (Azure SQL Compatible) |
| **ORM** | Entity Framework Core |
| **Authentication** | JWT (JSON Web Tokens) |
| **API Documentation** | Swagger/OpenAPI |
| **Containerization** | Docker + Docker Compose |
| **Testing** | NUnit + Test-Driven Development |
| **Mapping** | AutoMapper |
| **Logging** | Built-in ASP.NET Core Logging |

### Clean Architecture
This project follows **Clean Architecture** principles with clear separation of concerns:

- **🎯 Presentation Layer:** Controllers, DTOs, and API-specific logic
- **⚙️ Application Layer:** Business logic, services, and use cases
- **🏢 Domain Layer:** Core entities, business rules, and domain logic
- **💾 Infrastructure Layer:** Data access, external services, and frameworks
- **🧪 Test Layer:** Comprehensive testing across all layers

### Design Patterns
- **Repository Pattern:** Data access abstraction
- **Dependency Injection:** Loose coupling and testability
- **CQRS Pattern:** Command and Query separation
- **Result Pattern:** Consistent error handling
- **AutoMapper:** Object-to-object mapping
- **Factory Pattern:** Object creation abstraction

## ✨ Key Features

### 🔐 Authentication & Authorization
- **JWT-based authentication** with refresh tokens
- **Role-based authorization** (Owner, Participant)
- **Secure password hashing** with salt
- **Protected endpoints** with granular permissions

### 📊 Project Management
- **Projects** with owner/participant management
- **Sprints** with date validation and active tracking
- **Work Items** with states (ToDo, Doing, Done)
- **Tags** for categorization and filtering
- **User Management** with profile updates

### 🛡️ Security Features
- **Input validation** at all layers
- **SQL injection protection** via EF Core
- **Cross-cutting concerns** (logging, error handling)
- **Environment-based configuration**

### 📖 API Documentation
- **Swagger/OpenAPI** integration
- **Interactive API explorer**
- **Comprehensive endpoint documentation**
- **Schema definitions** for all DTOs

## 🚀 Quick Start

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for local development)
- [Git](https://git-scm.com/)

### 🐳 Docker Deployment (Recommended)

1. **Clone the repository**
git clone https://github.com/kingofmars4/AgileBoard.git cd AgileBoard

2. **Set up docker-compose**
Edit docker-compose.yml file to configure database connection and JWT settings.

3. **Build and run with Docker Compose**
Linux/Mac: ./deploy.sh
Windows: deploy.bat
Or manually: docker-compose up -d --build

4. **Access the application**
- **API Documentation:** http://localhost:8080/swagger
- **Health Check:** http://localhost:8080/health
- **Database:** localhost:1433 (sa/Password_Mt_Forte)

### **🔧 Local Development**

1. **Install dependencies**
dotnet restore

2. **Update database connection** in 'appsetings.json'
{ "ConnectionStrings": { "DefaultConnection": "Server=localhost;Database=AgileBoardDb;Trusted_Connection=true;" } }

3. **Run database migrations**
dotnet ef database update --project AgileBoard.Infrastructure --startup-project AgileBoard.API

4. **Run the application**
dotnet run --project AgileBoard.API

## 🧪 Testing

This project demonstrates **Test-Driven Development (TDD)** with comprehensive test coverage:

### **Run All Tests**
dotnet test

### **Test Coverage Report**
dotnet test --collect:"XPlat Code Coverage"

### **Test Categories**
- **Unit Tests:** Business logic validation
- **Integration Tests:** Full API endpoint testing
- **Authorization Tests:** Security and permissions
- **Repository Tests:** Data access layer

## 🎯 Skills Demonstrated

### **💻 Backend Development**
- ✅ **Clean Architecture** implementation
- ✅ **SOLID Principles** adherence
- ✅ **Repository Pattern** with Unit of Work
- ✅ **Dependency Injection** configuration
- ✅ **Entity Framework Core** with migrations
- ✅ **AutoMapper** for object mapping

### **🔐 Security**
- ✅ **JWT Authentication** implementation
- ✅ **Role-based Authorization**
- ✅ **Password hashing** with salt
- ✅ **Input validation** and sanitization
- ✅ **SQL injection prevention**

### **🧪 Testing & Quality**
- ✅ **Test-Driven Development (TDD)**
- ✅ **Unit Testing** with NUnit
- ✅ **Integration Testing**
- ✅ **Mocking** and test isolation
- ✅ **Test coverage reporting**

### **🐳 DevOps & Containerization**
- ✅ **Docker** containerization
- ✅ **Docker Compose** orchestration
- ✅ **Multi-stage builds**
- ✅ **Health checks** and monitoring

### **📚 API Design**
- ✅ **RESTful API** principles
- ✅ **OpenAPI/Swagger** documentation
- ✅ **HTTP status codes** proper usage
- ✅ **Content negotiation**
- ✅ **API versioning** ready

## 📊 Project Metrics

- **Lines of Code:** ~15,000+
- **Test Coverage:** 95%+
- **API Endpoints:** 35+
- **Database Tables:** 8
- **Docker Images:** Optimized multi-stage builds
- **Documentation:** Comprehensive Swagger/OpenAPI

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**David Rodrigues** - Back-End Developer
- LinkedIn: [david-rodrigues](https://linkedin.com/in/david-rodrigues-9493bb378)
- GitHub: [@kingofmars4](https://github.com/kingofmars4)