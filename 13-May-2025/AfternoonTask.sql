-- Cursors
/* 1) Write a cursor to list all customers and how many rentals each made. Insert these into a summary table.*/

create table rental_summary(
	id serial primary key,
	customer_id int,
	rental_count int,
	last_updated timestamp default current_timestamp
);

DO $$
DECLARE
    rental_Count INT;
    rec RECORD;
    cur CURSOR FOR SELECT * FROM customer;
BEGIN
    OPEN cur;

    LOOP
        FETCH cur INTO rec;
        EXIT WHEN NOT FOUND;

        SELECT COUNT(*) INTO rental_Count
        FROM rental
        WHERE rental.customer_id = rec.customer_id;

        INSERT INTO rental_summary (customer_id, rental_count)
        VALUES (rec.customer_id, rental_Count);
    END LOOP;

    CLOSE cur;
END $$;

select * from rental_summary;


/* 2) Using a cursor, print the titles of films in the 'Comedy' category rented more than 10 times. */


DO $$

DECLARE
	rec record;
	count_rental int;

	film_cursor cursor for
		select f.film_id,f.title from film f
			join film_category fc
			on f.film_id = fc.film_id
			join category c
			on fc.category_id = c.category_id
			where c.name = 'Comedy';
BEGIN
	OPEN film_cursor;

	LOOP
		fetch film_cursor into rec;
		exit when not found;
		
		select count(*) into count_rental from rental r
		join inventory i
		on r.inventory_id = i.inventory_id
		where i.film_id = rec.film_id;

		if count_rental > 10 then
			RAISE NOTICE 'Title: %, Rentals: %', rec.title, count_rental;
		end if;
	END LOOP;

	CLOSE film_cursor;
END $$;

/* 3) Create a cursor to go through each store and count the number of distinct films available,
and insert results into a report table. */


create table store_report(
	id serial primary key,
	store_id int,
	film_count int,
	last_updated timestamp default current_timestamp
);

do $$

declare
	rec record;
    filmcount int;
	cur cursor for
		select store_id from store;

begin
	open cur;

	loop
		fetch cur into rec;
		exit when not found;
		select count(distinct i.film_id) into filmcount from inventory i
			where i.store_id = rec.store_id;

		insert into store_report(store_id,film_count)
		values(rec.store_id, filmcount);
	end loop;

	close cur;
end $$;

select * from store_report;

/* 4) Loop through all customers who haven't rented in the last 6 months and insert their details into an inactive_customers table. */

create table inactivecustomers(
	id serial primary key,
	customer_id int,
	last_rental_date date
);

do $$

declare
	rec record;
	last_rental date;
	cur cursor for 
		select customer_id from customer;

begin
	open cur;

	loop
		fetch cur into rec;
		exit when not found;
		select max(r.rental_date::date) into last_rental
		from rental r
		where r.customer_id = rec.customer_id;

		if last_rental is null or last_rental < current_Date - Interval '6 months' then
			insert into inactivecustomers(customer_id,last_rental_date)
			values (rec.customer_id,last_rental);
		end if;
	end loop;

	close cur;
end $$;

select * from inactivecustomers;


-- Transactions

/* 1) Write a transaction that inserts a new customer, adds their rental, and logs the payment – all atomically. */

do $$

declare
	cust_id int;
	rent_id int;
	
begin

	INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date)
	VALUES (
    1,  
    'Vijay', 'D', 'vijay.d@example.com',
    (SELECT address_id FROM address LIMIT 1),  
    1, CURRENT_TIMESTAMP
	)
	returning customer_id into cust_id;

	INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
    VALUES (CURRENT_TIMESTAMP, 1, cust_id, 1)
    RETURNING rental_id INTO rent_id;

	INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
    VALUES (cust_id, 1, rent_id, 4.99, CURRENT_TIMESTAMP);

End $$;

/* 2) simulate a transaction where one update fails (e.g., invalid rental ID), and ensure the entire transaction rolls back. */
do $$

begin

	update rental 
	set return_date=current_timestamp
	where rental_id =1;
	
	update rental
	set return_datee=current_timestamp
	where rental_id = -9;

	raise notice 'Both Updates Succeeded';

EXCEPTION WHEN OTHERS THEN
	RAISE NOTICE 'Error occurred: %, rolling back transaction.', SQLERRM;


