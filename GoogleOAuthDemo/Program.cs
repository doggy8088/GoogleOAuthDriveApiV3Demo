using Google.Apis.Auth.AspNetCore3;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(o =>
        {
            // This forces challenge results to be handled by Google OpenID Handler, so there's no
            // need to add an AccountController that emits challenges for Login.
            o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
            // This forces forbid results to be handled by Google OpenID Handler, which checks if
            // extra scopes are required and does automatic incremental auth.
            o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
            // Default scheme that will handle everything else.
            // Once a user is authenticated, the OAuth2 token info is stored in cookies.
            o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddGoogleOpenIdConnect(options =>
        {
            options.ClientId = builder.Configuration.GetValue<string>("GoogleOAuth:ClientId");
            options.ClientSecret = builder.Configuration.GetValue<string>("GoogleOAuth:ClientSecret");
        });

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
