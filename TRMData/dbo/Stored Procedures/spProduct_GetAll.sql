CREATE PROCEDURE [dbo].[spProduct_GetAll]
	as
begin
	set nocount on;

	SELECT Id, ProductName, [Description], RetailPrice, QuantityInStock, IsTaxable
	from dbo.Product
end
