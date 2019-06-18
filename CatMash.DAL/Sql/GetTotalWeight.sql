IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetTotalWeight')
DROP PROCEDURE GetTotalWeight
GO

CREATE PROCEDURE GetTotalWeight
as
BEGIN
SET NOCOUNT ON

SELECT DISTINCT SUM(c.ProbabilityWeight) as totalWeight
FROM Cats c
JOIN CatsFurs cf on c.ID = cf.CatId
JOIN FurTypes f on cf.FurTypeId = f.Id
END
GO

exec GetTotalWeight