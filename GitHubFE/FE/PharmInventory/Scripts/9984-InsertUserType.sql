INSERT INTO dbo.UserCommodityType (   UserID ,
                                      TypeID
                                  ) 
  SELECT us.id UserID, ty.id TypeID FROM dbo.[User] us
LEFT JOIN   dbo.[Type] ty ON 1=1
LEFT JOIN dbo.UserCommodityType ut ON us.id = ut.UserID AND ty.id =ut.TypeID
WHERE ut.id IS NULL
AND 0= (
SELECT  COUNT(ut1.id) FROM dbo.[User] us1
  LEFT JOIN   dbo.[Type] ty1 ON 1=1
  JOIN dbo.UserCommodityType ut1 ON us1.id = ut1.UserID AND ty1.id =ut1.TypeID
)