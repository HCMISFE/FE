
CREATE PROCEDURE [dbo].[proc_ProgramProductDelete]
(
	@ID int
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	DELETE
	FROM [ProgramProduct]
	WHERE
		[ID] = @ID
	SET @Err = @@Error

	RETURN @Err
END
