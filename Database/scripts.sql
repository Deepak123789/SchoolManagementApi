
CREATE TABLE Users(
    Id INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(100) UNIQUE,
    PasswordHash NVARCHAR(500)
);

GO

CREATE PROCEDURE sp_GetUserByUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SELECT Id, Username, PasswordHash FROM Users WHERE Username=@Username
END

GO

CREATE PROCEDURE sp_CreateUser
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(500)
AS
BEGIN
    INSERT INTO Users(Username,PasswordHash) VALUES(@Username,@PasswordHash)
END
