select * from cd.facilities;

select name,membercost from cd.facilities;

select * from cd.facilities where membercost > 0;

SELECT facid, name, membercost, monthlymaintenance
FROM cd.facilities
WHERE membercost > 0 and membercost < (1.0 / 50.0) * monthlymaintenance;

select * from cd.facilities where lower(name) like '%tennis%';

select * from cd.facilities where facid in (1,5);

select name,case when monthlymaintenance < 100 then 'cheap' else 'expensive' end as cost from 
cd.facilities;

SELECT memid, surname, firstname, joindate
FROM cd.members
WHERE EXTRACT(MONTH FROM joindate) >= 9 and EXTRACT(YEAR FROM joindate) >= 2012;

select distinct surname from cd.members order by surname limit 10;

select surname from cd.members union select name as surname from cd.facilities;

select max(joindate) as latest from cd.members;

select firstname,surname,joindate from cd.members order by joindate desc limit 1;

select starttime from cd.bookings b join cd.members m on b.memid = m.memid where firstname = 'David' and surname='Farrell';
or
select starttime from cd.bookings b join cd.members m on b.memid = m.memid where concat(m.firstname,' ',m.surname) like 'David Farrell';

select b.starttime as start,f.name from cd.bookings b join cd.facilities f on b.facid = f.facid
where lower(f.name) like '%tennis court%' and date(b.starttime) = date('2012-09-21') order by b.starttime;

select distinct f.firstname,f.surname from cd.members f join cd.members s on f.memid = s.recommendedby order by f.surname,f.firstname;

select f.firstname memfname, f.surname memsname, s.firstname recfname, s.surname recsname
from cd.members f left join cd.members s on f.recommendedby = s.memid order by f.surname,f.firstname;

select distinct concat(m.firstname,' ',m.surname)as member,name facility from cd.members m 
join cd.bookings b on m.memid = b.memid
join cd.facilities f on b.facid = f.facid
where lower(name) like '%tennis court%'
order by member,name;

SELECT 
  CONCAT(m.firstname, ' ', m.surname) AS member,
  f.name AS facility,
  CASE 
    WHEN m.memid = 0 THEN b.slots * f.guestcost
    ELSE b.slots * f.membercost
  END AS cost
FROM 
  cd.members m 
JOIN 
  cd.bookings b ON m.memid = b.memid
JOIN 
  cd.facilities f ON b.facid = f.facid
WHERE
  DATE(b.starttime) = '2012-09-14'
  AND (
    b.slots * CASE 
      WHEN m.memid = 0 THEN f.guestcost
      ELSE f.membercost
    END
  ) > 30
ORDER BY 
  cost DESC;


select distinct concat(firstname,' ',surname) member, 
(select concat(firstname,' ',surname) from cd.members m2 where mr.recommendedby = m2.memid) as recommender
from cd.members mr;





