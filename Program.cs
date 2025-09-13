// This is the Program.cs file with full support for controllers and Swagger.

// 1. Create a new web application builder.
var builder = WebApplication.CreateBuilder(args);

// 2. Add controllers as a service.
// This is the core service that enables controller-based API endpoints.
builder.Services.AddControllers();

// Add DORS services and define a policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "HelloWorldCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 3. Add OpenAPI services to the container.
// The AddEndpointsApiExplorer() service enables the API Explorer feature,
// which is used to discover and describe the API endpoints.
builder.Services.AddEndpointsApiExplorer();

// The AddSwaggerGen() service generates the Swagger specification
// for your API. This is what the documentation UI is built on.
builder.Services.AddSwaggerGen();

// 4. Build the application.
var app = builder.Build();

// 5. Configure the HTTP request pipeline.
// In a development environment, you should enable Swagger UI.
// This block ensures the documentation is only available when you're
// developing the application.
if (app.Environment.IsDevelopment())
{
    // UseSwagger() exposes the generated Swagger JSON file.
    app.UseSwagger();
    // UseSwaggerUI() provides the interactive web-based UI for
    // exploring and testing your API.
    app.UseSwaggerUI();
}

// Enable HTTPS redirection. This is a best practice for security.
app.UseHttpsRedirection();

app.UseCors("HelloWorldCorsPolicy");

// 6. Map controllers to handle requests.
// This tells the application to route incoming HTTP requests
// to the correct controller and method based on the defined
// attributes like [Route] and [HttpGet].
app.MapControllers();

// 7. Run the application.
app.Run();