END $$;


/* 3) Use SAVEPOINT to update multiple payment amounts. Roll back only one payment update using ROLLBACK TO SAVEPOINT. */


begin;

	UPDATE payment
    SET amount = 100
    WHERE payment_id = 17504;

    savepoint amount_paid;

	UPDATE payment
    SET amount = 200
    WHERE payment_id = 17504;

	UPDATE payment
    SET amount = 300
    WHERE payment_id = 17504;

	rollback to amount_paid;
	
commit;

select * from payment where payment_id =17504;

/* 4) Perform a transaction that transfers inventory from one store to another (delete + insert) safely.  */

start transaction;
 
	alter table  rental
	drop constraint rental_inventory_id_fkey;
	 
	delete from inventory where inventory_id = 100 ;
	 
	insert into inventory values
	(100,20,2,current_timestamp);
	 
	alter table rental
	ADD constraint rental_inventory_id_fkey
	foreign key (inventory_id)
	references inventory(inventory_id);
 
commit;

abort;
SELECT 
    trigger_name,
    event_manipulation AS event,
    action_timing AS trigger_timing,
    action_statement AS trigger_function,
    event_object_table AS table_name
FROM 
    information_schema.triggers
WHERE 
    event_object_table = 'inventory';

/* 5)  Create a transaction that deletes a customer and all associated records (rental, payment), ensuring referential integrity.*/


DO $$
DECLARE
    cusid INT := 588;
BEGIN
  
    DELETE FROM payment
    WHERE customer_id = cusid;

      
	DELETE FROM rental
    WHERE customer_id = cusid;

	    DELETE FROM customer
    WHERE customer_id = cusid;


    RAISE NOTICE 'Customer and related data deleted successfully.';
    
EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error occurred: %', SQLERRM;
	
END $$;



--Trigger

/* 1) Create a trigger to prevent inserting payments of zero or negative amount. */


create or replace function fn_check_payment()
returns trigger as $$
begin
	if new.amount = 0 then
		raise exception 'Payment cannot be zero';
	end if;
	return new;
end;
$$ language plpgsql;

create trigger trg_check_payment
before insert on payment
for each row
execute function fn_check_payment();

INSERT INTO payment (customer_id, staff_id, rental_id, amount, payment_date)
VALUES (1, 2, 15000, 0, CURRENT_TIMESTAMP);

/* 2) Set up a trigger that automatically updates last_update on the film table when the title or rental rate is changed. */

create or replace function set_last_update()
returns trigger as $$
begin
	if new.title is distinct from old.title or new.rental_rate is distinct from old.rental_rate then
		new.last_update = current_timestamp;
	end if;
	return new;
end;
$$ language plpgsql

create trigger trg_last_update
before update on film
for each row
execute function set_last_update();

UPDATE film
SET rental_rate = rental_rate + 0.5
WHERE film_id = 1;

select * from film where film_id = 1;

/* 3) Write a trigger that inserts a log into rental_log whenever a film is rented more than 3 times in a week. */

select * from inventory;

CREATE TABLE rental_log (
    id SERIAL PRIMARY KEY,
    film_id INT,
    rental_count INT,
    log_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

create or replace function rental_log()
returns trigger
AS $$
declare 
	filmid int;
	rentalCount int;
	current_week DATE;
begin
	select film_id into filmid from inventory
		where inventory_id = new.inventory_id;

	current_week := date_trunc('week',new.rental_date)::date;
		
	select count(*) into rentalCount
	from rental 
	where inventory_id = new.inventory_id and date_trunc('week',rental_date)::date = current_week;

	if rentalCount >= 3 then
		insert into rental_log (film_id,rental_count)
		values (filmid,rentalCount);
	end if;
	return new;
	
end;
$$ language plpgsql;

drop trigger trg_check_rental_counts_week on rental

create trigger trg_check_rental_counts_week
after insert on rental
for each row 
execute function rental_log();

select * from rental;


INSERT INTO rental (rental_date, inventory_id, customer_id, staff_id)
VALUES 
    (current_timestamp, 2, 1, 1), 
    (current_timestamp, 2, 2, 1), 
    (current_timestamp, 2, 3, 1); 


select * from rental_log;