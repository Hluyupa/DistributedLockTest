using BrokerMessage;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace SomeService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    private readonly ITopicProducer<ReservationMessage> _topicProducer;

    public ReservationController(ITopicProducer<ReservationMessage> topicProducer)
    {
        _topicProducer = topicProducer;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateReservation(Guid placeId)
    {
        await _topicProducer.Produce(new
        {
            PlaceId = placeId
        });
        return Ok();
    } 
}