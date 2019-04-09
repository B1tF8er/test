--Create the TestApp database
IF NOT EXISTS(SELECT name FROM master..sysdatabases WHERE name='TestApp')
	CREATE DATABASE TestApp;
GO

--Use the TestApp database
USE TestApp;
GO

--Create the Users table in TestApp database
IF OBJECT_ID('Users') IS NULL
CREATE TABLE dbo.Users(
	Id INT IDENTITY(1,1) PRIMARY KEY
	,Name VARCHAR(128)
	,Avatar VARBINARY(MAX)
	,Email VARCHAR(256)	
);
GO