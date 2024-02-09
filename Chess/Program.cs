using Chess.Data;
using Chess.Hubs;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog;



Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <{SourceContext}>{NewLine}{Exception}")
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
            // Add any other sinks or enrichers as needed
            .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<ApplicationDbContext>(option =>
    {
        option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
    });

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(10);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    builder.Services.AddMvc().AddRazorOptions(options =>
    {
        options.ViewLocationFormats.Add("/Views/Pages/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    });

    // Add SignalR services
    builder.Services.AddSignalR();

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    app.Use(async (context, next) =>
    {
        if (!context.Session.Keys.Contains("LoggedIn"))
        {
            context.Session.SetInt32("LoggedIn", 0);
            context.Session.SetString("SessionUser", "0");
        }
        await next();
    });

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Pages}/{action=Home}");

    // Map the SignalR hub endpoint
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHub<ChessHub>("/ChessHub");
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Pages}/{action=Home}/{id?}");
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush(); // Ensure all pending log events are written before exiting
}
