CREATE PROCEDURE [proc_UserStoreInsert]
(
	@ID int = NULL output,
	@UserID int,
	@StoreID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	INSERT
	INTO [UserStore]
	(
		[UserID],
		[StoreID]
	)
	VALUES
	(
		@UserID,
		@StoreID
	)

	SET @Err = @@Error

	SELECT @ID = SCOPE_IDENTITY()

	RETURN @Err
END


 
