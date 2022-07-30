using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(o => o.AddPolicy("AllowAnyOrigin",
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      }));



var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowAnyOrigin");

string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
app.MapGet("/", () =>
{
    return "Welcome to JukaAPI version "+ assemblyVersion + "! To execute a program, send a GET request to \"/code_you_want_to_execute\".";
});


var executeCode = (string src) =>
{
    JukaCompiler.Compiler compiler = new JukaCompiler.Compiler();
    string decoded = Uri.UnescapeDataString(src);
    var outputValue = compiler.Go(decoded, false);

    if (compiler.HasErrors())
    {
        string errors = string.Join(Environment.NewLine, compiler.ListErrors());
        return Results.Json(new { errors = errors, original = decoded });
    }
    return Results.Json(new { output = outputValue, original = decoded });
};

app.MapGet("/{*src}", (string src) => executeCode(src));

app.MapPost("/{*src}", (string src) => executeCode(src));


app.Run();