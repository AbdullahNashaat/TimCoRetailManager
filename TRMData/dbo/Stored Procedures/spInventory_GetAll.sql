CREATE PROCEDURE [dbo].[spInventory_GetAll]
	as
begin
	set nocount on;
	SELECT [ProductId], [Quantity], [PurchasePrice], [PurchaseDate] 
	from dbo.Inventory;
end
