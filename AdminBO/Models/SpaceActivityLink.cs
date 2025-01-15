using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdminBO.Models;

public class SpaceActivityLink
{
    public long SpaceId { get; set; }
    public long SpaceActivityId { get; set; }
    
    public Space Space { get; set; }
    public SpaceActivity SpaceActivity { get; set; }
}
