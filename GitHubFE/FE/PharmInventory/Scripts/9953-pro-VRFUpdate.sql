CREATE PROCEDURE [dbo].[proc_VRFUpdate]
(
	@ID int,
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

	UPDATE [VRF]
	SET
		[FromMonth] = @FromMonth,
		[FromYear] = @FromYear,
		[ToMonth] = @ToMonth,
		[ToYear] = @ToYear,
		[DateOfSubmission] = @DateOfSubmission,
		[LastVRFStatus] = @LastVRFStatus,
		[VRFType] = @VRFType
	WHERE
		[ID] = @ID


	SET @Err = @@Error


	RETURN @Err
END

GO


