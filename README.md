# Dotnet Razor Page Login with MSSQL - CRUD Operations

## 📜 Project Overview
This project demonstrates a **Login System** built with **ASP.NET Core Razor Pages**, integrated with **Microsoft SQL Server (MSSQL)**. It includes full **CRUD (Create, Read, Update, Delete)** operations for user accounts.

---

## 🚀 Features
- 🔑 **User Authentication** (Login & Registration)
- 🏷️ **CRUD Operations** for managing user accounts
- 🗄️ **MSSQL Database Integration**
- 📦 **Entity Framework Core** for database operations
- 💻 **Razor Pages** with **.NET Core 8**

---

## 🏗️ Tech Stack
- **.NET Core 8**
- **ASP.NET Razor Pages**
- **Entity Framework Core**
- **Microsoft SQL Server (MSSQL)**
- **Dependency Injection (DI)**

---

## ⚙️ Installation & Setup

### 1. Clone the Repository
```bash
git clone https://github.com/aqu640/MVC-Login.git
cd MVC-Login
```

### 2. Update Connection String
Edit **`appsettings.json`** to match your MSSQL server.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=UsersAppDB;Trusted_Connection=True;"
  }
}
```

### 3. Apply Migrations
```bash
dotnet ef database update
```

### 4. Run the Application
```bash
dotnet run
```
### 5. Swagger
```URL
http://localhost:5227/swagger/index.html
```
---

## 🔍 Project Structure
```
📁 MVC-Login
│
├── 📂 Controllers
│   └── AccountController.cs
│
├── 📂 Data
│   └── ApplicationDbContext.cs
│
├── 📂 Models
│   └── User.cs
│
├── 📂 Pages
│   ├── 📁 Account
│   │   ├── Login.cshtml
│   │   ├── Register.cshtml
│   │   └── Index.cshtml
│
├── appsettings.json
├── Program.cs
└── UsersApp.csproj
```

---

## 🏃‍♂️ CRUD Operations

### 🆕 **Create (Register)**
- Users can register with **Username**, **Password**, and **Email**.

### 🔍 **Read (View List)**
- View a list of all registered users.

### ✏️ **Update (Edit)**
- Modify user details.

### 🗑️ **Delete (Remove)**
- Delete user accounts.

---

## 📸 Screenshots

### 🔑 **Login Page**
![image](https://hackmd.io/_uploads/SJp-73a5kl.png)

### 🏡 **Swagger**
![image](https://github.com/user-attachments/assets/b2d2a7c9-4be5-42dd-8389-45bca56ebedc)

### 📋 **User List**


---

## 🛠️ Troubleshooting
- **Migration Issues?**
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

- **Dependency Issues?**
  ```bash
  dotnet restore
  ```

---

## 🤝 Contributing
Contributions are welcome! Feel free to submit issues.

---

## 🏅 Acknowledgments
- [串接 SQL](https://www.youtube.com/watch?v=AvNVxRIMvco)
- [Create ASP.NET Core Web Application With SQL Server Database Connection and CRUD Operations](https://www.youtube.com/watch?v=T-e554Zt3n4)

---

## 📜 License
This project is licensed under the **MIT License**.
