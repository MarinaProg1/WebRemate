
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using WebRemate.Interfaces;
//using WebRemate.Service;

//var builder = WebApplication.CreateBuilder(args);
//var configuration = builder.Configuration;

//// Configurar HttpClient para las APIs
//builder.Services.AddHttpClient<IUsuarioApiService, UsuarioApiService>();
//builder.Services.AddHttpClient<IRemateApiService, RemateApiService>();
//builder.Services.AddHttpClient<IProductoApiService, ProductoApiService>();

//// Agregar servicios
//builder.Services.AddControllersWithViews();
//builder.Services.AddHttpContextAccessor();
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//// Configurar autenticación JWT + Cookies
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//{
//    options.LoginPath = "/Autenticacion/Login";
//    options.LogoutPath = "/Autenticacion/Logout";
//})
//.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//{
//    options.RequireHttpsMetadata = false; // Cambiar a true en producción
//    options.SaveToken = true;
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
//    };
//});

//// Configurar sesiones
//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromHours(1);
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//var app = builder.Build();

//// Configuración del pipeline de la aplicación
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//}

//app.UseStaticFiles();
//app.UseRouting();
//app.UseSession();
//app.UseAuthentication();
//app.UseAuthorization();

//// Configuración de rutas
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Remate}/{action=Index}/{id?}");

//app.Run();
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebRemate.Interfaces;
using WebRemate.Service;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Configurar HttpClient para las APIs
builder.Services.AddHttpClient<IUsuarioApiService, UsuarioApiService>();
builder.Services.AddHttpClient<IRemateApiService, RemateApiService>();
builder.Services.AddHttpClient<IProductoApiService, ProductoApiService>();
builder.Services.AddHttpClient<IOfertaApiService, OfertaApiService>();

// Agregar servicios
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configurar autenticación JWT + Cookies
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Usar cookies por defecto
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Autenticacion/Login"; // Ruta para redirigir al login si no está autenticado
    options.LogoutPath = "/Autenticacion/Logout"; // Ruta para cerrar sesión
    options.AccessDeniedPath = "/Home/AccessDenied"; // Ruta de acceso denegado
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // La sesión dura 30 minutos
    options.SlidingExpiration = false; // No extiende la expiración con actividad
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; 

})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false; // Cambiar a true en producción
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
    };
});

// Configurar sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuración del pipeline de la aplicación
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Asegúrate de que esté antes de UseAuthorization()
app.UseAuthorization();

// Configuración de rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Remate}/{action=Index}/{id?}");

app.Run();
