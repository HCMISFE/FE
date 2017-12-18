ALTER TABLE [dbo].[UserCommodityType]  WITH CHECK ADD  CONSTRAINT [FK_dbo_UserCommodityType_UserID] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
ALTER TABLE [dbo].[UserCommodityType] CHECK CONSTRAINT [FK_dbo_UserCommodityType_UserID]
 
 