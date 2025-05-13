
--Row Level Locking / Implicit Locking

/* 1) Try two concurrent updates to same row → see lock in action. */

--Transaction 1
BEGIN;
	UPDATE tbl_bank_accounts
	SET balance = 1000
	WHERE account_name = 'Alice';


--Transaction 2
BEGIN;
	UPDATE tbl_bank_accounts
	SET balance = 1500
	WHERE account_name = 'Alice';
COMMIT;


--Transaction 1
commit;

SELECT * FROM tbl_bank_accounts WHERE account_name = 'Alice';

/*Ans: Transaction 2 waits for Transaction 1 to commit before proceeding with its update. 
This is because PostgreSQL uses row-level locking, which prevents other transactions from modifying the same row 
until the lock is released.*/

-- 2) Write a query using SELECT...FOR UPDATE and check how it locks row.

--TRANSACTION 1
BEGIN;
	SELECT * FROM tbl_bank_accounts WHERE account_name = 'Alice'
		FOR UPDATE;
		
--TRANSACTION 2
BEGIN;
	UPDATE tbl_bank_accounts
	SET balance = 1500
	WHERE account_name = 'Alice';
COMMIT;

--TRANSACTION 1
COMMIT;

/*Ans : Transaction 1 implements an row shared lock using for update for the selected row. so other
transaction can't make any changes until transaction 1 is committed.*/

--) 3) Intentionally create a deadlock and observe PostgreSQL cancel one transaction.

--Transaction 1
begin;
	UPDATE tbl_bank_accounts
	SET balance = 1500
	WHERE account_name = 'Alice';

--Transaction 2
	BEGIN;
	UPDATE tbl_bank_accounts
	SET balance = 1700
	WHERE account_name = 'Bob';

--Transaction 1
	UPDATE tbl_bank_accounts
	SET balance = 1500
	WHERE account_name = 'Bob';

----Transaction 2
	UPDATE tbl_bank_accounts
	SET balance = 2500
	WHERE account_name = 'Alice';

/* Ans:  Transaction 1 is waiting for Bob, locked by Transaction 2 and Transaction 2 is waiting for Alice, locked by Transaction 1.
So deadlock was detected and one of the transactions was cancelled to be resolved.
*/	

--4) Use pg_locks query to monitor active locks.
SELECT 
    pg_stat_activity.pid,
    pg_stat_activity.datname,
    pg_stat_activity.usename,
    pg_stat_activity.application_name,
    pg_stat_activity.client_addr,
    pg_stat_activity.backend_start,
    pg_stat_activity.query_start,
    pg_stat_activity.state,
    pg_stat_activity.query,
    pg_locks.locktype,
    pg_locks.mode,
    pg_locks.granted,
    pg_locks.relation::regclass AS locked_relation
FROM pg_locks
JOIN pg_stat_activity
    ON pg_locks.pid = pg_stat_activity.pid
WHERE pg_stat_activity.datname = current_database()
ORDER BY pg_stat_activity.query_start;


--5) 

-- Access Share Mode

--Tran 1
begin;
	LOCK TABLE tbl_bank_accounts
	IN ACCESS SHARE MODE;

	select * from tbl_bank_accounts;
--Tran 2
BEGIN;
	UPDATE tbl_bank_accounts
	SET balance = 1700
	WHERE account_name = 'Bob';

--Tran1
commit;

-- It Allows other transactions to read (SELECT) and modify data (INSERT, UPDATE, DELETE).

--Tran 1
begin;
	LOCK TABLE tbl_bank_accounts
	IN ACCESS SHARE MODE;

	select * from tbl_bank_accounts;
--Tran 2
BEGIN;
	alter table tbl_bank_accounts add column last_updated timestamp default current_timestamp;
--Tran1
commit;

-- It Blocks Only ACCESS EXCLUSIVE locks (like DROP TABLE, TRUNCATE, or some ALTER TABLE operations).
	
-- Row Share

--TRANSACTION 1
BEGIN;
	SELECT * FROM tbl_bank_accounts WHERE account_name = 'Alice'
		FOR UPDATE;
		
--TRANSACTION 2
BEGIN;
	UPDATE tbl_bank_accounts
	SET balance = 1500
	WHERE account_name = 'Alice';
COMMIT;

--TRANSACTION 1
COMMIT;

/*Ans : Transaction 1 implements an row shared lock using for update for the selected row. so other
transaction can't make any changes until transaction 1 is committed.*/

-- Exclusive lock
-- transaction A
begin;
-- transaction A acquires an exclusive lock
lock table tbl_bank_accounts in exclusive mode;

-- transaction B
begin;
update tbl_bank_accounts set balance = 50000 where account_id = 1;
-- transaction B tries to update a row
-- will block until transaction A commits


--Access exclusive lock

-- transaction A
begin;
lock table tbl_bank_accounts in access exclusive mode;
-- transaction A acquires an access exclusive lock
alter table tbl_bank_accounts add column last_updated timestamp;

-- transaction B
begin;
-- transaction B tries to select or update the table
-- will block until transaction A commits
select * from tbl_bank_accounts where account_id = 1;
-- or
update tbl_bank_accounts set balance = 60000 where account_id = 1;


