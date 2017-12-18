CREATE PROCEDURE [dbo].[proc_VRFDetailInsert]
(
	@ID int = NULL output,
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

	INSERT
	INTO [VRFDetail]
	(
		[VRFID],
		[StoreID],
		[ItemID],
		[RequestedQuantity],
		[Doses],
	    [WasteFactor],
	    [TargetCoverage],
	    [VaccinationGiven],
	    [Remark]
	)
	VALUES
	(
       @VRFID ,
       @StoreID ,
       @ItemID,
       @RequestedQuantity,
       @Doses ,
       @WasteFactor,
       @TargetCoverage ,
       @VaccinationGiven,
       @Remark
	)
	SET @Err = @@Error
	SELECT @ID = SCOPE_IDENTITY()
	RETURN @Err
END



