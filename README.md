Overview
This guide will help you set up the Web App Workshop Management System, which includes four navigation sections, each designed to retrieve specific data according to requirements. Additionally, bonus features are included in the system. We use Razor methods for data retrieval and insertion.

Project Installation Guide
Pre-requisites
Visual Studio 2022: Ensure you have Visual Studio 2022 installed on your machine.
.NET 7.0 SDK: Install the .NET 7.0 SDK to build and run the application.
Setup Steps
Clone Repository: Clone the project repository from GitHub.

Repository URL: [GitHub Repository URL]
Download WebApp: In the repository, locate and download the webapp.rar file.

If needed, extract the contents of webapp.rar to access the webapp folder.
Note: It's recommended to use the webapp.rar zip file to avoid potential issues faced after cloning the repository with the webapp folder.
Open Solution: Open Visual Studio 2022.

Navigate to File > Open > Project/Solution.
Browse and select the webapp folder or the extracted solution from webapp.rar.
Replace Connection String:

Locate the appsettings.json file in the solution.
Update the connection string under "ConnectionStrings" to point to your database.
json
Copy code
"ConnectionStrings": {
    "DefaultConnection": "Server=YourServer;Database=Prowerse;User=YourUsername;Password=YourPassword;"
}
Database Setup:

Ensure SQL Server is running on your machine.
Open SQL Server Management Studio (SSMS) or any SQL client tool.
Create Database:

Execute the SQL script provided (creation_insertion_script.sql) from the document in the repository to create the database.
create the database Name Prowerse.
Run the script to create tables and insert initial data.
Build and Run Application:

Build the solution in Visual Studio.
Run the application to ensure everything is set up correctly.
