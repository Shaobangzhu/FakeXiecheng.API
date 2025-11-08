# FakeXiecheng.API

A RESTful Web API for a travel booking platform built with **ASP.NET
Core 3.1**, **Entity Framework Core**, and **SQL Server**.\
This project simulates a tourism booking system with user registration,
JWT authentication, and CRUD operations for travel routes and pictures.

------------------------------------------------------------------------

## 🚀 Features

-   User registration and login with **JWT authentication**
-   CRUD operations for travel routes and images
-   Pagination, search, and filtering with query parameters
-   Layered architecture: Controller → Service → Repository
-   **Entity Framework Core (Code-First)** with migrations
-   **Swagger UI** integration for API documentation
-   Centralized **error-handling middleware**
-   Configurable **Dependency Injection**
-   SQL Server database integration

------------------------------------------------------------------------

## 🧱 Project Structure

    FakeXiecheng.API/
    ├── Controllers/
    │   ├── TouristRoutesController.cs
    │   ├── TouristRoutePicturesController.cs
    │   └── AuthenticationController.cs
    │
    ├── Services/
    │   ├── ITouristRouteRepository.cs
    │   └── TouristRouteRepository.cs
    │
    ├── Models/
    │   ├── TouristRoute.cs
    │   ├── TouristRoutePicture.cs
    │   ├── ApplicationUser.cs
    │   └── DTOs/
    │       ├── TouristRouteDto.cs
    │       └── TouristRouteForCreationDto.cs
    │
    ├── Helpers/
    │   ├── PaginationResourceParameter.cs
    │   └── JwtHelper.cs
    │
    ├── Startup.cs
    ├── appsettings.json
    └── FakeXiecheng.sln

------------------------------------------------------------------------

## ⚙️ Technologies Used

  Category            Technology
  ------------------- ------------------------------------------
  Backend Framework   ASP.NET Core 3.1
  ORM                 Entity Framework Core
  Database            SQL Server
  Authentication      JWT (JSON Web Token)
  Documentation       Swagger
  Design Pattern      Repository Pattern, Dependency Injection

------------------------------------------------------------------------

## 🧩 Key Endpoints

  HTTP Verb   Endpoint                    Description
  ----------- --------------------------- ------------------------------
  `POST`      `/api/auth/register`        Register new user
  `POST`      `/api/auth/login`           Login and get JWT token
  `GET`       `/api/touristRoutes`        Get paginated list of routes
  `GET`       `/api/touristRoutes/{id}`   Get route by ID
  `POST`      `/api/touristRoutes`        Create new route
  `PUT`       `/api/touristRoutes/{id}`   Update route
  `DELETE`    `/api/touristRoutes/{id}`   Delete route

------------------------------------------------------------------------

## 🔐 Authentication Flow

1.  User registers via `/api/auth/register`

2.  Login using `/api/auth/login` to receive JWT token

3.  Include token in header as:

        Authorization: Bearer <your_token>

4.  Access protected endpoints

------------------------------------------------------------------------

## 🧠 Learning Objectives

-   Understand **ASP.NET Core Web API** fundamentals
-   Practice **Entity Framework Core** (code-first & migration)
-   Implement **JWT Authentication**
-   Learn **middleware pipeline** and **DI container**
-   Explore **Swagger** integration for testing

------------------------------------------------------------------------

## 💡 Future Improvements

-   Upgrade to **.NET 8** and migrate to minimal APIs
-   Add **Redis** caching for performance
-   Integrate **Docker** containerization
-   Add **unit tests** with xUnit or MSTest
-   Implement **logging** with Serilog

------------------------------------------------------------------------

## 👨‍💻 Author

**Chaoran Lu**\
Full Stack Developer \| React • Node.js • .NET Core\
GitHub: [Shaobangzhu](https://github.com/Shaobangzhu)
# FakeXiecheng.API
Back-End RESTful API Using ASP.NET core
