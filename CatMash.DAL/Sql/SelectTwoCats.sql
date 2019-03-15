IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'SelectTwoCats')
DROP PROCEDURE SelectTwoCats
GO

CREATE PROCEDURE SelectTwoCats (@catOneId int, @catTwoID int, @furType int = null)
as
BEGIN
SET NOCOUNT ON

SELECT DISTINCT c.Id, c.CatUrl, c.IsAStar, c.IsTopOne, c.IsAlone, c.Rating, c.ViewsNumber, c.ProbabilityWeight
FROM Cats c
JOIN CatsFurs cf on c.ID = cf.CatId
JOIN FurTypes f on cf.FurTypeId = f.Id
WHERE c.Id = @catOneId OR c.Id = @catTwoID
END
GO
exec SelectTwoCats 3 , 5