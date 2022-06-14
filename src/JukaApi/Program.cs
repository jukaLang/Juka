using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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


app.MapGet("/", () =>
{
    return "Welcome to JukaAPI! To execute a program, send a GET request to \"/code_you_want_to_execute\".";
});


app.MapGet("/{src}", (string src) =>
{
    JukaCompiler.Compiler compiler = new JukaCompiler.Compiler();
    string sourceAsString = src;
    var outputValue = compiler.Go(sourceAsString, false);

    if (compiler.HasErrors())
    {
        var errors = compiler.ListErrors().ToString();
        return JsonSerializer.Serialize(new { errors = errors });
    }
    return JsonSerializer.Serialize(new { output = outputValue });
});

app.Run();
