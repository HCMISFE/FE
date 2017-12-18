CREATE PROCEDURE [proc_UserStoreLoadAll]
AS
BEGIN

	SET NOCOUNT ON
	DECLARE @Err int

	SELECT
		[ID],
		[UserID],
		[StoreID]
	FROM [UserStore]

	SET @Err = @@Error

	RETURN @Err
END

 
