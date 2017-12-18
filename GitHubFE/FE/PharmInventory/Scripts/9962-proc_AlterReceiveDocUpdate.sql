 ALTER PROCEDURE [dbo].[proc_ReceiveDocUpdate]
(
	@ID INT,
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
	@UnitID INT =NULL,
	@InternalDrugCode nvarchar(30) = NULL
)
AS
BEGIN

	SET NOCOUNT OFF
	DECLARE @Err INT

	UPDATE [ReceiveDoc]
	SET
		[BatchNo] = @BatchNo,
		[ItemID] = @ItemID,
		[SupplierID] = @SupplierID,
		[Quantity] = @Quantity,
		[Date] = @Date,
		[ExpDate] = @ExpDate,
		[Out] = @Out,
		[ReceivedStatus] = @ReceivedStatus,
		[ReceivedBy] = @ReceivedBy,
		[Remark] = @Remark,
		[StoreID] = @StoreID,
		[LocalBatchNo] = @LocalBatchNo,
		[RefNo] = @RefNo,
		[Cost] = @Cost,
		[IsApproved] = @IsApproved,
		[QuantityLeft] = @QuantityLeft,
		[NoOfPack] = @NoOfPack,
		[QtyPerPack] = @QtyPerPack,
		[EurDate] = @EurDate,
		[ManufacturerID] = @ManufacturerID,
		[BoxLevel] = @BoxLevel,
		[UnitID] =@UnitID,
		[SubProgramID] =@SubProgramID,
		[InternalDrugCode]   = @InternalDrugCode
	WHERE
		[ID] = @ID


	SET @Err = @@Error


	RETURN @Err
END


SET ANSI_NULLS ON
 