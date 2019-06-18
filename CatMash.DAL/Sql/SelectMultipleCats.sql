IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SelectMultipleCats')
DROP PROCEDURE SelectMultipleCats
GO

CREATE PROCEDURE SelectMultipleCats (@isAStar bit = null, @furTypeId int = null, @isAlone bit = null)
as
BEGIN
SET NOCOUNT ON

SELECT DISTINCT c.Id, c.CatUrl, c.IsAStar, c.IsTopOne, c.IsAlone, c.Rating, c.Wins, c.ViewsNumber, c.ProbabilityWeight
FROM Cats c
JOIN CatsFurs cf on c.ID = cf.CatId
JOIN FurTypes f on cf.FurTypeId = f.Id
WHERE (@isAStar IS NULL OR c.IsAStar = @isAStar) 
	AND(@furTypeId IS NULL OR f.Id = @furTypeId)
	AND (@isAlone IS NULL OR c.IsAlone = @isAlone)
END
GO

exec SelectMultipleCats