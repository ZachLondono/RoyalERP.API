using MediatR;
using RoyalERP.Common;
using RoyalERP.Manufacturing;
using RoyalERP.Sales;
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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();