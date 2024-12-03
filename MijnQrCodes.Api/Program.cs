using MediatR;
using MijnQrCodes.Application;
using MijnQrCodes.Application.Queries.RenderQrCodeQuery;
using MijnQrCodes.Application.Queries.ResolveLinkQuery;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




app.MapGet("/qr/{code}", async (HttpContext context, string code, IMediator mediator, CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new ResolveLinkQuery { Code = code }, cancellationToken);

    context.Response.Redirect(response.Url);
});

app.MapGet("/qr/{code}/download", async (HttpContext context, string code, IMediator mediator, CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new RenderQrCodeQuery { Code = code }, cancellationToken);

    return Results.File(response.QrCodeImage, "image/png");
});




app.Run();