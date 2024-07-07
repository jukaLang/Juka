using Microsoft.OpenApi.Models;
using System.Reflection;

const string DebugVersion = "DEBUG";
const string InitialVersion = "0.0.0.1";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? DebugVersion;
if (assemblyVersion == InitialVersion)
{
    assemblyVersion = DebugVersion;
}

ConfigureServices(builder, assemblyVersion);
WebApplication app = builder.Build();
Configure(app);

app.Run();

void ConfigureServices(WebApplicationBuilder builder, string assemblyVersion)
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Juka Programming Language API",
            Description = $"Welcome to JukaAPI version {assemblyVersion}! To execute a program, send a GET or a POST request to \"/code_you_want_to_execute\". You can also send a POST request to '/' to execute code embedded in body (raw).",
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
}

void Configure(WebApplication app)
{
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

    app.MapGet("/{*code:regex(^(?!index\\.html|swagger-ui\\.css|swagger-ui-bundle\\.js|swagger-ui-standalone-preset\\.js|favicon-32x32\\.png))}", ExecuteCode).WithName("Run Juka (Short)");
    app.MapPost("/{*code}", ExecuteCode).WithName("Run Juka POST");
    app.MapPost("/", ExecuteCodeFromBody).WithName("Run Juka POST (Long)");
}

static IResult ExecuteCode(string code)
{
    JukaCompiler.Compiler compiler = new();
    string decoded = Uri.UnescapeDataString(code);
    string outputValue = compiler.Go(decoded, isFile: false);

    if (compiler.HasErrors())
    {
        string errors = string.Join(Environment.NewLine, compiler.ListErrors());
        return Results.Ok(new { errors, original = decoded });
    }

    return Results.Ok(new { output = outputValue, original = decoded });
}

static async Task<IResult> ExecuteCodeFromBody(HttpRequest request)
{
    StreamReader stream = new(request.Body);
    string code = await stream.ReadToEndAsync();
    return ExecuteCode(code);
}
