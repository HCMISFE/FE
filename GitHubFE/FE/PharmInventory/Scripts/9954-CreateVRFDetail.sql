CREATE TABLE [dbo].[VRFDetail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[VRFID] [int] NULL,
	[StoreID] [int] NULL,
	[ItemID] [int] NULL,
	[RequestedQuantity] [int] NULL,
	[Doses] [int] NULL,
	[WasteFactor] [decimal](18, 0) NULL,
	[TargetCoverage] [int] NULL,
	[VaccinationGiven] [int] NULL,
	[Remark] [varchar](100) NULL,
 CONSTRAINT [PK_VRFDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]