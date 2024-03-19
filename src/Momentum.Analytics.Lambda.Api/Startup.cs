using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.DynamoDb.PageViews;
using Momentum.Analytics.DynamoDb.Pii;
using Momentum.Analytics.DynamoDb.Visits;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.Analytics.Lambda.Api.PageViews;
using Momentum.Analytics.Lambda.Api.Pii;
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
            .AddLogging()
            .AddMemoryCache()
            .AddPageViewService()
            //.AddNoopPageViewService()
            .AddDynamoDbPiiService()
            //.AddNoopPiiService()
            .AddDynamoDbVisitService()
            .AddHttpContextAccessor()
            .AddTransient<ICookieWriter, CookieWriter>();

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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    } // end method
} // end method