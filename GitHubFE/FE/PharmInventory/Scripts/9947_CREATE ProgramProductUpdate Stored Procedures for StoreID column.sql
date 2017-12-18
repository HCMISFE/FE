CREATE PROCEDURE [dbo].[proc_ProgramProductUpdate]
(
	@ID int,
	@StoreID int = NULL,
	@ProgramID int = NULL,
	@ItemID int = NULL
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	UPDATE [ProgramProduct]
	SET
		[StoreID] = @StoreID,
		[ProgramID] = @ProgramID,
		[ItemID] = @ItemID
	WHERE
		[ID] = @ID


	SET @Err = @@Error


	RETURN @Err
END


