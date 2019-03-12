IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SelectAllCats')
DROP PROCEDURE SelectAllCats
GO

CREATE PROCEDURE SelectAllCats
as
BEGIN
SET NOCOUNT ON
SELECT c.Id, c.CatUrl, c.IsAStar, c.IsTopOne, c.IsAlone, c.Rating, 0
FROM Cats c

SELECT f.Id
FROM Cats c
JOIN CatsFurs cf on c.ID = cf.CatId
JOIN FurTypes f on cf.FurTypeId = f.Id
END
GO