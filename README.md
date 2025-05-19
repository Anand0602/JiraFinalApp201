Sure! Here's the **entire content of the `README.md` file** in a **single markdown code block**, ready to copy and paste:

```markdown
# JiraFinalApp201

A Jira-inspired Task Management Web App built with **ASP.NET Core MVC**, **Entity Framework Core**, and **AutoMapper**. This app features project-based task tracking, scoped task identifiers (`CON-2-5` format), and a clean layered architecture.

---

## 🚀 Features

- 🔐 Session-based user authentication  
- 🗂️ Project and Task management (Create, Update, Delete)  
- 🔢 Scoped task ID generation (e.g., `CON-2-5`)  
- 🔄 AutoMapper integration for model-view separation  
- 🧱 Layered architecture: Controller → Service → Repository → Data  
- 💾 EF Core ORM with SQL Server or In-Memory DB support  
- 🧪 Basic Unit Testing support  
- 🎨 Responsive UI using Bootstrap  

---

## 🛠️ Technologies

- .NET 8 / ASP.NET Core MVC  
- Entity Framework Core  
- AutoMapper  
- SQL Server / In-Memory DB  
- Bootstrap 5  
- LINQ, Session, Middleware  

---

## 📂 Project Structure

```

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

```

---

## 🔢 Task ID Format

Each task in a project has a custom ID like:

```

CON-{ProjectId}-{TaskCount}

````

**Example**:  
If you create the 5th task in project with ID 2 → `CON-2-5`  
This logic is implemented in the Task service layer to keep IDs unique per project.

---

## 🧑‍💻 Getting Started

### ✅ Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)  
- SQL Server (or configure InMemory database for development)  
- Visual Studio / VS Code  

### 🛠️ Installation

1. Clone the repo:

```bash
git clone https://github.com/Anand0602/JiraFinalApp201.git
cd JiraFinalApp201
````

2. Restore dependencies:

```bash
dotnet restore
```

3. Apply EF Core migrations:

```bash
dotnet ef database update
```

4. Run the app:

```bash
dotnet run
```

Then open a browser and navigate to:
📍 `http://localhost:5000` or `https://localhost:5001`

---

## 🔐 Authentication

* Session-based login/logout
* Middleware protects authenticated routes
* Register → Login → Access Dashboard/Projects/Tasks

---

## ✅ Usage

* Login to your dashboard
* Create Projects
* Inside each project, add tasks
* Each task gets a `CON-{ProjectId}-{TaskNo}` style ID

---

## 🧰 Tools Used

* AutoMapper (for entity/view model mapping)
* EF Core Code-First Migrations
* Bootstrap UI Templates
* LINQ queries for filtering tasks

---

## 🤝 Contributing

1. Fork the repository
2. Create your branch (`git checkout -b feature/feature-name`)
3. Commit your changes (`git commit -m 'Add new feature'`)
4. Push to the branch (`git push origin feature/feature-name`)
5. Open a Pull Request

---

## 📜 License

This project is licensed under the MIT License.

---

## 🙋‍♂️ Acknowledgments

* .NET Documentation
* Bootstrap UI Kit
* GitHub Copilot & ChatGPT for assistant-based dev acceleration

```

Let me know if you'd like this README to include screenshots, database schema diagrams, or setup for Docker/Azure deployment.
```
