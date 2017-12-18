 CREATE PROCEDURE [proc_UserCommodityTypeLoadByPrimaryKey]
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
		[TypeID]
	FROM [UserCommodityType]
	WHERE
		([ID] = @ID)

	SET @Err = @@Error

	RETURN @Err
END
   