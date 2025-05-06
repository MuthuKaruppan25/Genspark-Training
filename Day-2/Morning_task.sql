select title as Title_name from titles;
select title as Title_name from titles where pub_id = 1389; 
select * from titles where price between 10 and 15;
select * from titles where price is null;
select * from titles where title like 'the%';
select * from titles where lower(title) not like '%v%';
select * from titles order by royalty;
select * from titles order by pub_id desc, type asc, price desc;
select type, avg(price) from titles group by type;
select distinct type from titles;
select top 2 * from titles order by price desc;
select * from titles where type ='business' and price < 20 and advance > 7000;
select pub_id, count(pub_id) as no_of_books from titles where price between 15 and 25 and lower(title) like '%it%' group by pub_id having count(pub_id) >= 2 order by no_of_books;
Select CONCAT(au_fname, au_lname) as AuthorName from authors where state = 'CA';
select state,count(state) as count_in_state from authors group by state;



use product_supplier;


CREATE TABLE Category (
    id INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL unique
);


CREATE TABLE Countries (
    id INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL unique
);


CREATE TABLE State (
    id INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL unique,
    country_id INT NOT NULL,
    FOREIGN KEY (country_id) REFERENCES Countries(id)
);

CREATE TABLE City (
    id INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL unique,
    state_id INT NOT NULL,
    FOREIGN KEY (state_id) REFERENCES State(id)
);

CREATE TABLE Area (
    zipcode VARCHAR(10) PRIMARY KEY,
    name VARCHAR(100) NOT NULL unique,
    city_id INT NOT NULL,
    FOREIGN KEY (city_id) REFERENCES City(id)
);

CREATE TABLE Address (
    id INT PRIMARY KEY,
    door_number VARCHAR(50),
    addressline1 VARCHAR(255),
    zipcode VARCHAR(10),
    FOREIGN KEY (zipcode) REFERENCES Area(zipcode)
);

create table supplier(
	id int primary key,
	name varchar(100) not null,
	contact_person varchar(100),
	phone varchar(20) unique,
	email varchar(50) unique,
	address_id int,
	status VARCHAR(50),
    FOREIGN KEY (address_id) REFERENCES Address(id)
);
create table product(
	id int primary key,
	name varchar(50),
	unit_price decimal (10,2),
	quantity int,
	category_id int,
	description varchar(200),
	image varchar(250)
	foreign key (category_id) references Category(id)
);

CREATE TABLE Product_Supplier (
    transaction_id INT PRIMARY KEY,
    product_id INT NOT NULL,
    supplier_id INT NOT NULL,
    date_of_supply DATE,
    quantity INT,
    FOREIGN KEY (product_id) REFERENCES Product(id),
    FOREIGN KEY (supplier_id) REFERENCES Supplier(id)
);

CREATE TABLE Customer (
    id INT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    phone VARCHAR(20),
    age INT,
    address_id INT,
    FOREIGN KEY (address_id) REFERENCES Address(id)
);

CREATE TABLE Orders (
    order_number INT PRIMARY KEY,
    customer_id INT NOT NULL,
    date_of_order DATE,
    amount DECIMAL(10, 2),
    order_status VARCHAR(50),
    FOREIGN KEY (customer_id) REFERENCES Customer(id)
);

CREATE TABLE Order_Details (
    id INT PRIMARY KEY,
    order_number INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT,
    unit_price DECIMAL(10, 2),
    FOREIGN KEY (order_number) REFERENCES Orders(order_number),
    FOREIGN KEY (product_id) REFERENCES Product(id)
);






