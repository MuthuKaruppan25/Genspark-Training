use employee_db;


create table emp (empno int primary key ,
	empname varchar(100) not null ,
	salary int check (salary >= 0) ,
	department varchar(100)  , 
	bossno int ,
	foreign key (bossno) references emp(empno)
);

create table dept ( 
	deptname varchar(100) primary key ,
	deptfloor int  check (deptfloor >= 0) , 
	phoneno varchar(10) not null,empno int ,
	foreign key (empno)references emp(empno)
);

alter table emp add constraint fk_const foreign key (department)
references dept(deptname);

create table item ( 
	itemname varchar(100) primary key,
	itemtype varchar(100) not null , 
	itemcolor varchar(100) not null
);


create table sales ( 
	salesno int primary key ,
	qty int check (qty >= 1) ,
	itemname varchar(100) not null,
	deptname varchar(100) not null ,
	foreign key (deptname)references dept(deptname) ,
	foreign key (itemname)references item(itemname) 
);



INSERT INTO emp VALUES
(1, 'Alice', 75000, NULL, NULL),
(2, 'Ned', 45000, NULL, 1),
(3, 'Andrew', 25000, NULL, 2),
(4, 'Clare', 22000, NULL, 2),
(5, 'Todd', 38000, NULL, 1),
(6, 'Nancy', 22000, NULL, 5),
(7, 'Brier', 43000, NULL, 1),
(8, 'Sarah', 56000, NULL, 7),
(9, 'Sophile', 35000, NULL, 1),
(10, 'Sanjay', 15000, NULL, 3),
(11, 'Rita', 15000, NULL, 4),
(12, 'Gigi', 16000, NULL, 4),
(13, 'Maggie', 11000, NULL, 4),
(14, 'Paul', 15000, NULL, 3),
(15, 'James', 15000, NULL, 3),
(16, 'Pat', 15000, NULL, 3),
(17, 'Mark', 15000, NULL, 3);

INSERT INTO dept VALUES
('Management', 5, '34', 1),
('Books', 1, '81', 4),
('Clothes', 2, '24', 4),
('Equipment', 3, '57', 3),
('Furniture', 4, '14', 3),
('Navigation', 1, '41', 3),
('Recreation', 2, '29', 4),
('Accounting', 5, '35', 5),
('Purchasing', 5, '36', 7),
('Personnel', 5, '37', 9),
('Marketing', 5, '38', 2);


INSERT INTO item VALUES
('Pocket Knife-Nile', 'E', 'Brown'),
('Pocket Knife-Avon', 'E', 'Brown'),
('Compass', 'N', ''),
('Geo positioning system', 'N', ''),
('Elephant Polo stick', 'R', 'Bamboo'),
('Camel Saddle', 'R', 'Brown'),
('Sextant', 'N', ''),
('Map Measure', 'N', ''),
('Boots-snake proof', 'C', 'Green'),
('Pith Helmet', 'C', 'Khaki'),
('Hat-polar Explorer', 'C', 'White'),
('Exploring in 10 Easy Lessons', 'B', ''),
('Hammock', 'F', 'Khaki'),
('How to win Foreign Friends', 'B', ''),
('Map case', 'E', 'Brown'),
('Safari Chair', 'F', 'Khaki'),
('Safari cooking kit', 'F', 'Khaki'),
('Stetson', 'C', 'Black'),
('Tent - 2 person', 'F', 'Khaki'),
('Tent -8 person', 'F', 'Khaki');

UPDATE emp SET department = 'Management' WHERE empno = 1;
UPDATE emp SET department = 'Marketing' WHERE empno = 2;
UPDATE emp SET department = 'Marketing' WHERE empno = 3;
UPDATE emp SET department = 'Marketing' WHERE empno = 4;
UPDATE emp SET department = 'Accounting' WHERE empno = 5;
UPDATE emp SET department = 'Accounting' WHERE empno = 6;
UPDATE emp SET department = 'Purchasing' WHERE empno = 7;
UPDATE emp SET department = 'Purchasing' WHERE empno = 8;
UPDATE emp SET department = 'Personnel' WHERE empno = 9;
UPDATE emp SET department = 'Navigation' WHERE empno = 10;
UPDATE emp SET department = 'Books' WHERE empno = 11;
UPDATE emp SET department = 'Clothes' WHERE empno = 12;
UPDATE emp SET department = 'Clothes' WHERE empno = 13;
UPDATE emp SET department = 'Equipment' WHERE empno = 14;
UPDATE emp SET department = 'Equipment' WHERE empno = 15;
UPDATE emp SET department = 'Furniture' WHERE empno = 16;
UPDATE emp SET department = 'Recreation' WHERE empno = 17;


INSERT INTO sales VALUES
(101, 2, 'Boots-snake proof', 'Clothes'),
(102, 1, 'Pith Helmet', 'Clothes'),
(103, 1, 'Sextant', 'Navigation'),
(104, 3, 'Hat-polar Explorer', 'Clothes'),
(105, 5, 'Pith Helmet', 'Equipment'),
(106, 2, 'Pocket Knife-Nile', 'Clothes'),
(107, 3, 'Pocket Knife-Nile', 'Recreation'),
(108, 1, 'Compass', 'Navigation'),
(109, 2, 'Geo positioning system', 'Navigation'),
(110, 5, 'Map Measure', 'Navigation'),
(111, 1, 'Geo positioning system', 'Books'),
(112, 1, 'Sextant', 'Books'),
(113, 3, 'Pocket Knife-Nile', 'Books'),
(114, 1, 'Pocket Knife-Nile', 'Navigation'),
(115, 1, 'Pocket Knife-Nile', 'Equipment'),
(116, 1, 'Sextant', 'Clothes'),
(117, 1, 'Sextant', 'Equipment'),
(118, 1, 'Sextant', 'Recreation'),
(119, 1, 'Sextant', 'Furniture'),
(120, 1, 'Pocket Knife-Nile', 'Furniture'),
(121, 1, 'Exploring in 10 Easy Lessons', 'Books'),
(122, 1, 'How to win Foreign Friends', 'Books'),
(123, 1, 'Compass', 'Books'),
(124, 1, 'Pith Helmet', 'Books'),
(125, 1, 'Elephant Polo stick', 'Recreation'),
(126, 1, 'Camel Saddle', 'Recreation');


select * from emp;
select * from dept;
select * from item;
select * from sales;