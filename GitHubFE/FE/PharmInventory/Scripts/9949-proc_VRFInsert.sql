CREATE PROCEDURE [dbo].[proc_VRFInsert]
(
	@ID int = NULL output,
	@FromMonth int,
	@FromYear int,
	@ToMonth int,
	@ToYear int,
	@DateOfSubmission datetime = NULL,
	@LastVRFStatus varchar(100) = NULL,
	@VRFType int = NULL
)
AS
BEGIN
	SET NOCOUNT OFF
	DECLARE @Err int

	INSERT
	INTO [VRF]
	(
		[FromMonth],
		[FromYear],
		[ToMonth],
		[ToYear],
		[DateOfSubmission],
		[LastVRFStatus],
		[VRFType]
	)
	VALUES
	(
		@FromMonth,
		@FromYear,
		@ToMonth,
		@ToYear,
		@DateOfSubmission,
		@LastVRFStatus,
		@VRFType
	)

	SET @Err = @@Error

	SELECT @ID = SCOPE_IDENTITY()

	RETURN @Err
END
