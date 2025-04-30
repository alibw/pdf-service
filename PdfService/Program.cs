using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using PdfService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/Print", async (HttpContext context) =>
{
    var request = context.Request;
    var boundary = request.ContentType.Split("boundary=")[1];
    var reader = new MultipartReader(boundary, request.Body);

    MultipartSection section;
    var parts = new List<(string ContentType, string Content, string ConentDisposition)>();

    while ((section = await reader.ReadNextSectionAsync()) != null)
    {
        using var sr = new StreamReader(section.Body);
        var content = await sr.ReadToEndAsync();
        var contentType = section.ContentType;
        var contentDisposition = section.ContentDisposition;

        parts.Add((contentType, content, contentDisposition));
    }

    string reportTemplate = "";
    string fileName = "";
    dynamic data = null;

    foreach (var part in parts)
    {
        if (part.ContentType == "application/json")
        {
            data = JsonConvert.DeserializeObject(part.Content);
        }
        else if (part.ContentType == "application/xml")
        {
            reportTemplate = part.Content;
            fileName = part.ConentDisposition?.Split(".")[1];
        }
    }

    var reportStream = Reporter.Print(reportTemplate, data);
    return Results.File(reportStream, "application/pdf", $"{fileName}.pdf");
});

app.Run();