
--1) Write a cursor that loops through all films and prints titles longer than 120 minutes.

do $$
	declare my_cursor refcursor;
	declare films record;
	begin
		open my_cursor for
			select * from film where length > 120;

		loop
			fetch my_cursor into films;
			exit when not found;
			raise notice 'Movie Id : % , Movie Name : %, Duration : %',films.film_id,films.title,films.length;
		end loop;

		close my_cursor;
	end
$$ language plpgsql;

rollback
begin;
 
declare cursor_films cursor
for
select * from film where length >120;
 
fetch all from cursor_films;
 
close cursor_films;
end;

--2) Create a cursor that iterates through all customers and counts how many rentals each made.

do $$
	declare my_cursor refcursor;
	declare customers record;
	begin
		open my_cursor for
			select c.customer_id, concat(first_name,' ',last_name)as CustomerName, count(*) as RentalCount
			from customer c join rental r
			on c.customer_id = r.customer_id
			group by c.customer_id,first_name,last_name;

		loop
			fetch my_cursor into customers;
			exit when not found;
			raise notice 'Customer Id : % ,Customer Name : %, Rental Count : %',customers.customer_id,customers.CustomerName,customers.RentalCount;
		end loop;

		close my_cursor;
	end
$$ language plpgsql;

--3) Using a cursor, update rental rates: Increase rental rate by $1 for films with less than 5 rentals.


DO $$
DECLARE 
    my_cursor REFCURSOR;
    films RECORD;
BEGIN
    OPEN my_cursor FOR
        SELECT 
            f.film_id AS film_id, 
            f.title AS film_name, 
            COUNT(r.rental_id) AS film_count
        FROM film f
        JOIN inventory i ON f.film_id = i.film_id
        LEFT JOIN rental r ON i.inventory_id = r.inventory_id
        GROUP BY f.film_id, f.title;

    LOOP
        FETCH my_cursor INTO films;
        EXIT WHEN NOT FOUND;

        IF films.film_count < 5 THEN
            UPDATE film 
            SET rental_rate = rental_rate + 1
            WHERE film_id = films.film_id;

            RAISE NOTICE 'Updated Successfully: %', films.film_name;
        END IF;
    END LOOP;

    CLOSE my_cursor;
END
$$ LANGUAGE plpgsql;

--4) Create a function using a cursor that collects titles of all films from a particular category.

CREATE FUNCTION getfilmtitles(cid INT)
RETURNS TABLE(MovieName VARCHAR(100)) AS $$
DECLARE
    filmrec RECORD;
    my_cursor REFCURSOR;
BEGIN
    OPEN my_cursor FOR
        SELECT f.title FROM film f
        JOIN film_category fc ON f.film_id = fc.film_id
        WHERE category_id = cid;

    LOOP
        FETCH NEXT FROM my_cursor INTO filmrec;
        EXIT WHEN NOT FOUND;

        MovieName := filmrec.title; 
        RETURN NEXT;
    END LOOP;

    CLOSE my_cursor;
END;
$$ LANGUAGE plpgsql;

select getfilmtitles(1)
	
--5) Loop through all stores and count how many distinct films are available in each store using a cursor.

DO $$
DECLARE 
    my_cursor REFCURSOR;
    stores RECORD;
	filmcount int;
BEGIN
    OPEN my_cursor FOR
        	SELECT * from store;
    LOOP
        FETCH my_cursor INTO stores;
        EXIT WHEN NOT FOUND;

        SELECT COUNT(DISTINCT i.film_id)
        INTO filmcount
        FROM inventory i
        WHERE i.store_id = stores.store_id;

        RAISE NOTICE 'store id: % , FilmCount: %', stores.store_id,filmcount;
        
    END LOOP;

    CLOSE my_cursor;
END
$$ LANGUAGE plpgsql;


--Triggers

--6) Write a trigger that logs whenever a new customer is inserted.

create table customer_log(
	log_id serial primary key,
	customer_id int,
	first_name text,
	last_name text,
	created_At timestamp default current_timestamp
);

create or replace function log_customer()
returns trigger as $$
begin 
	insert into customer_log(customer_id,first_name,last_name)
	values (NEW.customer_id,NEW.first_name,NEW.last_name);

	return new;
end;
$$ language plpgsql;

create trigger trg_log_customer
after insert on customer
for each row
execute function log_customer();

INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date)
VALUES (1, 'John', 'Doe', 'john.doe@example.com', 5, 1, CURRENT_TIMESTAMP);

select * from customer_log;

--7) Create a trigger that prevents inserting a payment of amount 0.

create function fn_check_payment()
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

--8) Set up a trigger to automatically set last_update on the film table before update.

create or replace function set_last_update()
returns trigger as $$
begin
	new.last_update = current_timestamp;
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

--9) Create a trigger to log changes in the inventory table (insert/delete).

create table inventory_log(
	logid serial primary key,
	operation text,
	inventory_id int,
	film_id int,
	store_id int,
	changed_At timestamp default current_timestamp
);

