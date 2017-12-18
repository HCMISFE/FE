CREATE PROCEDURE [dbo].[proc_VRFLoadAll]
AS
BEGIN

	SET NOCOUNT ON
	DECLARE @Err int

	SELECT
		[ID],
		[FromMonth],
		[FromYear],
		[ToMonth],
		[ToYear],
		[DateOfSubmission],
		[LastVRFStatus],
		[VRFType]
	FROM [VRF]
	SET @Err = @@Error
	RETURN @Err
END
