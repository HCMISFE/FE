CREATE PROCEDURE [dbo].[proc_VRFDetailLoadByPrimaryKey]
(
	@ID int
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Err int

	SELECT
	   [ID],
	   [VRFID] ,
       [StoreID] ,
       [ItemID],
       [RequestedQuantity],
       [Doses] ,
       [WasteFactor],
       [TargetCoverage] ,
       [VaccinationGiven],
       [Remark]
	FROM [VRFDetail]
	WHERE
		([ID] = @ID)
	SET @Err = @@Error
	RETURN @Err
END


