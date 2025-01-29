var builder = WebApplication.CreateBuilder(args);

// Добавление MVC-контроллеров и представлений
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Task}/{action=Index}/{id?}"); // <-- Теперь по умолчанию TaskController
});

app.Run();