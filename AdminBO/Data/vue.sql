CREATE VIEW
    View_Reservation AS
SELECT
    r.Id,
    r.EmployeeId,
    e.Name AS EmployeeName,
    s.OwnerId as OwnerId,
    (
        select
            Name
        from
            users
        where
            Id = s.ownerId
    ) as OwnerName,
    r.SpaceId,
    s.Name AS SpaceName,
    r.ReservationDate,
    r.StartDate,
    r.EndDate,
    r.Status
FROM
    Reservations r
    JOIN Users e ON r.EmployeeId = e.Id
    JOIN Spaces s ON r.SpaceId = s.Id;


create view
    View_Count_Activities as
Select
    sa.Id,
    sa.Name,
    count(sal.SpaceId) as compt
from
    SpaceActivities sa
    left join SpaceActivityLinks sal on sa.Id = sal.SpaceActivityId
group by
    sa.Id,
    sa.Name;