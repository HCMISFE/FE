INSERT INTO dbo.UserStore (   UserID ,
                              StoreID
                          )
            SELECT us.ID UserID ,
                   ty.ID StoreID
            FROM   dbo.[User] us
                   LEFT JOIN dbo.[Stores] ty ON 1 = 1
                   LEFT JOIN dbo.UserStore ut ON us.ID = ut.UserID
                                                 AND ty.ID = ut.StoreID
            WHERE  ut.ID IS NULL
                   AND 0 = (   SELECT COUNT(ut1.ID)
                               FROM   dbo.[User] us1
                                      LEFT JOIN dbo.[Stores] ty1 ON 1 = 1
                                      JOIN dbo.UserStore ut1 ON us1.ID = ut1.UserID
                                                                AND ty1.ID = ut1.StoreID
                           ) 