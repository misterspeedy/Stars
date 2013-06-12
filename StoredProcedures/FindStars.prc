IF OBJECT_ID('FindStars', 'P') IS NOT NULL
	DROP PROCEDURE FindStars
GO
CREATE PROCEDURE FindStars @MinRA float, @MaxRA float, @MinDec float, @MaxDec float, @BrighterThan float AS
	SELECT * FROM StarLocations SL 
	WHERE SL.RA BETWEEN @MinRA AND @MaxRA 
	AND SL.Dec BETWEEN @MinDec AND @MaxDec
	AND SL.Mag <= @BrighterThan
GO