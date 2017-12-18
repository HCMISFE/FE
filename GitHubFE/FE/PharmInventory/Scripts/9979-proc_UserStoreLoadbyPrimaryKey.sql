CREATE PROCEDURE [proc_UserStoreLoadByPrimaryKey]
(
	@ID int
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Err int

	SELECT
		[ID],
		[UserID],
		[StoreID]
	FROM [UserStore]
	WHERE
		([ID] = @ID)

	SET @Err = @@Error

	RETURN @Err
END
