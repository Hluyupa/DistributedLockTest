namespace BookingService.Repositories;

public class ReservationRepository
{
    private readonly Context _context;

    public ReservationRepository(Context context)
    {
        _context = context;
    }
    
    public async Task<bool> TryReservePlaceAsync(Guid placeId)
    {
        var place = _context.Places.FirstOrDefault(p => p.PlaceId == placeId);
        if (place == null || place.IsReserved)
        {
            return false;
        }
        place.IsReserved = true;
        await _context.SaveChangesAsync();
        return true;
    }
}