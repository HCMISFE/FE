
CREATE PROCEDURE [dbo].[proc_ProgramProductLoadByPrimaryKey]
(
	@ID int
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Err int

	SELECT
		[ID],
		[StoreID],
		[ProgramID],
		[ItemID]
	FROM [ProgramProduct]
	WHERE
		([ID] = @ID)

	SET @Err = @@Error

	RETURN @Err
END
