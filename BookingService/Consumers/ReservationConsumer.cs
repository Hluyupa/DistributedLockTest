using BookingService.Repositories;
using BrokerMessage;
using MassTransit;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace BookingService.Consumers;

public class ReservationConsumer : IConsumer<ReservationMessage>
{
    private readonly ReservationRepository _reservationRepository;
    private readonly IDistributedLockFactory _redlockFactory;
    public ReservationConsumer(ReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
        _redlockFactory = RedLockFactory.Create(new List<RedLockMultiplexer>()
        {
            ConnectionMultiplexer.Connect("localhost:63791"),
            ConnectionMultiplexer.Connect("localhost:63792"),
            ConnectionMultiplexer.Connect("localhost:63793")
        });
    }
    
    public async Task Consume(ConsumeContext<ReservationMessage> context)
    {
        var resourceKey = "place:" + context.Message.PlaceId;
        using (var redlock = await _redlockFactory.CreateLockAsync(resourceKey, TimeSpan.FromSeconds(30)))
        {
            if (redlock.IsAcquired)
            {
                // Блокировка успешно захвачена
                Console.WriteLine($"Lock acquired for resource: {resourceKey}");

                await _reservationRepository.TryReservePlaceAsync(context.Message.PlaceId);
            }
            else
            {
                // Блокировка не была захвачена
                Console.WriteLine($"Failed to acquire lock for resource: {resourceKey}");
            }
        }
    }
}