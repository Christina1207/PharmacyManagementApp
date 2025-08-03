# Pharmacy Management System

This Application is a modern, web-based application designed to streamline the operations of a pharmacy within a larger organization. It provides a comprehensive solution for managing medications, inventory, prescriptions, and patient records, with a specific focus on handling insurance coverage for employees and their families.

This system is built with a decoupled frontend and backend architecture, ensuring maintainability, scalability, and a better user experience.

---

## ✨ Key Features

* **Patient Management**: Admins can manage records for employees and their family members, including activating or deactivating their accounts.
* **Medication & Inventory Control**: Pharmacists can add new medications, manage stock levels, and record incoming orders to update inventory automatically. The system tracks medication batches by expiration date to ensure a First-In-First-Out (FIFO) dispensing process.
* **Prescription Dispensing**: A streamlined process for dispensing prescriptions, automatically calculating patient costs based on their coverage type (full coverage for employees, 75% for family members).
* **User and Role Management**: Admins have full control over user accounts, with the ability to add new pharmacists or other admins and manage their system roles and access levels.
* **Reporting**: The system can generate financial and administrative reports to provide insights into sales and inventory levels, supporting better decision-making.

---

## 🏗️ Architecture Overview

The App is built on a modern, decoupled architecture that separates the frontend and backend applications. This approach offers several advantages, including improved maintainability, scalability, and the ability to develop and deploy each part of the system independently.

### Backend

The backend is developed using **ASP.NET Core** and follows the principles of **Clean Architecture**. This architectural style organizes the code into four distinct layers, ensuring a clear separation of concerns and making the system more robust and easier to maintain:

* **Domain Layer**: This is the core of the application, containing the main business entities and logic. It is the most independent layer and has no dependencies on other layers in the project.
* **Application Layer**: This layer contains the application's use cases and orchestrates the flow of data between the Domain and Infrastructure layers. It uses Data Transfer Objects (DTOs) to decouple the core domain from the outside world.
* **Infrastructure Layer**: This layer handles all external concerns, such as database interactions (using Entity Framework Core), file systems, and other external services. It implements the interfaces defined in the application layer.
* **Presentation Layer**: This is the entry point to the backend, implemented as an ASP.NET Core Web API. It exposes a set of RESTful endpoints that the frontend consumes.

Several design patterns are also used to ensure a well-structured and maintainable codebase:
* **Repository Pattern**
* **Unit of Work Pattern**
* **Dependency Injection**

The database is managed using a **Code-First** approach with Entity Framework Core, allowing the database schema to be generated and updated directly from the C# entity classes.

### Frontend

The frontend is a **Single-Page Application (SPA)** built with **React** and **TypeScript**. It utilizes a component-based architecture, which allows for the creation of reusable UI elements and a more organized and maintainable codebase. The key parts of the frontend architecture are:

* **UI Components**: The user interface is built using a combination of custom components and the **shadcn/ui** component library, which provides a set of accessible and customizable UI elements.
* **State Management**: The application's global state, particularly for authentication, is managed using React's **Context API**.
* **Services Layer**: All communication with the backend API is handled through a dedicated services layer, which uses the **axios** library to make HTTP requests.

The frontend and backend communicate via a **RESTful API**, with the backend exposing endpoints that return data in JSON format.

---

## 📂 Project Structure

The repository is organized into two main parts: the backend solution and the frontend application.

```
/
├── PharmacyManagementApp/    # .NET Solution Root
│   ├── Domain/               # Core entities and business logic
│   ├── Application/          # Application services, DTOs, and interfaces
│   ├── Infrastructure/       # Data access, repositories, DbContext
│   └── PharmacyManagmentApp/ # API Controllers and startup configuration
│
└── PharmacyApp-Frontend/     # React Frontend Application
    ├── public/               # Static assets
    └── src/                  # Main source code
        ├── components/       # Reusable UI components (including shadcn/ui)
        ├── context/          # React Context for state management
        ├── hooks/            # Custom React hooks
        ├── lib/              # Utility functions
        ├── pages/            # Application pages/routes
        └── services/         # API client and services for backend communication
```

---

## 🛠️ Dependencies

### Backend

* ASP.NET Core
* Entity Framework Core
* ASP.NET Core Identity for authentication
* JWT Bearer for token-based authorization
* AutoMapper for object-to-object mapping
* Swashbuckle for API documentation

### Frontend

* React & TypeScript
* Vite
* Tailwind CSS
* shadcn/ui
* axios
* React Router
* Recharts

---

## 🚀 Getting Started

To run the project on your local machine, you will need to have [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and [Node.js](https://nodejs.org/) installed.

### Backend Setup

1.  **Navigate to the solution directory**:
    ```bash
    cd PharmacyManagementApp
    ```
2.  **Restore dependencies**:
    ```bash
    dotnet restore PharmacyManagmentApp.sln
    ```
3.  **Update the database connection string**:
    Open `PharmacyManagmentApp/appsettings.Development.json` and modify the `DefaultConnection` string to point to your local SQL Server instance.
4.  **Apply database migrations**:
    ```bash
    dotnet ef database update --project Infrastructure
    ```
5.  **Run the backend server**:
    ```bash
    dotnet run --project PharmacyManagmentApp
    ```
    The API will be available at `https://localhost:7144` and `http://localhost:5286`.

### Frontend Setup

1.  **Navigate to the frontend directory**:
    ```bash
    cd PharmacyApp-Frontend
    ```
2.  **Install dependencies**:
    ```bash
    npm install
    ```
3.  **Update the API base URL**:
    In a new file named `.env.local` inside the `PharmacyApp-Frontend` directory, add the following line to point to your local backend server:
    ```
    VITE_API_BASE_URL=https://localhost:7144/api
    ```
4.  **Run the frontend development server**:
    ```bash
    npm run dev
    ```
    The application will be accessible at `http://localhost:8080`.
