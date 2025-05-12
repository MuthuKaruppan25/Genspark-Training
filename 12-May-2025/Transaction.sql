/*1) In a transaction, if I perform multiple updates and an error happens in the third statement, but I have not used SAVEPOINT, what will happen if I issue a ROLLBACK?
Will my first two updates persist?*/

BEGIN;

	UPDATE tbl_bank_accounts
	SET balance = balance - 500
	WHERE account_name = 'Alice';

	UPDATE tbl_bank_accounts
	SET balance = balance + 500
	WHERE account_name = 'Bob';

	UPDATE tbl_bank_account
	SET balance = balance + 500
	WHERE account_name = 'Bob';

ROLLBACK;

SELECT * FROM tbl_bank_accounts;

/*Ans :  No, The First two updates won't persist if i doesnt used the savepoint, rollback leads to the last 
saved state. */

/* 2) Suppose Transaction A updates Alice’s balance but does not commit. 
Can Transaction B read the new balance if the isolation level is set to READ COMMITTED?*/

--TRANSACTION A
BEGIN;

	UPDATE tbl_bank_accounts
	SET balance = balance + 500
	WHERE account_name = 'Alice';

--TRANSACTION B

BEGIN ISOLATION LEVEL READ COMMITTED;
	SELECT balance FROM tbl_bank_accounts WHERE account_name = 'Alice';
COMMIT;

--TRANSACTION A
COMMIT;

-- ANS:  No, Transaction B cannot read the new balance updated by Transaction A if the isolation level is READ COMMITTED.

/* 3) What will happen if two concurrent transactions both execute:
UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';
at the same time? Will one overwrite the other? */

--TRANSACTION A
BEGIN;
	UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';

--TRANSACTION B
BEGIN;
	UPDATE tbl_bank_accounts SET balance = balance - 100 WHERE account_name = 'Alice';
COMMIT;

--TRANSACTION A
COMMIT;

/* ANS: No, one will not overwrite the other.when two transaction tries to update the same row,
it enforces row locks to minimize the access, so the second transaction can update and commit the 
changes only after the first one commits the changes.*/

/* 4) It will only undo the changes made after the savepoint named after_alice, not the entire transaction.*/

BEGIN;
	
UPDATE tbl_bank_accounts
SET balance = balance - 100
WHERE account_name = 'Alice';

SAVEPOINT After_Alice;

UPDATE tbl_bank_accountS
SET balance = balance + 100
WHERE account_name = 'Bob';

ROLLBACK TO After_Alice;

COMMIT;

SELECT * FROM tbl_bank_accounts;

--ANS:  It will only undo the changes made after the savepoint named After_Alice, not the entire transaction.

/* 5) Which isolation level in PostgreSQL prevents phantom reads?*/

-- READ COMMITTED

--TRANSACTION 1

BEGIN;
	SELECT * FROM tbl_bank_accounts;



--TRANSACTION 2

BEGIN;
	
	INSERT INTO tbl_bank_accounts
	(account_name, balance)
	VALUES
	('WILLY', 7000.00);

COMMIT;

--TRANSACTION 1
COMMIT;

-- It allows the phantom reads because it reads the committed changes from other transactions

--TRANSACTION 1

BEGIN ISOLATION LEVEL REPEATABLE READ;

	
	SELECT * FROM tbl_bank_accounts;

	
--TRANSACTION 2

BEGIN ISOLATION LEVEL REPEATABLE READ;
	INSERT INTO tbl_bank_accounts
	(account_name, balance)
	VALUES
	('WILLY', 7000.00);

COMMIT;

--TRANSACTION 1
COMMIT;

/* It doesn't displays the newly added record, because it uses the initial snapshot which is the last
last saved state therefore it prevents the read phantom, but it allows the write phantom ,it
won't enforce restriction to phantoms completely because it tracks only the same rows involved in both trans*/

--TRANSACTION 1

BEGIN ISOLATION LEVEL SERIALIZABLE;

	
	SELECT * FROM tbl_bank_accounts;

	
--TRANSACTION 2

BEGIN ISOLATION LEVEL SERIALIZABLE;
	INSERT INTO tbl_bank_accounts
	(account_name, balance)
	VALUES
	('WILLY', 7000.00);

COMMIT;

--TRANSACTION 1
COMMIT;

/* Ans: Serializable is the one that completely prevents phantom because it blocks the concurrent access and even
tracks the part of the state involved in the transaction therefore it can prevent both the read
and write phantoms*/

/* 6) Can Postgres perform a dirty read (reading uncommitted data from another transaction)? */

/* Ans: No Postgres doesn't allows to perform dirty reads. A dirty read occurs when Transaction B reads data that was modified by Transaction A but not yet committed. 
If Transaction A rolls back, Transaction B has read invalid, therefore it leads to incosistent data.
postgres enforces  MVCC(Multi-Version Concurrency Control) which doesn't allows the read uncommitted level
by default, it enforces read committed by defalut other levels such as repeatable reads and serializable is
supported by postgres.Therefore PostgreSQL always protects you from dirty reads. */

/* 7) If autocommit is ON (default in Postgres), and I execute an UPDATE, is it safe to assume the change is immediately committed? */

/*ans: Autocommit is the mode that treats every single statement as individual transactions,until
it is explicility defined inside the begin and commit block, which is the explicit way of the creating
transaction.Therefore any statement that is not explicitly defined is a transaction and it will be
committed immediately after execution. */


-- 8)

--TRANSACTION 1
BEGIN;
UPDATE accounts SET balance = balance - 500 WHERE id = 1;

--TRANSACTION 2
SELECT balance FROM accounts WHERE id = 1;


/*Ans: No, The second transaction won't see the updated balances because by default postgres
enforces read committed level , so only the committed changes are visible in other transactions.
Here changes made in the transaction 1 is not committed , so these changes are not visible in
second transaction. */