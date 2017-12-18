
--update ProgramProduct
--set StoreId = (select top 1 StoreID 
--				from ReceiveDoc rd 
--				where rd.ItemID = ProgramProduct.ItemID 
--						and rd.SubProgramID = ProgramProduct.ProgramID
--				order by rd.ID desc)