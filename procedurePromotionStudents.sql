--DROP PROCEDURE IF EXISTS PromotionStudents;
ALTER PROCEDURE PromotionStudents
@Studies NVARCHAR(100),
@Semester INT,
@NewIdEnrollment INT OUTPUT
AS
BEGIN
    DECLARE @IdStudy INT = (SELECT TOP 1 IdStudy FROM Studies WHERE Name=@Studies);
    IF @IdStudy IS NULL
    BEGIN
        RAISERROR('Nie istnieją takie studia', 11,1);
    END;

    DECLARE @IdEnrollment INT = (SELECT TOP 1 IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudy AND Semester=@Semester);
    IF @IdEnrollment IS NULL
    BEGIN
        RAISERROR('Nie istnieje taki enroll', 11,1)
    END;

    DECLARE @NewSemester INT = @Semester + 1;
    set @NewIdEnrollment = (SELECT IdEnrollment FROM Enrollment WHERE IdStudy=@IdStudy AND Semester=@NewSemester);
    IF @NewIdEnrollment IS NULL
    BEGIN
        SET @NewIdEnrollment = (SELECT max(IdEnrollment) FROM Enrollment)+1;
        INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) VALUES (@NewIdEnrollment, @NewSemester, @IdStudy, SYSDATETIME());
    END;

    UPDATE Student SET IdEnrollment=@NewIdEnrollment WHERE IdEnrollment=@IdEnrollment;
END;