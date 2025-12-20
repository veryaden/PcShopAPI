--尋找USERID是1的訂單主表 1
SELECT * FROM ORDERS WHERE UserID = '1'
--取得訂單主表資料後尋找OrderId來找到明細表 多
SELECT * FROM  OrderItems WHERE  OrderID = '1'
--一對多

--購物車也是同個概念
SELECT * FROM Cart where UserID = '1'
SELECT * 
FROM CartItems AS CI
JOIN ProductSKUs AS PSK ON CI.SKUID = PSK.SKUID
JOIN Products AS PD ON PSK.ProductID = PD.ProductID
WHERE CartID ='1'

