
CREATE PROCEDURE [dbo].[proc_ProgramProductLoadAll]
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

	SET @Err = @@Error

	RETURN @Err
END
