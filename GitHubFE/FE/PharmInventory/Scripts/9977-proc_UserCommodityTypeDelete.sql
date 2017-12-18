CREATE PROCEDURE [proc_UserCommodityTypeDelete]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	DELETE
	FROM [UserCommodityType]
	WHERE
		[ID] = @ID
	SET @Err = @@Error

	RETURN @Err
END
