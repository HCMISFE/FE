CREATE PROCEDURE [dbo].[proc_VRFDetailLoadAll]
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
	SET @Err = @@Error
	RETURN @Err
END


