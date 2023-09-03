using ManejoPresupuestos.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Es AddTransient porque esta vez no compartiremos datos
builder.Services.AddTransient<IRepositorioTiposCuentas, RepositorioTiposCuentas>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
    

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
    pattern: "{controller=Home}/{action=Index}/{id?}");
//pattern: "{controller=Home}/{action=Index}/{id?}/{extra?}/{full?}");

app.Run();
