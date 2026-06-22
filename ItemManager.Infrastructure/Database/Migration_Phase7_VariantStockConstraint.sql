ALTER TABLE ItemStock
ADD CONSTRAINT UQ_ItemStock_Item_Location_Variant
UNIQUE (ItemID, LocationID, ItemVariantID);