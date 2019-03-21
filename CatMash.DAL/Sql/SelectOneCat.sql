IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'UpdateOneCat')
DROP PROCEDURE UpdateOneCat
GO

CREATE PROCEDURE UpdateOneCat (@Id INT, @views INT, @weight FLOAT, @rating FLOAT = null)
as
BEGIN
SET NOCOUNT ON
IF(@rating IS NOT NULL)
	UPDATE Cats
	SET ViewsNumber = @views, ProbabilityWeight = @weight , Rating = @rating
	WHERE Id = @Id;
ELSE
	UPDATE Cats
	SET ViewsNumber = @views, ProbabilityWeight = @weight
	WHERE Id = @Id;

EXEC SelectOneCat @Id;

END
GO