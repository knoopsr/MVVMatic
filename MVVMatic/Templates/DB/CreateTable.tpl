CREATE TABLE {Schema}.tbl{TableName}
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
{Columns}    
    CreatedBy NVARCHAR(100) NULL,
    CreatedOn DATETIME NULL,
    ChangedBy NVARCHAR(100) NULL,
    ChangedOn DATETIME NULL,
    DeletedBy NVARCHAR(100) NULL,
    DeletedOn DATETIME NULL,
    Deleted BIT NOT NULL DEFAULT 0,
    ControlField ROWVERSION
);
