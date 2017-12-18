CREATE PROCEDURE [dbo].[proc_VRFDetailDelete]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	DELETE
	FROM [VRFDetail]
	WHERE
		[ID] = @ID
	SET @Err = @@Error

	RETURN @Err
END