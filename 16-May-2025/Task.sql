--Phase 1

/* 
Tables to Design (Normalized to 3NF):

1. **students**

   * `student_id (PK)`, `name`, `email`, `phone`

2. **courses**

   * `course_id (PK)`, `course_name`, `category`, `duration_days`

3. **trainers**

   * `trainer_id (PK)`, `trainer_name`, `expertise`

4. **enrollmentsnrollment**

   * `enrollment_id (PK)`, `student_id (FK)`, `course_id (FK)`, `enroll_date`

5. **certificates**

   * `certificate_id (PK)`, `enrollment_id (FK)`, `issue_date`, `serial_no`

6. **course\_trainers** (Many-to-Many if needed)

   * `course_id`, `trainer_id`
*/

-- Phase 2

create extension if not exists pgcrypto;
create table student(
	student_id serial constraint pk_student_id primary key,
	full_name varchar(100) not null,
	email varchar(100) unique not null,
	phone varchar(10) unique not null
);

create table courses(
	course_id serial constraint pk_course_id primary key,
	course_name varchar(100) unique not null,
	category varchar(50) not null,
	duration_days numeric constraint chk_duration check(duration_days > 0)
);

create table trainers(
	trainer_id serial constraint pk_trainer_id primary key,
	trainer_name varchar(100) not null,
	expertise text not null
);

create table enrollment(
	enrollment_id serial constraint pk_enrollment_id primary key,
	student_id int,
	course_id int,
	enroll_date timestamp default current_timestamp,
	constraint fk_student_id foreign key (student_id) references student(student_id),
	constraint fk_course_id foreign key (course_id) references courses(course_id),
	constraint uq_student_course unique(student_id,course_id)
);

Create table certificates (
    certificate_id SERIAL CONSTRAINT pk_certificate_id PRIMARY KEY,
    enrollment_id INT,
    serial_no VARCHAR(12) UNIQUE DEFAULT ENCODE(gen_random_bytes(6), 'hex'), 
    issue_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_enrollment_id FOREIGN KEY (enrollment_id) REFERENCES enrollment(enrollment_id)
);

create table course_trainer(
    course_trainer_id serial constraint pk_course_trainer_id primary key,
 	course_id int,
	trainer_id int,
	constraint fk_course_id foreign key (course_id) references courses(course_id),
	constraint fk_trainer_id foreign key (trainer_id) references trainers(trainer_id),
	constraint uq_student_trainer unique(course_id,trainer_id)
);


--INDEX

-- Email
create index idx_email on student(email);

-- Student_id
create index idx_student_id on enrollment(student_id);

-- Course_id
create index idx_course_id on enrollment(course_id);

-- enrollment_id
create index idx_enrollment_id on certificates(enrollment_id);

-- course_id teacher course
create index idx_teacher_course_id on course_trainer(course_id);

--trainer_id 
create index idx_trainer_id on course_trainer(trainer_id);

--serial no
create index idx_serial_no on certificates(serial_no);

--Population of Data

INSERT INTO student (full_name, email, phone) VALUES
('Alice Johnson', 'alice@example.com', '1234567890'),
('Bob Smith', 'bob@example.com', '2345678901'),
('Charlie Brown', 'charlie@example.com', '3456789012');

INSERT INTO student (full_name, email, phone) VALUES
('David Miller', 'david.m@example.com', '4567890123'),
('Evelyn Wright', 'evelyn.w@example.com', '5678901234'),
('Frank Harris', 'frank.h@example.com', '6789012345'),
('Grace Lee', 'grace.l@example.com', '7890123456'),
('Hannah Walker', 'hannah.w@example.com', '8901234567');


INSERT INTO courses (course_name, category, duration_days) VALUES
('Introduction to SQL', 'Database', 10),
('Advanced Python', 'Programming', 15),
('Web Development Basics', 'Web', 12);

INSERT INTO courses (course_name, category, duration_days) VALUES
('Data Structures', 'Programming', 20),
('Machine Learning Basics', 'AI', 25),
('UI/UX Design', 'Design', 14),
('Cloud Fundamentals', 'Cloud', 18),
('Cybersecurity Essentials', 'Security', 16);



INSERT INTO trainers (trainer_name, expertise) VALUES
('David Miller', 'Databases, SQL'),
('Emily Clark', 'Python, Machine Learning'),
('Frank Moore', 'HTML, CSS, JavaScript');

INSERT INTO course_trainer (course_id, trainer_id) VALUES
(1, 1), 
(2, 2),  
(3, 3);  

INSERT INTO enrollment (student_id, course_id) VALUES
(1, 1),  
(2, 2),  
(3, 3),  
(1, 2);  

INSERT INTO certificates (enrollment_id) VALUES
(1),  
(2),  
(3);  

-- Phase-3 Sql Joins Practice

--1) List students and the courses they enrolled in

select s.student_id,s.full_name,c.course_id,c.course_name from student s
	join enrollment e
	on s.student_id = e.student_id
	join courses c
	on e.course_id = c.course_id
	order by s.student_id,c.course_id;

--2) Show students who received certificates with trainer names

select certificate_id,serial_no,c.enrollment_id,s.student_id,full_name,co.course_id,course_name,t.trainer_id,trainer_name
from certificates c join enrollment e
	on c.enrollment_id = e.enrollment_id
	join student s 
	on e.student_id = s.student_id
	join courses co
	on e.course_id = co.course_id
	join course_trainer ct
	on co.course_id = ct.course_id
	join trainers t
	on ct.trainer_id = t.trainer_id;

--3) Count no of students per course

