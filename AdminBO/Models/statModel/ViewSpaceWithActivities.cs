using AdminBO.Models;

public class ViewSpaceWithActivities
{
    public Space Space { get; set; }
    public List<string> SpaceActivities { get; set; } = new List<string>();
}
