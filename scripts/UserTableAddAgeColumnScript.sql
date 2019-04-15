--Use the TestApp database
USE TestApp;
GO

--Add Age column to Users table in TestApp database
IF OBJECT_ID('Users') IS NOT NULL AND COL_LENGTH('dbo.Users', 'Age') IS NULL
	ALTER TABLE dbo.Users ADD Age INT NOT NULL;
GO