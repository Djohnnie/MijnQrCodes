using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MudBlazor.Services;
using MijnQrCodes.Application._di;
using MijnQrCodes.Contracts.Auth;
using MijnQrCodes.Dashboard.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddApplication();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
});

app.MapPost("/api/login", async (HttpContext context, IMediator mediator) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/login?error=" + Uri.EscapeDataString("Vul gebruikersnaam en wachtwoord in."));
    }

    var result = await mediator.Send(new LoginQuery { Username = username, Password = password });

    if (!result.Success)
    {
        return Results.Redirect("/login?error=" + Uri.EscapeDataString(result.Error!));
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
        new(ClaimTypes.Name, result.Username),
        new("MustChangePassword", result.MustChangePassword.ToString())
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect(result.MustChangePassword ? "/change-password" : "/");
}).DisableAntiforgery();

app.MapPost("/api/register", async (HttpContext context, IMediator mediator) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect("/register?error=" + Uri.EscapeDataString("Vul alle velden in."));
    }

    if (password.Length < 6)
    {
        return Results.Redirect("/register?error=" + Uri.EscapeDataString("Wachtwoord moet minimaal 6 tekens bevatten."));
    }

    if (password != confirmPassword)
    {
        return Results.Redirect("/register?error=" + Uri.EscapeDataString("Wachtwoorden komen niet overeen."));
    }

    var result = await mediator.Send(new RegisterCommand { Username = username, Password = password });

    if (!result.Success)
    {
        return Results.Redirect("/register?error=" + Uri.EscapeDataString(result.Error!));
    }

    return Results.Redirect($"/register?registered=true&autoApproved={result.AutoApproved.ToString().ToLower()}");
}).DisableAntiforgery();

app.MapPost("/api/change-password", async (HttpContext context, IMediator mediator) =>
{
    if (context.User.Identity?.IsAuthenticated != true)
    {
        return Results.Redirect("/login");
    }

    var form = await context.Request.ReadFormAsync();
    var newPassword = form["newPassword"].ToString();
    var confirmPassword = form["confirmPassword"].ToString();

    if (string.IsNullOrWhiteSpace(newPassword))
    {
        return Results.Redirect("/change-password?error=" + Uri.EscapeDataString("Vul een nieuw wachtwoord in."));
    }

    if (newPassword.Length < 6)
    {
        return Results.Redirect("/change-password?error=" + Uri.EscapeDataString("Wachtwoord moet minimaal 6 tekens bevatten."));
    }

    if (newPassword != confirmPassword)
    {
        return Results.Redirect("/change-password?error=" + Uri.EscapeDataString("Wachtwoorden komen niet overeen."));
    }

    var userId = Guid.Parse(context.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var success = await mediator.Send(new ChangePasswordCommand { UserId = userId, NewPassword = newPassword });

    if (!success)
    {
        return Results.Redirect("/change-password?error=" + Uri.EscapeDataString("Er is iets misgegaan. Probeer opnieuw."));
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, userId.ToString()),
        new(ClaimTypes.Name, context.User.FindFirstValue(ClaimTypes.Name)!),
        new("MustChangePassword", "False")
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
