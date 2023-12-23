using chatt.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static chatt.Models.mychat;

namespace chatt
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.
            //builder.Services.AddControllersWithViews();

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();

            //app.UseRouting();

            //app.UseAuthorization();

            //app.MapControllerRoute(
            //    name: "default",
            //pattern: "{controller=Home}/{action=Index}/{id?}");


            var builder = WebApplication.CreateBuilder(args);

            string connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection)); //connectDB
            builder.Services.AddSignalR();      // подключема сервисы SignalR

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.MapHub<ChatHub>("/chatt");   // ChatHub будет обрабатывать запросы по пути /chat

            app.Run();

        }
    }
}