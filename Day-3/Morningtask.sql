use pubs;

select title,pub_name 
	from titles left join publishers
	on titles.pub_id = publishers.pub_id;


select title,pub_name 
	from titles right outer join publishers
	on titles.pub_id = publishers.pub_id;

--select the author id for all the books. print the author id and book name
select au_id, title from titleauthor 
	join titles 
	on titleauthor.title_id=titles.title_id; 


select authors.au_id, title,CONCAT(au_fname,'',au_lname) as Name_of_Author from titleauthor 
	join authors
	on titleauthor.au_id = authors.au_id
	join titles 
	on titleauthor.title_id=titles.title_id; 

select pub_name publishername, title bookname, ord_date OrderDate 
	from publishers p 
	join titles t
	on p.pub_id = t.pub_id
	join sales s
	on t.title_id = s.title_id;

select  p.pub_name Publisher_Name,min(s.ord_date) Order_Date
	from publishers p
	join titles t
	on p.pub_id = t.pub_id
	join sales s
	on t.title_id = s.title_id
	group by p.pub_name
	order by 2 desc;

select pub_name as Publisher_Name, MIN(ord_date) as First_Order_Date
from publishers 
	left outer join titles on publishers.pub_id = titles.pub_id
	left outer join sales on sales.title_id = titles.title_id
	group by pub_name
	order by 2 desc;

select pub_name as Publisher_Name, MIN(ord_date) as First_Order_Date
from publishers 
	right outer join titles on publishers.pub_id = titles.pub_id
	left outer join sales on sales.title_id = titles.title_id
	group by pub_name
	order by 2 desc;

select title book_name, stor_address as store_address
	from titles t
	join sales s
	on t.title_id = s.title_id
	join stores st
	on s.stor_id = st.stor_id;

select title, st.stor_address from titles t 
	left join sales s 
	on t.title_id = s.title_id 
	left join stores st 
	on s.stor_id = st.stor_id
	order by 2 desc;

create procedure proc_firstProcedure
as
begin
	print 'Hello World'
end

exec proc_firstProcedure

create table products(
	id int identity(1,1) constraint pk_product primary key,
	name nvarchar(100) not null,
	details nvarchar(max)
);

create proc proc_storeproduct(@pname nvarchar(100),@pdetails nvarchar(max))
as
begin
	insert into products(name,details) values(@pname,@pdetails)
end
go
proc_storeproduct 'Laptop','{"brand":"Dell","spec":{"ram":"16GB","cpu":"i5"}}'

select * from products;

select id, name, JSON_QUERY(details,'$.spec')product_spec from products;

create proc proc_alterproduct(@pid int,@newvalue nvarchar(max))
as
begin
	update products set details = JSON_MODIFY(details,'$.spec.ram',@newvalue) where id = @pid
end

proc_alterproduct 2,'24GB'

select id, name, JSON_VALUE(details,'$.brand')product_spec from products;



create table posts (
	id int primary key,
	userId int,
	title nvarchar(100),
	body nvarchar(max)
)

create proc proc_bulkinsert(@jsondata nvarchar(max))
as
begin
	insert into posts(id,userId,title,body)
	select id,userId,title,body from openjson(@jsondata)
	with (userId int, id int, title nvarchar(100),body nvarchar(max))
end

begin
declare @jsonData nvarchar(max) = '[
  {
    "userId": 1,
    "id": 1,
    "title": "sunt aut facere repellat provident occaecati excepturi optio reprehenderit",
    "body": "quia et suscipit\nsuscipit recusandae consequuntur expedita et cum\nreprehenderit molestiae ut ut quas totam\nnostrum rerum est autem sunt rem eveniet architecto"
  },
  {
    "userId": 1,
    "id": 2,
    "title": "qui est esse",
    "body": "est rerum tempore vitae\nsequi sint nihil reprehenderit dolor beatae ea dolores neque\nfugiat blanditiis voluptate porro vel nihil molestiae ut reiciendis\nqui aperiam non debitis possimus qui neque nisi nulla"
  },
  {
    "userId": 1,
    "id": 3,
    "title": "ea molestias quasi exercitationem repellat qui ipsa sit aut",
    "body": "et iusto sed quo iure\nvoluptatem occaecati omnis eligendi aut ad\nvoluptatem doloribus vel accusantium quis pariatur\nmolestiae porro eius odio et labore et velit aut"
  },
  {
    "userId": 1,
    "id": 4,
    "title": "eum et est occaecati",
    "body": "ullam et saepe reiciendis voluptatem adipisci\nsit amet autem assumenda provident rerum culpa\nquis hic commodi nesciunt rem tenetur doloremque ipsam iure\nquis sunt voluptatem rerum illo velit"
  }
]'

EXEC proc_bulkinsert @jsonData;
end

select * from posts;

select * from products where
	try_cast(JSON_VALUE(details,'$.spec.cpu') as nvarchar(20)) = 'i5'

create proc proc_fetchuserbyid (@uid int)
as 
begin
	select * from posts where userId = @uid
end
go
proc_fetchuserbyid 1
