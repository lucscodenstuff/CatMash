IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'CreateTables')
DROP PROCEDURE CreateTables
GO

CREATE PROCEDURE CreateTables 
as
BEGIN
SET NOCOUNT ON
IF EXISTS(SELECT * FROM dbo.sysobjects where id = object_id(N'dbo.[CatsFurs]') and OBJECTPROPERTY(id, N'IsTable') = 1)
DROP TABLE CatsFurs
IF EXISTS(SELECT * FROM dbo.sysobjects where id = object_id(N'dbo.[Cats]') and OBJECTPROPERTY(id, N'IsTable') = 1)
DROP TABLE Cats
IF EXISTS(SELECT * FROM dbo.sysobjects where id = object_id(N'dbo.[FurTypes]') and OBJECTPROPERTY(id, N'IsTable') = 1)
DROP TABLE FurTypes

CREATE TABLE dbo.Cats
(
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	CatUrl NVARCHAR(250) NOT NULL,
	IsAStar BIT NOT NULL,
	IsTopOne BIT NOT NULL,
	IsAlone BIT NOT NULL,
	Rating FLOAT NOT NULL,
	Wins INT NOT NULL,
	ViewsNumber INT NOT NULL,
	ProbabilityWeight FLOAT NOT NULL,
	INDEX IX_IsAlone NONCLUSTERED (IsAlone),
	INDEX IX_IsAStar NONCLUSTERED (IsAStar),
	INDEX IX_IsTopOne NONCLUSTERED (IsTopOne)
)
CREATE TABLE dbo.FurTypes
(
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	FurType NVARCHAR(20) NOT NULL
)

CREATE TABLE dbo.CatsFurs
(
	CatId INT NOT NULL,
	FurTypeId INT NOT NULL,
	CONSTRAINT FK_CatsFurs_Cats FOREIGN KEY (CatId) REFERENCES dbo.Cats (Id)
	ON DELETE CASCADE
	ON UPDATE CASCADE,
	CONSTRAINT FK_CatsFurs_FurTypes FOREIGN KEY (FurTypeId) REFERENCES dbo.FurTypes (Id)
	ON DELETE CASCADE
	ON UPDATE CASCADE

)
END
GO