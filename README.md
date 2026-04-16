# 🛒 Sales Management REST API

![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0-blue)
![JWT](https://img.shields.io/badge/JWT-Auth-orange)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)


> A robust, production-ready REST API for managing sales order lifecycles,
 built with **ASP.NET Core 8** and **Entity Framework Core** following **Clean Architecture** principles.

---

## 🔍 Overview
This project provides a comprehensive API solution for sales management,
covering order creation, real-time inventory tracking, 
and advanced sales reporting. Designed with **Separation of Concerns**
to ensure the system is scalable and maintainable.

---

## ✨ Key Features
* 🔐 **Advanced Security:** Role-based access control (RBAC) using **JWT Bearer Tokens**.
* 🏗️ **Generic Repository Pattern:** Clean and reusable data access layer.
* 📦 **Smart Inventory Management:** Automatic stock deduction upon order creation with validation.
* 📊 **Reporting Engine:** Built-in endpoints for daily and date-range sales reports.
* 🛡️ **Global Error Handling:** Centralized Middleware for consistent JSON error responses.
* 📜 **API Documentation:** Full **Swagger/OpenAPI** integration.

---
## 📡 Core Endpoints Summary

| Method | Endpoint | Access Level | Description |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/login` | Anonymous | Authenticates user and returns a JWT Token. |
| `POST` | `/api/Orders` | SalesRep+ | Creates a new sales order and updates inventory. |
| `GET` | `/api/Reports/daily` | **Admin Only** | Retrieves total sales summary for the current day. |
| `DELETE` | `/api/Orders/{id}` | **Admin Only** | Deletes a specific order (Restricted to Admin). |

---

## 🏗️ Architecture
The project follows an **N-Tier Architecture**:
1. **Presentation Layer:** Controllers handling HTTP requests and DTOs.
2. **Business Logic Layer:** Services containing business rules and validations.
3. **Data Access Layer:** Repositories abstracting database operations via EF Core.

---

## 🚀 Getting Started
1. **Clone the repository:** `git clone https://github.com/MoOzma/SalesManagementAPI.git`
2. **Database Config:** Update the connection string in `appsettings.json`.
3. **Apply Migrations:** Run `dotnet ef database update`.
4. **Run Project:** Run `dotnet run`.

---

