 create proc proc_FilterProductswithcount(@pcpu varchar(20), @pcount int out)
  as
  begin
      set @pcount = (select count(*) from products where 
	  try_cast(json_value(details,'$.spec.cpu') as nvarchar(20)) =@pcpu)
  end

begin
	declare @count int
	exec proc_FilterProductswithcount 'i5', @count out
	print concat('The number of computers is ',@count)
end

select * from products


create table people(
	id int primary key,
	name nvarchar(20),
	age int
);
	
drop proc proc_BulkInsertQuery

create table bulk_insert_log(
	filepath nvarchar(max),
	status nvarchar(20) constraint chk_status check(status in('Success','Failure')),
	Message nvarchar(max),
	insertedOn Datetime Default GetDate()
);

create or alter proc proc_BulkInsertQuery(@Filepath nvarchar(max))
as
begin
	begin try
		declare @insertQuery nvarchar(max)
		set @insertQuery = 'BULK INSERT people from '''+@Filepath+'''
		with(
			FIRSTROW  = 2,
			FIELDTERMINATOR = '','',
			ROWTERMINATOR = ''\n''
		)'
		exec sp_executesql @insertQuery
	   insert into BulkInsertLog(filepath,status,message)
	   values(@filepath,'Success','Bulk insert completed')
	end try
	begin catch
		insert into bulk_insert_log(filepath,status,Message)
		values(@Filepath,'Failure',ERROR_MESSAGE())
	end catch

end

proc_BulkInsertQuery 'C:\Users\muthukaruppanp\Downloads\data.csv'

select * from people

select * from bulk_insert_log

with cte_Authors
as
	(select au_id, au_fname author_name from authors)
select * from cte_Authors

update cte_Authors set author_name= 'Gowtham' where au_id = '409-56-7008'

select * from authors where au_id = '409-56-7008'

select * from cte_Authors


create or alter proc proc_pagination( @page int , @pageSize int )
as
begin

with PaginatedBooks as
(
	select title_id,title,price,ROW_NUMBER() over (order by price desc) as RowNum
	from titles
)

select * from PaginatedBooks where RowNum between ((@page-1)*@pageSize+1) and (@page*@pageSize)
end

proc_pagination 2,5

select title_id,title,price from titles order by price desc offset 2 rows fetch next 1 rows only

create function fn_calculateTax(@baseprice int, @tax int)
returns float
as
begin
	return (@baseprice + (@baseprice*@tax/100))
end

select dbo.fn_calculateTax(1000,10)

select title,dbo.fn_calculateTax(price,10) as Total_Price from titles

create function fn_tablesample(@minprice float)
returns table
as 
	return select title,price from titles where price > @minprice


select * from fn_tablesample(10)


create function fn_tableSampleOld(@minprice float)
  returns @Result table(Book_Name nvarchar(100), price float)
  as
  begin
    insert into @Result select title,price from titles where price>= @minprice
    return 
end
select * from dbo.fn_tableSampleOld(10)