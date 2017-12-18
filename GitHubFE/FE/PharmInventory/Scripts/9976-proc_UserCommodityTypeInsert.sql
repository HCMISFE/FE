CREATE PROCEDURE [proc_UserCommodityTypeInsert]
(
	@ID int = NULL output,
	@UserID int,
	@TypeID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	INSERT
	INTO [UserCommodityType]
	(
		[UserID],
		[TypeID]
	)
	VALUES
	(
		@UserID,
		@TypeID
	)

	SET @Err = @@Error

	SELECT @ID = SCOPE_IDENTITY()

	RETURN @Err
END


 