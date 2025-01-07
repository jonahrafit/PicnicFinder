using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontOffice.Models
{
    public class SpaceViewModel
    {
        public long Id { get; set; }
        public long OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Capacity { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, "
                + $"OwnerId: {OwnerId}, "
                + $"Name: {Name}, "
                + $"Latitude: {Latitude}, "
                + $"Longitude: {Longitude}, "
                + $"Capacity: {Capacity}, ";
        }
    }
}
