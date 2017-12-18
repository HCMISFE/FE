
ALTER PROCEDURE [dbo].[proc_ReceiveDocLoadByPrimaryKey]
(
	@ID INT
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Err INT

	SELECT
		[ID],
		[BatchNo],
		[ItemID],
		[SupplierID],
		[Quantity],
		[Date],
		[ExpDate],
		[Out],
		[ReceivedStatus],
		[ReceivedBy],
		[Remark],
		[StoreID],
		[LocalBatchNo],
		[RefNo],
		[Cost],
		[IsApproved],
		[QuantityLeft],
		[NoOfPack],
		[QtyPerPack],
		[EurDate],
		[ManufacturerID],
		[BoxLevel],
		[SubProgramID],
		[UnitID],
		[InternalDrugCode]
	FROM [ReceiveDoc]
	WHERE
		([ID] = @ID)

	SET @Err = @@Error

	RETURN @Err
END


SET ANSI_NULLS ON
 


