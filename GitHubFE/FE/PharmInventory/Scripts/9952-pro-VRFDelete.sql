CREATE PROCEDURE [dbo].[proc_VRFDelete]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	DELETE
	FROM [VRF]
	WHERE
		[ID] = @ID
	SET @Err = @@Error
	RETURN @Err
END