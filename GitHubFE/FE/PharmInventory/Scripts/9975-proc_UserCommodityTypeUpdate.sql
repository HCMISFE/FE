 CREATE PROCEDURE [proc_UserCommodityTypeUpdate]
(
	@ID int,
	@UserID int,
	@TypeID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	UPDATE [UserCommodityType]
	SET
		[UserID] = @UserID,
		[TypeID] = @TypeID
	WHERE
		[ID] = @ID


	SET @Err = @@Error


	RETURN @Err
END
 
