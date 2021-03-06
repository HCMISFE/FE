ALTER PROCEDURE [dbo].[proc_IssueDocLoadAll]
AS
BEGIN

	SET NOCOUNT ON
	DECLARE @Err int

	SELECT
		[ID],
		[ItemID],
		[StoreId],
		[ReceivingUnitID],
		[LocalBatchNo],
		[Quantity],
		[Date],
		[IsTransfer],
		[IssuedBy],
		[Remark],
		[RefNo],
		[BatchNo],
		[IsApproved],
		[Cost],
		[NoOfPack],
		[QtyPerPack],
		[DUSOH],
		[DURequestedQty],
		[RecomendedQty],
		[RecievDocID],
		[EurDate],
		[OrderID],
		[UnitID],
		[RecipientName]
	FROM [IssueDoc]

	SET @Err = @@Error

	RETURN @Err
END
