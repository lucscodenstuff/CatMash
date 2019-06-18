IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SelectOneCat')
DROP PROCEDURE SelectOneCat
GO

CREATE PROCEDURE SelectOneCat (@Id INT)
as
BEGIN
SET NOCOUNT ON
SELECT c.Id, c.CatUrl, c.IsAStar, c.IsTopOne, c.IsAlone, c.Rating, c.Wins, c.ViewsNumber, c.ProbabilityWeight, 0
FROM Cats c
WHERE c.Id = @Id

SELECT f.Id
FROM Cats c
JOIN CatsFurs cf on c.ID = cf.CatId
JOIN FurTypes f on cf.FurTypeId = f.Id
WHERE c.Id = @Id
END
GO
EXEC SelectOneCat 3