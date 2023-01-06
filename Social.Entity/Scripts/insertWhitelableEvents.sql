USE [DataBaseName]
GO
Update [dbo].[EventTypeList] set Name ='WhiteLable-Public' where id =5

INSERT INTO [dbo].[EventTypeList]
           ([entityID]
           ,[Name]
           ,[color]
           ,[key])
     VALUES
           (newid(),'WhiteLable-Private','#3f4254',0)
GO


