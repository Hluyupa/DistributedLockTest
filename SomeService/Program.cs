using BrokerMessage;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddMassTransit(p =>
{
    p.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
    p.AddRider(rider =>
    {
        const string kafkaBrokerServer = "localhost:9092";
    
        rider.AddProducer<ReservationMessage>("reservation-topic");
        
        rider.UsingKafka((context, k) =>
        {
            k.Host(kafkaBrokerServer);
        });
    });
});

var app = builder.Build();

app.UseRouting();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();