using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(o => o.AddPolicy("AllowAnyOrigin",
                      policyBuilder =>
                      {
                          policyBuilder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      }));



WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAnyOrigin");

string? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
app.MapGet("/", () => 
    "Welcome to JukaAPI version "+ assemblyVersion + "! To execute a program, send a GET or a POST request to \"/code_you_want_to_execute\". You can also send a POST request to '/' to execute code embedded in body (raw).");


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

app.MapGet("/{*src}", ExecuteCode);

app.MapPost("/{*src}", ExecuteCode);

app.MapPost("/", async (HttpRequest request) =>
{
    StreamReader stream = new(request.Body);
    string src = await stream.ReadToEndAsync();
    return ExecuteCode(src);
});

app.Run();