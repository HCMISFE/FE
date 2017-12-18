  
 ALTER PROCEDURE [dbo].[proc_ReceiveDocInsert]
(
	@ID INT = NULL OUTPUT,
	@BatchNo VARCHAR(50) = NULL,
	@ItemID INT = NULL,
	@SupplierID INT = NULL,
	@Quantity BIGINT = NULL,
	@Date DATETIME = NULL,
	@ExpDate DATETIME = NULL,
	@Out BIT = NULL,
	@ReceivedStatus INT = NULL,
	@ReceivedBy VARCHAR(50) = NULL,
	@Remark TEXT = NULL,
	@StoreID INT = NULL,
	@LocalBatchNo VARCHAR(50) = NULL,
	@RefNo VARCHAR(50) = NULL,
	@Cost FLOAT = NULL,
	@IsApproved BIT = NULL,
	@QuantityLeft BIGINT = NULL,
	@NoOfPack INT = NULL,
	@QtyPerPack INT = NULL,
	@EurDate DATETIME = NULL,
	@ManufacturerID INT = NULL,
	@BoxLevel INT = NULL,
	@SubProgramID INT = NULL,
	@UnitID INT = NULL,
	@InternalDrugCode nvarchar(30) = NULL
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err int

	INSERT
	INTO [ReceiveDoc]
	(
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
	)
	VALUES
	(
		@BatchNo,
		@ItemID,
		@SupplierID,
		@Quantity,
		@Date,
		@ExpDate,
		@Out,
		@ReceivedStatus,
		@ReceivedBy,
		@Remark,
		@StoreID,
		@LocalBatchNo,
		@RefNo,
		@Cost,
		@IsApproved,
		@QuantityLeft,
		@NoOfPack,
		@QtyPerPack,
		@EurDate,
		@ManufacturerID,
		@BoxLevel,
		@SubProgramID,
		@UnitID,
		@InternalDrugCode
	)

	SET @Err = @@Error

	SELECT @ID = SCOPE_IDENTITY()

	RETURN @Err
END
 