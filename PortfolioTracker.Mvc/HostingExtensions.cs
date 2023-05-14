using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Data;
using PortfolioTracker.Data.Repositories;
using PortfolioTracker.EntityModels.Contracts;
using PortfolioTracker.Mvc.Data;

namespace PortfolioTracker.Mvc;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("PortfolioTrackerConnection") ?? throw new InvalidOperationException("Connection string 'Default' not found.");
        builder.Services.AddDbContext<PortfolioTrackerDbContext>(options =>
            options.UseSqlServer(connectionString));

        var identityConnectionString = builder.Configuration.GetConnectionString("DefaultIdentity") ?? throw new InvalidOperationException("Connection string 'DefaultIdentity' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(identityConnectionString));

        builder.Services.AddScoped<IAssetRepository, AssetRepository>();

        builder.Services.AddHttpClient(name: "CapitalMarketDataWebApi",
            configureClient: options =>
            {
                options.BaseAddress = new Uri("http://localhost:5000");
                options.DefaultRequestHeaders.Accept.Add(
                   new MediaTypeWithQualityHeaderValue(mediaType: "application/json", quality: 1.0));
            });

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        }

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();

            // Generates HTML error responses
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Adds a middleware to the pipeline that will catch exceptions, log them, and re-execute the request in an alternate pipeline.
            app.UseExceptionHandler("/Home/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Endpoint routing
        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        return app;
    }
}
