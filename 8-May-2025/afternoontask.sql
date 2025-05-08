use master;

--1) 1) List all orders with the customer name and the employee who handled the order.

select o.*,CONCAT(e.firstname,' ',e.lastname) as Employee,c.contactname
	from orders o
	join Customers c 
	on o.customerid = c.customerid
	join Employees e
	on o.employeeid = e.employeeid


--2) Get a list of products along with their category and supplier name.
select p.*,c.categoryname, s.contactname as supplier_name from Products p
	join Categories c
	on p.categoryid = c.categoryid
	join Suppliers s
	on p.supplierid = s.supplierid


--3) Show all orders and the products included in each order with quantity and unit price.
select o.*,p.*,od.unitprice,od.quantity from Orders o
	join OrderDetails od
	on o.orderid = od.orderid
	join Products p
	on od.productid = p.productid


--4) List employees who report to other employees (manager-subordinate relationship).
select e.employeeid as subordinate_id,CONCAT(e.firstname,' ',e.lastname) as subordinate_name,m.employeeid as manager_id,CONCAT(m.firstname,' ',m.lastname) as manager_name
	from Employees e
	join Employees m
	on e.reportsto = m.employeeid


--5) Display each customer and their total order count.
select c.customerid,c.contactname,count(*) as Order_Count from Customers c
	join Orders o
	on c.customerid = o.customerid
	group by c.customerid,c.contactname;


--6) Find the average unit price of products per category.
select c.categoryid,c.categoryname, avg(p.unitprice) as avg_price from Products p
	join Categories c
	on p.categoryid = c.categoryid
	group by c.categoryid,c.categoryname


--7) List customers where the contact title starts with 'Owner'.
select * from Customers
	where lower(contacttitle) like 'owner%'


--8) Show the top 5 most expensive products.
select productname,unitprice from Products 
	order by unitprice desc
	offset 0 row fetch next 5 rows only


select top 5 productname,unitprice from Products 
	order by unitprice desc



--9) Return the total sales amount (quantity × unit price) per order.
select o.orderid, sum(od.unitprice * od.quantity) as total_sales from Orders o
	join OrderDetails od
	on o.orderid = od.orderid
	group by o.orderid


--10) Create a stored procedure that returns all orders for a given customer ID.
create or alter proc proc_listorders(@cid nvarchar(5))
as
begin
	select * from Orders where customerid = @cid
end

begin
	declare @cusid nvarchar(5) = 'ANTON'
	exec proc_listorders @cusid
end


--11) Write a stored procedure that inserts a new product.
create proc proc_insert_product(@productname nvarchar(40),@sid int, @cid int,@qpr nvarchar(20),@uprice money,@uinst smallint,@uinord smallint,@lvl smallint,@dcnt bit)
as
begin
	insert into Products 
	(productname,supplierid,categoryid,quantityperunit,unitprice,unitsinstock,unitsonorder,reorderlevel,discontinued)
	Values
	 (@productname, @sid, @cid, @qpr, @uprice, @uinst, @uinord, @lvl, @dcnt);
end

EXEC proc_insert_product 
    @productname = 'Sample Product',
    @sid = 1,
    @cid = 1,
    @qpr = '10 units',
    @uprice = 15.99,
    @uinst = 100,
    @uinord = 50,
    @lvl = 10,
    @dcnt = 0;


--12) Create a stored procedure that returns total sales per employee.
CREATE PROCEDURE proc_total_sales (@eid INT)
AS
BEGIN
    SELECT 
        e.employeeid, 
        CONCAT(e.firstname, ' ', e.lastname) AS Emp_Name, 
        SUM(od.unitprice * od.quantity) AS total_Sales
    FROM 
        Employees e
    JOIN 
        Orders o ON e.employeeid = o.employeeid
    JOIN 
        OrderDetails od ON o.orderid = od.orderid
    WHERE 
        e.employeeid = @eid
    GROUP BY 
        e.employeeid, e.firstname, e.lastname;
END;

exec proc_total_sales 1


--13) Use a CTE to rank products by unit price within each category.
with RankProducts as
  (select productid,productname,categoryid,unitprice,
   Rank() over ( partition by categoryid order by unitprice desc) as Rank
   from Products
   )
select productid,productname,categoryid,unitprice,Rank from RankProducts order by categoryid,Rank;


--14) Create a CTE to calculate total revenue per product and filter products with revenue > 10,000.
WITH TotalRevenuePerProduct AS
(
    SELECT 
        p.productid, 
        p.productname, 
        SUM(od.unitprice * od.quantity) AS totalsales
    FROM 
        Products p
    JOIN 
        OrderDetails od ON p.productid = od.productid
    GROUP BY 
        p.productid, p.productname
)
SELECT 
    productid, 
    productname, 
    totalsales 
FROM 
    TotalRevenuePerProduct
WHERE
	totalsales > 10000;

--15)  Use a CTE with recursion to display employee hierarchy.
WITH RecursiveEmployeeRank AS
(
    
    SELECT employeeid, CONCAT(firstname, ' ', lastname) AS EmployeeName, 1 AS level
    FROM Employees
    WHERE reportsto IS NULL
    
    UNION ALL
    
    SELECT e.employeeid, 
           CONCAT(e.firstname, ' ', e.lastname) AS EmployeeName, 
           r.level + 1 AS level
    FROM Employees e
    JOIN RecursiveEmployeeRank r 
    ON e.reportsto = r.employeeid
)
SELECT * 
FROM RecursiveEmployeeRank;

-- Practice Qns on Cursor
use pubs;

BEGIN
    DECLARE @pub_id INT, @title NVARCHAR(MAX);

    DECLARE title_cursor CURSOR FOR
    SELECT pub_id, title
    FROM titles;

    OPEN title_cursor;

    FETCH NEXT FROM title_cursor INTO @pub_id, @title;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Display matching publisher info with the current title
        SELECT 
            p.*, 
            @title AS title
        FROM publishers p
        WHERE p.pub_id = @pub_id;

        FETCH NEXT FROM title_cursor INTO @pub_id, @title;
    END;

    CLOSE title_cursor;
    DEALLOCATE title_cursor;
END;


BEGIN
    DECLARE @title_name NVARCHAR(MAX);

    DECLARE title_cursor CURSOR SCROLL FOR
    SELECT title
    FROM titles;

    OPEN title_cursor;

    FETCH LAST FROM title_cursor INTO @title_name;
	print 'Last Row of titles : '+ @title_name+' of the book'

	FETCH FIRST FROM title_cursor INTO @title_name;
	print 'Fist Row of titles : '+ @title_name+' of the book'


    CLOSE title_cursor;
    DEALLOCATE title_cursor;
END;