create or replace function log_changes()
returns trigger as $$
begin
	if TG_OP = 'INSERT' then
		insert into inventory_log(operation,inventory_id,film_id,store_id)
		values ('Insert',new.inventory_id,new.film_id,new.store_id);
	elsif TG_OP = 'DELETE' then
		insert into inventory_log(operation,inventory_id,film_id,store_id)
		values ('Delete',oldventory_id,old.film_id,old.store_id);
	end if;
	return null;
end;
$$ language plpgsql;

create trigger tg_log_inventory
after insert or delete on inventory
for each row
execute function log_changes();

INSERT INTO inventory (film_id, store_id)
VALUES (1, 1);  

delete from inventory where inventory_id = (select max(inventory_id ) from inventory);

select * from inventory_log;

--10) Write a trigger that ensures a rental can’t be made for a customer who owes more than $50.

select * from film

CREATE OR REPLACE FUNCTION block_rental()
RETURNS TRIGGER AS $$
DECLARE 
    rentalAmount NUMERIC := 0;
    paidAmount NUMERIC := 0;
    owed NUMERIC;
BEGIN
 
    SELECT COALESCE(SUM(f.rental_rate), 0) INTO rentalAmount
    FROM rental r
    JOIN inventory i ON r.inventory_id = i.inventory_id
    JOIN film f ON i.film_id = f.film_id
    WHERE r.customer_id = NEW.customer_id;


    SELECT COALESCE(SUM(amount), 0) INTO paidAmount
    FROM payment 
    WHERE customer_id = NEW.customer_id;

    
    owed := rentalAmount - paidAmount;


    IF owed > 50 THEN
		raise notice '%',new.customer_id;
        RAISE EXCEPTION 'Rental Denied: Customer % owes $%', NEW.customer_id, owed;
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;


create trigger trg_block_rent
before insert on rental
for each row
execute function block_rental();

-- Transaction

--11) Write a transaction that inserts a customer and an initial rental in one atomic operation.


CREATE OR REPLACE PROCEDURE insert_customer_and_rental()
LANGUAGE plpgsql
AS $$
DECLARE
    new_customer_id INT;
BEGIN
    BEGIN
        
        INSERT INTO customer (store_id, first_name, last_name, email, address_id, active, create_date)    
        VALUES (1, 'singam', 'Singh', 'rockybhai@example.com', 5, 1, CURRENT_TIMESTAMP)
        RETURNING customer_id INTO new_customer_id;

      
        INSERT INTO rental (
            rental_date, inventory_id, customer_id, staff_id
        )
        VALUES (
            CURRENT_TIMESTAMP, 5000, new_customer_id, 2
        );

    EXCEPTION WHEN OTHERS THEN
        RAISE NOTICE 'Transaction failed. Rolling back.';
        RAISE;
    END;
END $$;

call insert_customer_and_rental();


--12) Simulate a failure in a multi-step transaction (update film + insert into inventory) and roll back.

DO $$
DECLARE
    filmid INT;
BEGIN

    UPDATE film
    SET rental_rate = 8.99
    WHERE film_id = 8
    RETURNING film_id INTO filmid;


    INSERT INTO inventory (film_id, store_id, last_update)
    VALUES (filmid, 9999999, CURRENT_TIMESTAMP);



EXCEPTION WHEN OTHERS THEN

    RAISE NOTICE 'Error occurred: %', SQLERRM;
   
END $$;

--13) Create a transaction that transfers an inventory item from one store to another.


DO $$

DECLARE 
    source_store_id INT := 1;
    dest_store_id INT := 2;
    filmid INT := 8;

BEGIN

	update inventory
	set store_id = dest_store_id
	where store_id = source_store_id and film_id = filmid;
    
   
    RAISE NOTICE 'Film transferred from store % to store %', source_store_id, dest_store_id;

EXCEPTION WHEN OTHERS THEN
 
    RAISE NOTICE 'Error occurred: %', SQLERRM;
    
END $$;

--14) Demonstrate SAVEPOINT and ROLLBACK TO SAVEPOINT by updating payment amounts, then undoing one.

select * from payment where payment_id = 17503

begin;
	update payment set amount = 10.00 
	where payment_id = 17503;

	savepoint before_update;

	update payment set amount = 15.00 
	where payment_id = 17503;

	rollback to before_update;
commit;

--15) Write a transaction that deletes a customer and all associated rentals and payments, ensuring atomicity.

select max(customer_id) from customer

DO $$
DECLARE
    cusid INT := 599;
BEGIN
    -- Delete payments
    DELETE FROM payment
    WHERE customer_id = cusid;

       -- Delete customer
    DELETE FROM customer
    WHERE customer_id = cusid;

	DELETE FROM rental
    WHERE customer_id = cusid;


    RAISE NOTICE 'Customer and related data deleted successfully.';
    
EXCEPTION WHEN OTHERS THEN
    RAISE NOTICE 'Error occurred: %', SQLERRM;
	
END $$;


