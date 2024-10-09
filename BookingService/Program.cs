using BookingService;
using BookingService.Consumers;
using BookingService.Repositories;
using BrokerMessage;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ReservationRepository>();

builder.Services.AddMassTransit(p =>
{
    p.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
    p.AddRider(rider =>
    {
        const string kafkaBrokerServer = "localhost:9092";
        rider.AddConsumer<ReservationConsumer>();
        
        rider.UsingKafka((context, k) =>
        {
            k.Host(kafkaBrokerServer);
            k.TopicEndpoint<ReservationMessage>("reservation-topic", "reservation-consumer-group", e =>
            {
                e.ConfigureConsumer<ReservationConsumer>(context);
            });
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();