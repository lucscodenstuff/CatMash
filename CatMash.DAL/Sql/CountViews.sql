IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'CountViews')
DROP PROCEDURE CountViews
GO

CREATE PROCEDURE CountViews (@furTypeId int = null)
as
BEGIN
SET NOCOUNT ON

SELECT DISTINCT SUM(c.ViewsNumber)
FROM Cats c
JOIN CatsFurs cf on c.ID = cf.CatId
JOIN FurTypes f on cf.FurTypeId = f.Id
WHERE @furTypeId IS NULL OR f.Id = @furTypeId
GROUP BY c.ViewsNumber
END
GO

exec CountViews