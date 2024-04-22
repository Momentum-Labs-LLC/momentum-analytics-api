using Amazon.DynamoDBv2;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.DynamoDb.PageViews;
using Momentum.Analytics.DynamoDb.Pii;
using Momentum.Analytics.DynamoDb.Visits;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.FromCookie;

namespace Momentum.Analytics.Lambda.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging(config => 
            {
                config.AddFilter("Microsoft", LogLevel.Warning);
                config.AddFilter("System", LogLevel.Warning);
                config.SetMinimumLevel(LogLevel.Error);
                config.AddLambdaLogger();
            })
            .AddMemoryCache()
            .AddSingleton<IConfiguration>(Configuration)
            .AddVisitWindowCalculator()
            .AddNodaTime()
            .AddPageViewService()
            .AddDynamoDbPiiService()
            .AddDynamoDbVisitService()
            .AddCookieWriter();
        

        var corsOriginsValue = Configuration.GetValue<string>(ApiConstants.CORS_ORIGINS, ApiConstants.CORS_ORIGINS_DEFAULT);
        var corsOrigins = corsOriginsValue.Split(",").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim());

        
        services.AddCors(corsBuilder => 
        {
            corsBuilder.AddDefaultPolicy(policyBuilder => 
            {
                policyBuilder.WithOrigins(corsOrigins.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddMvcCore(options => 
            {
                options.AddFromCookieBinder();
            });
        services.AddControllers();
    } // end method

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();            
        } // end if

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();
        app.UseCors();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    } // end method
} // end method