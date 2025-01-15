using AdminBO.Models;

public class ViewSpaceWithActivities
{
    public Space space { get; set; }
    public List<string> SpaceActivities { get; set; } = new List<string>();
}
