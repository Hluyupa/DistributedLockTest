namespace BookingService.Models;

public class Place
{
    public Guid PlaceId { get; set; }

    public int PlaceNumber { get; set; }

    public bool IsReserved { get; set; } = false;
}