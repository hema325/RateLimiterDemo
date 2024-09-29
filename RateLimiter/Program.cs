using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter(opt =>
{
    opt.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    opt.AddFixedWindowLimiter("FixedWindowLimiter", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 2;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.AutoReplenishment = false;
    });

    opt.AddSlidingWindowLimiter("SlidingWindowLimiter", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.AutoReplenishment = false;
        opt.PermitLimit = 2;
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.SegmentsPerWindow = 2; // 1 acts as fixed window limiter 
    });
    
    opt.AddTokenBucketLimiter("TokenBucketLimiter", opt =>
    {
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        opt.AutoReplenishment = true;
        opt.TokenLimit = 2;
        opt.TokensPerPeriod = 1;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    opt.AddConcurrencyLimiter("ConcurrencyLimiter", opt =>
    {
        opt.PermitLimit = 1;
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    opt.AddPolicy("IPAddressLimiter", httpContext =>
    {
        return RateLimitPartition.GetFixedWindowLimiter(httpContext.Connection.RemoteIpAddress, partition =>
        {
            return new FixedWindowRateLimiterOptions
            {
                Window = TimeSpan.FromSeconds(10),
                PermitLimit = 2,
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                AutoReplenishment = false
            };
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

app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
