ALTER TABLE [dbo].[UserStore]  WITH CHECK ADD  CONSTRAINT [FK_dbo_UserStore_StoreID] FOREIGN KEY([StoreID])
REFERENCES [dbo].[Stores] ([ID])
 ALTER TABLE [dbo].[UserStore] CHECK CONSTRAINT [FK_dbo_UserStore_StoreID] 