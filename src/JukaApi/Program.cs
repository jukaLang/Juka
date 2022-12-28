using Microsoft.OpenApi.Models;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "DEBUG";
if (assemblyVersion == "0.0.0.1")
{
    assemblyVersion = "DEBUG";
}


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Juka Programming Language API",
        Description = "Welcome to JukaAPI version "+ assemblyVersion + "! To execute a program, send a GET or a POST request to \\\"/code_you_want_to_execute\\\". You can also send a POST request to '/' to execute code embedded in body (raw).",
        TermsOfService = new Uri("https://jukalang.com/"),
        Contact = new OpenApiContact
        {
            Name = "Contact Juka Language Team",
            Url = new Uri("https://jukalang.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "AGPL GNU License",
            Url = new Uri("https://jukalang.com/license")
        }
    });
});
builder.Services.AddCors(o => o.AddPolicy("AllowAnyOrigin",
                      policyBuilder =>
                      {
                          policyBuilder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      }));



WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger(c =>
{
    c.RouteTemplate = "{documentname}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAnyOrigin");



IResult ExecuteCode(string src)
{
    JukaCompiler.Compiler compiler = new();
    string decoded = Uri.UnescapeDataString(src);
    string outputValue = compiler.Go(decoded, isFile:false);

    if (compiler.HasErrors())
    {
        Console.WriteLine("Error!");
        string errors = string.Join(Environment.NewLine, compiler.ListErrors());
        return Results.Json(new { errors, original = decoded });
    }

    Console.WriteLine(outputValue);
    return Results.Json(new { output = outputValue, original = decoded});
}

app.MapGet("/{code:regex(^(?!index\\.html|swagger-ui\\.css|swagger-ui-bundle\\.js|swagger-ui-standalone-preset\\.js))}", ExecuteCode).WithName("Run Juka (Short)");

app.MapPost("/{*code}", ExecuteCode).WithName("Run Juka POST");

app.MapPost("/", async (HttpRequest request) =>
{
    StreamReader stream = new(request.Body);
    string src = await stream.ReadToEndAsync();
    return ExecuteCode(src);
}).WithName("Run Juka POST (Long)");

app.Run();