using Dapper;
using MediatR;
using RoyalERP.Common;
using RoyalERP.Common.Data;
using RoyalERP.Common.Domain;
using RoyalERP.API.Manufacturing;
using RoyalERP.API.Sales;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x => {
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddManufacturing();
builder.Services.AddSales();

SqlMapper.AddTypeHandler(new JsonTypeHandler<Dictionary<string,string>>());

var AllowAllOriginsPolicy = "_myAllowSpecificOrigins";
builder.Services.AddCors(options => {
    options.AddPolicy(name: AllowAllOriginsPolicy, 
                      policy => {
                          policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                      });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AllowAllOriginsPolicy);
app.UseDomainExceptionMiddleware();
app.MapControllers();

if (app.Environment.IsDevelopment()) {
    app.Run();
} else {
    app.Run("http://*:5000");
}