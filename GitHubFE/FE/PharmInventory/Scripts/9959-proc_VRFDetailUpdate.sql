CREATE PROCEDURE [dbo].[proc_VRFDetailUpdate]
(
	@ID int,
	@VRFID int = NULL,
	@StoreID int = NULL,
	@ItemID int = NULL,
	@RequestedQuantity int = NULL,
	@Doses int = NULL,
	@WasteFactor decimal =NULL,
	@TargetCoverage int= NULL,
	@VaccinationGiven int =NULL,
	@Remark varchar(100) =NULL
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	UPDATE [VRFDetail]
	SET
		[VRFID] = @VRFID,
		[StoreID] = @StoreID,
		[ItemID] = @ItemID,
		[RequestedQuantity] = @RequestedQuantity ,
		[Doses]=@Doses,
		[WasteFactor]=@WasteFactor,
		[TargetCoverage]=@TargetCoverage,
		[VaccinationGiven]=@VaccinationGiven,
		[Remark]=@Remark
	WHERE
		[ID] = @ID
	SET @Err = @@Error
	RETURN @Err
END
