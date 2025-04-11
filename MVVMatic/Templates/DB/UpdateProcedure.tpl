USE [{Database}]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [{Schema}].[U_{ProcedureName}]
(
    @ID INT,
{Parameters}
    @ReturnValue INT OUTPUT,
    @ControlField ROWVERSION
)
AS

SET NOCOUNT OFF;

BEGIN
BEGIN TRY
    UPDATE [{Schema}].[tbl{TableName}]
    SET
{UpdateAssignments}
        ChangedBy = SYSTEM_USER,
        ChangeOn = GETDATE()
    WHERE (ID = @ID AND ControlField = @ControlField);

    IF @@ROWCOUNT > 0
        SET @ReturnValue = 1 -- OK
    ELSE
        SET @ReturnValue = 0 -- Concurrency
END TRY
BEGIN CATCH
    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_SEVERITY() AS ErrorSeverity,
        ERROR_STATE() AS ErrorState,
        ERROR_PROCEDURE() AS ErrorProcedure,
        ERROR_LINE() AS ErrorLine,
        ERROR_MESSAGE() AS ErrorMessage;

    SET @ReturnValue = ERROR_NUMBER()
END CATCH
END
