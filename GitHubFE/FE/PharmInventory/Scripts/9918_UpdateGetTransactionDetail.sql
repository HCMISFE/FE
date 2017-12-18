ALTER PROCEDURE [dbo].[GetTransactionDetails]
	  @storeid int,
	  @todate datetime,
	  @fromdate datetime
AS
BEGIN
select t.*
	from
	(select 
		case when vw.ID in (select distinct ItemID from ReceiveDoc where StoreID = @storeid) then 1 else 0 end as Received, 
		isnull(rd.Quantity,0) as ReceivedQty, 
		isnull(rd.Price,0) as ReceivedPrice, 
		--isnull(rd.QuantityLeft,0) as QuantityLeft, 
		--isnull(rd.price1,0) as QuantityLeftPrice, 
		isnull(id.Quantity,0) as IssuedQty, 
		isnull(id.Price,0) as IssuedPrice,  
		 ISNULL(bb.QuantityLeft,0) AS BBQty,
		ISNULL(bb.Price,0) AS BBPrice,
		 ISNULL(eb.QuantityLeft,0) AS QuantityLeft,
		ISNULL(eb.Price,0) AS QuantityLeftPrice,
	    ISNULL(rd.Quantity,0) + ISNULL(adj.Quantity,0) - ISNULL(id.Quantity,0) - ISNULL(loss.Quantity,0) AS SOH, 
		vw.ID, vw.FullItemName,vw.StockCode, 
		vw.Unit,vw.Name ,vw.TypeID AS TypeID
		FROM vwGetAllItems vw  
		LEFT JOIN 
		(SELECT ItemID , SUM(Quantity) Quantity, SUM(QuantityLeft) QuantityLeft, SUM(Quantity * Cost) Price ,SUM(QuantityLeft*Cost) price1 
		FROM ReceiveDoc WHERE StoreID = @storeid AND (date BETWEEN @fromdate AND @todate) GROUP BY ItemID ) AS rd ON rd.ItemID = vw.ID 
	LEFT JOIN 
		(SELECT ItemID ,  SUM(QuantityLeft) QuantityLeft, SUM(QuantityLeft * Cost) Price ,MAX(Date) receivedate
		FROM ReceiveDoc WHERE StoreID = @storeid AND (date <=  @fromdate  ) GROUP BY ItemID ) AS bb ON bb.ItemID = vw.ID 
	LEFT JOIN 
		(SELECT ItemID ,  SUM(QuantityLeft) QuantityLeft, SUM(QuantityLeft * Cost) Price ,MAX(Date) receivedate
		FROM ReceiveDoc WHERE StoreID = @storeid AND (date <=  @todate  ) GROUP BY ItemID ) AS eb ON eb.ItemID = vw.ID 
	 --	LEFT JOIN 
			--(SELECT ItemID , SUM(PhysicalInventory) Quantity FROM YearEnd WHERE StoreID = @storeid AND Year = YEAR(@fromdate)  GROUP BY ItemID ) AS bb ON bb.ItemID = vw.ID 
	      LEFT JOIN
		(SELECT ItemID , SUM(Quantity) Quantity FROM Disposal WHERE StoreID = @storeid AND (date BETWEEN @fromdate AND @todate) AND Losses = 1 GROUP BY ItemID ) AS loss ON loss.ItemID = vw.ID 
		LEFT JOIN
		(SELECT ItemID , SUM(Quantity) Quantity FROM Disposal WHERE StoreID = @storeid AND (date BETWEEN @fromdate AND @todate) AND Losses = 0 GROUP BY ItemID ) AS adj ON adj.ItemID = vw.ID 
	     LEFT JOIN  
		(select ItemID , sum(Quantity) Quantity, SUM(Quantity * Cost) Price 
		from IssueDoc where StoreID = @storeid and (date between @fromdate and @todate) group by ItemID ) as id on id.ItemID = vw.ID 
		where vw.IsInHospitalList = 1) t order by t.FullItemName
END


