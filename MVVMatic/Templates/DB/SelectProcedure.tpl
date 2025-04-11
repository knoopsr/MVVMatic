USE [{Database}]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER PROCEDURE [{Schema}].[S_{ProcedureName}]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
{SelectColumns}
        ControlField
    FROM [{Schema}].[tbl{TableName}]
    WHERE Deleted = 0
END
