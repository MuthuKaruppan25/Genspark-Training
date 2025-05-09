--1) List all films with their length and rental rate, sorted by length descending.

select title,length as Duration, rental_rate as RentalAmount from film 
	order by Duration desc;

--2) Find the top 5 customers who have rented the most films.

select c.customer_id, concat(first_name,' ',last_name)as CustomerName, count(*) as RentalCount
	from customer c join rental r
	on c.customer_id = r.customer_id
	group by c.customer_id,first_name,last_name
	order by RentalCount desc limit 5



--3 Display all films that have never been rented.

select f.film_id as Filmid, title as FilmName from film f
	left join inventory i 
	on f.film_id = i.film_id
	left join rental r
	on i.inventory_id = r.inventory_id
	where rental_id is null 
	order by filmid

--4) List all actors who appeared in the film ‘Academy Dinosaur’.

select a.actor_id,concat(first_name,' ',last_name)as ActorName from actor a
	join film_actor fa
	on a.actor_id = fa.actor_id
	join film f
	on fa.film_id = f.film_id
	where title like '%Academy Dinosaur%'

--5) List each customer along with the total number of rentals they made and the total amount paid.

select c.customer_id, concat(first_name,' ',last_name)as CustomerName, count(*) as RentalCount, sum(amount) as TotalAmount
	from customer c join rental r
	on c.customer_id = r.customer_id
	join payment p
	on r.customer_id = p.customer_id
	group by c.customer_id,first_name,last_name
	order by totalamount desc

--6) Using a CTE, show the top 3 rented movies by number of rentals.

with RentedMovies as
(
	select f.film_id, title as MovieName,Count(*) as RentalCount from film f
	join inventory i 
	on f.film_id = i.film_id
	join rental r
	on i.inventory_id = r.inventory_id
	group by f.film_id,title
	order by RentalCount desc 
)
select * from RentedMovies limit 3

/*7) Find customers who have rented more than the average number of films.
Use a CTE to compute the average rentals per customer, then filter.*/

with AvgRentals as
(
	select c.customer_id, concat(first_name,' ',last_name)as CustomerName, Count(*) as TotalRentals
	from customer c join rental r
	on c.customer_id = r.customer_id
	group by c.customer_id,first_name,last_name
)
select * from AvgRentals where TotalRentals > (select avg(TotalRentals) from AvgRentals)

--8) Write a function that returns the total number of rentals for a given customer ID.

create function totalNo_Of_Rentals(cid int)
returns int as $$
declare totalRentals int := 0;
begin 
	select  Count(*) into totalRentals
	from rental r
	where customer_id = cid;

	return totalRentals;
end;
$$ language plpgsql;
select totalNo_Of_Rentals(87)

--9) Write a stored procedure that updates the rental rate of a film by film ID and new rate.

select * from film;

create procedure updateRent(f_id int,new_rate numeric)
as $$
begin
	update film set rental_rate = new_rate where film_id = f_id;
end
$$ language plpgsql

call updateRent(133,5.99)
select * from film where film_id =133

--10) Write a procedure to list overdue rentals (return date is NULL and rental date older than 7 days).


CREATE OR REPLACE PROCEDURE overdueDetails()
LANGUAGE plpgsql
AS $$
DECLARE
    my_cursor REFCURSOR;
	rental_rec RECORD;
BEGIN
    OPEN my_cursor FOR 
    SELECT rental_id, 
           CASE
               WHEN return_date IS NULL THEN CURRENT_DATE - rental_date::DATE
               ELSE return_date::DATE - rental_date::DATE
           END AS overdue_days
    FROM rental
    WHERE return_date IS NULL 
       OR (return_date::DATE - rental_date::DATE) > 7;
	LOOP
        FETCH my_cursor INTO rental_rec;  
        EXIT WHEN NOT FOUND;              
        RAISE NOTICE 'Rental ID: %, Overdue Days: %', rental_rec.rental_id, rental_rec.overdue_days;
    END LOOP;
    CLOSE my_cursor;
	
END;
$$;

CALL overdueDetails();
