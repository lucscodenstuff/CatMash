CREATE TABLE dbo.Cats
(
	Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
	CatUrl NVARCHAR(250) NOT NULL,
	IsAStar BIT NOT NULL,
	IsTopOne BIT NOT NULL,
	IsAlone BIT NOT NULL,
	Rating DECIMAL NOT NULL,
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