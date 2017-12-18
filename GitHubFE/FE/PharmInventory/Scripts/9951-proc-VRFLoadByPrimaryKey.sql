CREATE PROCEDURE [dbo].[proc_VRFLoadByPrimaryKey]
(
	@ID int
)
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
	WHERE
		([ID] = @ID)
	SET @Err = @@Error
	RETURN @Err
END