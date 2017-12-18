CREATE PROCEDURE [proc_UserStoreUpdate]
(
	@ID int,
	@UserID int,
	@StoreID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	UPDATE [UserStore]
	SET
		[UserID] = @UserID,
		[StoreID] = @StoreID
	WHERE
		[ID] = @ID


	SET @Err = @@Error


	RETURN @Err
END
 
 
