# Test App
This is a sample project composed by
  * app
    * src
        * API
        * Client
    * test
        * API.Tests
        * Client.Tests
  * scripts (SQL scripts)

# Objectives
1. Add `.gitignore` and `.gitattributtes` files 
2. Create a WPF (Client) and a WEB.API (API)
3. Add projects for unit testing to WPF (Client.Tests) and API (Api.Tests) projects
4. Create SQL script to create a database with one table and the following fields
 * Fields
    - Name (varchar)
    - Avatar (varbinary)
    - Email (varchar)
5. WEB.API *must* use Entity Framework database first to connect to the database
6. Establish communication from the WPF application with the WEB.API
7. Add a controller to the WEB.API to get/post information from/to the database
8. Create a view in the WPF project to manage that data (CRUD)
9. Create a SQL script to add a new field to the table and update the database context and show it in the CRUD
 * Field
    - Age (int)

## Plus.
1. IoC
2. Framework like GalaSoft
3. Asynchronous code

# How to edit this project
1. Create a GitHub account if you don't have one already
2. Fork this repository
3. Create a feature branch from the branch `test`
4. Submit a Pull Request to this repository
