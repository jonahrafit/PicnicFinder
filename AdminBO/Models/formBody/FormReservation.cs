using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class FormReservation
{
    public int SpaceId { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime ReservationDate { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime StartDate { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime EndDate { get; set; }

    public override string ToString()
    {
        return $"FormReservation [SpaceId={SpaceId}, ReservationDate={ReservationDate}, StartDate={StartDate}, EndDate={EndDate}]";
    }
}