select c.course_id, course_name, count(*) as no_of_students from courses c
	left join enrollment e
	on c.course_id = e.course_id
	group by c.course_id,course_name
	order by course_id;

-- Phase 4: Functions & Stores Procedures

-- Function

/* 
Create `get_certified_students(course_id INT)`
→ Returns a list of students who completed the given course and received certificates.
*/

drop function get_certified_students;

create or replace function get_certified_students(p_course_id int)
returns table (student_id int, student_name varchar(100))
as $$
begin
	return query
	select s.student_id, s.full_name from student s
	join enrollment e
	on s.student_id = e.student_id
	join certificates c
	on e.enrollment_id = c.enrollment_id
	where e.course_id = p_course_id;
	
end;
$$ language plpgsql;

select * from get_certified_students(1);

-- Stored Procedure

select * from student;
select * from courses;
select * from enrollment;
select * from certificates;

create or replace procedure sp_enroll_students(p_student_id int, p_course_id int, p_flag text)
Language plpgsql
as $$
declare 
	v_enrollment_id int;

begin

	Insert into enrollment(student_id, course_id,enroll_date)
	values (p_student_id,p_course_id, '2025-04-10 11:07:28.520637')
	returning enrollment_id into v_enrollment_id;

	IF LOWER(TRIM(p_flag)) = 'completed' THEN

		Insert into certificates (enrollment_id)
		values (v_enrollment_id);

	end if;

	raise notice 'Student % enrolled in course % with status %',p_student_id,p_course_id,p_flag;
end;
$$;

call sp_enroll_students(5,5,'Completed');

-- Phase-5

-- Cursor

/* Use a cursor to:

* Loop through all students in a course
* Print name and email of those who do not yet have certificates

*/

create or replace procedure sp_students_unverified(p_courseid int)
language plpgsql
as $$
declare
	rec record;

	cur_unverified cursor for
		select s.full_name,s.email from student s
			join enrollment e 
			on s.student_id = e.student_id
			left join certificates c
			on e.enrollment_id = c.enrollment_id
			where c.enrollment_id is null 
			and e.course_id = p_courseid;
begin
	open cur_unverified;

	loop
		fetch cur_unverified into rec;
		exit when not found;

		raise notice 'Name : %, Email : %, of the unverified student',rec.full_name,rec.email;
	end loop;
	close cur_unverified;
	
end;
$$;

call sp_students_unverified(2);

-- Phase 6: Security & Roles

/* 1. Create a `readonly_user` role:

   * Can run `SELECT` on `students`, `courses`, and `certificates`
   * Cannot `INSERT`, `UPDATE`, or `DELETE`
*/

create role readonly_user login password '123456';

grant connect on database "dvdRental" to readonly_user;

grant select on table student,courses, certificates to readonly_user;

/* 
postgres=> \c dvdRental
You are now connected to database "dvdRental" as user "readonly_user".
dvdRental=> select * from student;
 student_id |   full_name   |        email         |   phone
------------+---------------+----------------------+------------
          1 | Alice Johnson | alice@example.com    | 1234567890
          2 | Bob Smith     | bob@example.com      | 2345678901
          3 | Charlie Brown | charlie@example.com  | 3456789012
          4 | David Miller  | david.m@example.com  | 4567890123
          5 | Evelyn Wright | evelyn.w@example.com | 5678901234
          6 | Frank Harris  | frank.h@example.com  | 6789012345
          7 | Grace Lee     | grace.l@example.com  | 7890123456
          8 | Hannah Walker | hannah.w@example.com | 8901234567
(8 rows)

dvdRental=> insert into student(full_name,email,phone)values('muthu','muthu@gmail.com','2342323345');
ERROR:  permission denied for table student

*/

/*
	2. Create a `data_entry_user` role:

   * Can `INSERT` into `students`, `enrollments`
   * Cannot modify certificates directly

*/ 

create role data_entry_user login password '123456';

grant connect on database "dvdRental" to data_entry_user;

grant insert on table student,enrollment to data_entry_user;

grant usage,select on sequence enrollment_enrollment_id_seq to data_entry_user;

grant usage,select on sequence student_student_id_seq to data_entry_user;

/*
postgres=> \c dvdRental
You are now connected to database "dvdRental" as user "data_entry_user".
dvdRental=> update certificates set enrollment_id = 6 where certificate_id = 1;
ERROR:  permission denied for table certificates
dvdRental=> insert into student (full_name,email,phone) values('muthu','muthu@gmail.com','8291919321');
ERROR:  permission denied for sequence student_student_id_seq
dvdRental=> insert into enrollment(student_id,course_id) values(6,6);
ERROR:  permission denied for sequence enrollment_enrollment_id_seq
dvdRental=> insert into student (full_name,email,phone) values('muthu','muthu@gmail.com','8291919321');
INSERT 0 1
dvdRental=> insert into enrollment(student_id,course_id) values(6,6);
INSERT 0 1

*/

-- Phase-7 : Transactions & Atomicity

create or replace procedure sp_enrol_details(p_student_id int,p_course_id int)
language plpgsql
as $$
declare
	v_enrollment_id int;
begin

	begin
		Insert into enrollment(student_id, course_id,enroll_date)
		values (p_student_id,p_course_id, current_timestamp)
		returning enrollment_id into v_enrollment_id;

		Insert into certificates (enrollment_id)
		values (15);

		raise notice 'Student % enrolled in course %',p_student_id,p_course_id;
	exception when others then
		raise notice 'Error occured : %',sqlerrm;
	end;
end;
$$;

call sp_enrol_details(7,7);

select * from enrollment;
select * from certificates;