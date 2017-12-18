 CREATE PROCEDURE [proc_UserStoreDelete]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	DELETE
	FROM [UserStore]
	WHERE
		[ID] = @ID
	SET @Err = @@Error

	RETURN @Err
END
 
 

