using ManejoPresupuestos.Models;
using ManejoPresupuestos.Servicios;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Es AddTransient porque esta vez no compartiremos datos
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>();
builder.Services.AddTransient<IRepositorioTransacciones, RepositorioTransacciones>();
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioUsuarios>();

//configuramos Identity
builder.Services.AddTransient<IUserStore<Usuario>, UsuarioStore>();
builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    opciones.Password.RequireDigit = false;
    opciones.Password.RequireLowercase = false;
    opciones.Password.RequireUppercase = false;
    opciones.Password.RequireNonAlphanumeric = false;

}).AddErrorDescriber<MensajesDeErrorIdentity>();
//----

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServicioReportes, ServicioReportes>();

builder.Services.AddAutoMapper(typeof(Program)); //configuramos autoMapper

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


/*
 CUANDO USAMOS 
 /{extra?}/{full?}
    Significa que cuando usemos las etiquetas <a> con el atributo asp-route-extra="extra" o asp-route-full="fua"
    significa que los valores en el enrutamiento se veran afectadas, es decir que el valor sera asignado en la ruta,
    si se usa asp-route-atributo="atributo" este no sera reconocido por la ruta y simplemente sera usado en la ruta directamente como
    /?atributo="atributo" y como con /extra/fua/atributo/
 */
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");
//pattern: "{controller=Home}/{action=Index}/{id?}/{extra?}/{full?}");

app.Run();
