using CSS.Challenge.Web.Hubs;
using CSS.Challenge.Web.Utilities;

namespace CSS.Challenge.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            // Add SignalR services
            builder.Services.AddSignalR();
            builder.Services.AddControllers(); // Add controller services
            builder.Services.AddSingleton<LogBroadcaster>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();
            // Map SignalR hub
            app.MapHub<LogHub>("/logHub");
            app.MapControllers(); // Map controller routes

            app.Run();
        }
    }
}
