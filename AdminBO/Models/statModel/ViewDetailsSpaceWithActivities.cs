using AdminBO.Models;

public class ViewDetailsSpaceWithActivities
{
    public Space Space { get; set; }
    public List<SpaceActivity> SpaceActivities { get; set; } = new List<SpaceActivity>();
}
