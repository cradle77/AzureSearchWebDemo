USE [AdventureWorks]
GO

/****** Object:  View [dbo].[CategoryHierarchy]    Script Date: 15/05/2016 12:20:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[CategoryHierarchy]
AS
WITH CatTree AS (
	SELECT	*, CAST(Name AS Varchar(MAX)) AS HierachyName
    FROM	SalesLT.ProductCategory
    WHERE	(ParentProductCategoryID IS NULL)
    
	UNION ALL
    
	SELECT	c.*, CAST(p.Name + '|' + c.Name AS varchar(MAX)) AS HierarchyName
    FROM	SalesLT.ProductCategory AS c INNER JOIN
			CatTree AS p ON c.ParentProductCategoryID = p.ProductCategoryID
    WHERE	(c.ParentProductCategoryID IS NOT NULL)
)

SELECT	*
FROM	CatTree 


GO

/****** Object:  View [dbo].[ProductsSearchView]    Script Date: 15/05/2016 12:20:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






/****** Script for SelectTopNRows command from SSMS  ******/
CREATE VIEW [dbo].[ProductsSearchView]
AS
SELECT	p.ProductID, 
		p.Name, 
		p.ProductNumber, 
		p.Color, 
		CAST(p.StandardCost AS float) AS StandardCost, 
		CAST(p.ListPrice AS float) AS ListPrice, 
		p.Size, 
		CAST(p.Weight AS float) as Weight, 
		p.ProductModelID, 
		dbo.ToBase64(p.ThumbNailPhoto) as ThumbNailPhoto,
		CASE WHEN DiscontinuedDate IS NULL THEN 0 ELSE 1 END AS IsDeleted, 
		c.HierachyName AS Category, 
		c.Name AS CategoryName, 
		m.Name AS ModelName, 
		d.Description
FROM   SalesLT.Product AS p INNER JOIN
           dbo.CategoryHierarchy AS c ON p.ProductCategoryID = c.ProductCategoryID INNER JOIN
           SalesLT.ProductModel AS m ON p.ProductModelID = m.ProductModelID LEFT OUTER JOIN
           SalesLT.ProductModelProductDescription AS md ON p.ProductModelID = md.ProductModelID AND md.Culture = 'en' LEFT OUTER JOIN
           SalesLT.ProductDescription AS d ON d.ProductDescriptionID = md.ProductDescriptionID






GO

