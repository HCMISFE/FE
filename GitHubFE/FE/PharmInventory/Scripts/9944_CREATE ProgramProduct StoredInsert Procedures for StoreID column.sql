
/****** Object:  StoredProcedure [dbo].[proc_ProgramProductInsert]    Script Date: 12/23/2014 12:17:50 PM ******/

CREATE PROCEDURE [dbo].[proc_ProgramProductInsert]
(
	@ID int = NULL output,
	@StoreID int = NULL,
	@ProgramID int = NULL,
	@ItemID int = NULL
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	INSERT
	INTO [ProgramProduct]
	(
		[StoreID],
		[ProgramID],
		[ItemID]
	)
	VALUES
	(
		@StoreID,
		@ProgramID,
		@ItemID
	)

	SET @Err = @@Error

	SELECT @ID = SCOPE_IDENTITY()

	RETURN @Err
END

