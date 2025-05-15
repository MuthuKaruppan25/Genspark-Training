
CREATE EXTENSION IF NOT EXISTS pgcrypto;


/*1) 1. Create a stored procedure to encrypt a given text
Task: Write a stored procedure sp_encrypt_text that takes a plain text input (e.g., email or mobile number) and returns an encrypted version using PostgreSQL's pgcrypto extension. */ 



CREATE OR REPLACE PROCEDURE sp_encrypt_email(
    email TEXT,
    OUT encrypted_text bytea
)
AS $$
DECLARE
    secret_key TEXT := '##@!ofoib2313$#ASDW@e333'; 
BEGIN
    encrypted_text := pgp_sym_encrypt(email, secret_key);
END;
$$ LANGUAGE plpgsql;

DO $$
DECLARE
    result bytea;
BEGIN
    CALL sp_encrypt_email('muthu@gmail.com', result);
    RAISE NOTICE 'Encrypted Text: %', result;
END;
$$;


/*2) Create a stored procedure to compare two encrypted texts
Task: Write a procedure sp_compare_encrypted that takes two encrypted values and checks if they decrypt to the same plain text. */


create or replace procedure sp_compare_encrypt(encrypt1 bytea, encrypt2 bytea, out res boolean)
as $$
declare 
	secret_key TEXT := '##@!ofoib2313$#ASDW@e333'; 
	decrypt1 text;
	decrypt2 text;
begin
	decrypt1 := pgp_sym_decrypt(encrypt1::bytea,secret_key);
	decrypt2 := pgp_sym_decrypt(encrypt2::bytea,secret_key);

	if decrypt1 = decrypt2 then
		res := true;
	else
		res := false;
	end if;
end;
$$ language plpgsql;

do $$
declare
	enc1 bytea;
    enc2 bytea; 
    result BOOLEAN;
begin
	call sp_encrypt_email('muthu@gmail.com',enc1);
	call sp_encrypt_email('muthu@gmail.com',enc2);
	call sp_compare_encrypt(enc1,enc2,result);

	if result = true then
		Raise Notice 'Both are Equal';
	else
		Raise Notice 'Both are not Equal';
	end if;
end;
$$ ;

/* 3) Create a stored procedure to partially mask a given text
Task: Write a procedure sp_mask_text that:
 
Shows only the first 2 and last 2 characters of the input string
 
Masks the rest with *
 
E.g., input: 'john.doe@example.com' → output: 'jo***************om' */

drop procedure sp_mask_text;

create or replace procedure sp_mask_text(name text, out masked_name text)
as $$
declare
	len int;
	mask_len int;
begin
	len := Length(name);

	if len <= 4 then
		masked_name := name;
	else
		mask_len = len - 4;
		masked_name= substring(name from 1 for 2) || repeat('*',mask_len)
						|| substring(name from len-1 for 2);
	end if;
end;
$$ language plpgsql;

do $$
declare
	masked_Text text;
begin

	call sp_mask_text('muthukaruppan',masked_Text);

	Raise notice 'Masked Text: %',masked_Text;

end;
$$;

/*4) Create a procedure to insert into customer with encrypted email and masked name
Task:
 
Call sp_encrypt_text for email
 
Call sp_mask_text for first_name
 
Insert masked and encrypted values into the customer table */

create table sample_customer(
	id serial primary key,
	name text,
	email text
);


create or replace procedure sp_insert_customer(name text,email text)
as $$
declare
	masked_name text;
	encrypted_email bytea;
begin
	call sp_mask_text(name,masked_name);
	call sp_encrypt_email(email,encrypted_email);

	insert into sample_customer (name,email)
	values (masked_name,encrypted_email);

	
end;
$$ language plpgsql;


CALL sp_insert_customer('MuthuKaruppan', 'muthukaruppan@gmail.com');
CALL sp_insert_customer('AnbuSelvan', 'anbuselvan1990@gmail.com');
CALL sp_insert_customer('Divya Shree', 'divya.shree21@gmail.com');
CALL sp_insert_customer('Karthik Raja', 'karthik.raja007@gmail.com');
CALL sp_insert_customer('Revathi Bala', 'revathi.bala23@gmail.com');

select * from sample_customer;


/* 5) Create a procedure to fetch and display masked first_name and decrypted email for all customers
Task:
Write sp_read_customer_masked() that:
 
Loops through all rows
 
Decrypts email
 
Displays customer_id, masked first name, and decrypted email */
 
create or replace procedure sp_read_customer_masked()
as $$
declare
	secret_key TEXT := '##@!ofoib2313$#ASDW@e333';
	decrypted_email text;
	rec record;
	cur cursor for
		select * from sample_customer;
begin
	open cur;

	loop
		fetch cur into rec;
		exit when not found;
		decrypted_email := pgp_sym_decrypt(rec.email::bytea,secret_key);

		raise notice 'Customer Id: %, Masked Name : %, Decrypted Email: %',rec.id, rec.name,decrypted_email;
	end loop;
	close cur;
end;
$$ language plpgsql;

call sp_read_customer_masked();
