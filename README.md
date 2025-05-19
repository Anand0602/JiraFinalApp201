 

 
## 🛠️ JiraApp

A **Jira-inspired Task Management Web App** built with **ASP.NET Core MVC**, **Entity Framework Core**, and **AutoMapper**. This application enables users to manage projects and tasks with unique, scoped identifiers (`CON-2-5`), and is structured with a clean, scalable architecture.

---

## 🚀 Features

- 🔐 **Session-based user authentication**
- 📁 **Project and Task CRUD operations**
- 🧮 **Scoped task ID generation** (e.g., `CON-2-5`)
- 🔄 **AutoMapper integration** for clean separation between models and views
- 🧱 **Layered architecture**: Controller → Service → Repository → Data
- 💾 **EF Core** with support for SQL Server or In-Memory DB
- 🧪 **Basic unit testing support**
- 🎨 **Responsive Bootstrap UI**

---

## 🛠️ Technologies Used

- ✅ **.NET 8 / ASP.NET Core MVC**
- ✅ **Entity Framework Core**
- ✅ **AutoMapper**
- ✅ **Bootstrap 5**
- ✅ **SQL Server / In-Memory DB**
- ✅ **LINQ, Middleware, Session management**

---

## 📁 Project Structure

```plaintext
JiraFinalApp201/
│
├── Controllers/          → MVC Controllers (e.g., TaskController, AccountController)
├── Models/               → Entity Models (User, Project, TaskItem)
├── ViewModels/           → DTOs for views
├── Services/             → Business Logic Layer
├── Repositories/         → Data Access Layer (via EF Core)
├── Views/                → Razor Views for UI
├── wwwroot/              → Static files (CSS, JS, images)
├── appsettings.json      → Configuration
└── Program.cs / Startup.cs
````

---

## 🔢 Task ID Format

Each task in a project is assigned a scoped custom ID:

```text
CON-{ProjectId}-{TaskCount}
```

### 📌 Example

If you create the 5th task in the project with ID `2`, it becomes:

```text
CON-2-5
```

> ✅ The logic is handled in the **Task Service Layer** to ensure uniqueness per project.

---

## 🧑‍💻 Getting Started

### ✅ Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* SQL Server (or configure an In-Memory database)
* Visual Studio or Visual Studio Code

### 🛠️ Installation Steps

1. **Clone the repository**

   ```bash
   git clone https://github.com/Anand0602/JiraFinalApp201.git
   cd JiraFinalApp201
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Apply database migrations**

   ```bash
   dotnet ef database update
   ```

4. **Run the application**

   ```bash
   dotnet run
   ```

5. **Visit in browser**

   ```
   http://localhost:5000
   https://localhost:5001
   ```

---

## 🔐 Authentication

* 🧾 User registration and login with session-based authentication
* 🔒 Protected routes for authorized users
* 📂 Access to dashboard, projects, and tasks after login

---

## 📌 Usage Flow

1. 🔓 Login or Register
2. ➕ Create a Project
3. ➕ Add Tasks to the Project
4. 🆔 Each task gets a `CON-{ProjectId}-{TaskNumber}` style ID
5. 🛠️ Edit/Delete tasks and projects as needed

---

## 🧰 Tools & Packages

* **AutoMapper** – Clean mapping between entities and view models
* **EF Core** – ORM for managing database operations
* **Bootstrap** – UI styling and layout
* **LINQ** – Elegant data queries

---

## 🤝 Contributing

We welcome contributions from the community!

1. **Fork the repository**
2. **Create a new branch**

   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Make changes and commit**

   ```bash
   git commit -m "Add your message"
   ```
4. **Push to GitHub**

   ```bash
   git push origin feature/your-feature-name
   ```
5. **Open a Pull Request**

---

## 📜 License

This project is licensed under the **MIT License**.

---

## 🙏 Acknowledgments

* 📘 [.NET Documentation](https://learn.microsoft.com/en-us/dotnet/)
* 🎨 [Bootstrap](https://getbootstrap.com/)
* 🤖 GitHub Copilot & ChatGPT for development assistance

---

> Built with ❤️ using ASP.NET Core MVC, EF Core, and Bootstrap.

```

 
