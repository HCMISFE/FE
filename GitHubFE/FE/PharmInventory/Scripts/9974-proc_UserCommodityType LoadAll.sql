CREATE PROCEDURE [proc_UserCommodityTypeLoadAll]
AS
BEGIN

	SET NOCOUNT ON
	DECLARE @Err int

	SELECT
		[ID],
		[UserID],
		[TypeID]
	FROM [UserCommodityType]

	SET @Err = @@Error

	RETURN @Err
END
 