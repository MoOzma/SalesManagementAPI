# 🛒 Sales Management REST API

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

