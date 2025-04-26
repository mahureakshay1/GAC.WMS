using GAC.WMS.Application;
using GAC.WMS.Api.Middleware;
using Hangfire;
using GAC.WMS.Infrastructure.Job;
using GAC.WMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GAC.WMS.Infrastructure.ServiceExtension;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();
// Swagger configuration
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//Middleware configuration
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();

app.UseRateLimiter();
app.MapControllers();

// Add hangfire dashbord support to applicaiton 
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    // Authorization is not added here, becasue it is not part of assesment
    DisplayStorageConnectionString = false,
    IsReadOnlyFunc = context => false,
    DashboardTitle = "GAC Warehouse Management System - Jobs "
});

// added only to copy initial data to db for tetsing this should not add in production
if (!app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate(); // Applies pending migrations
    }
}
// scheduling a job
RecurringJob.AddOrUpdate<PurchaseOrderImporter>(
    "Purchase_Order_Import",
    job => job.ExecuteAsync(),
    builder.Configuration["FileIntegration:CronExpression"] ?? @"*/5 * * * *");

app.Run();
