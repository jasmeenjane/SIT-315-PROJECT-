using MedChain;

var builder = WebApplication.CreateBuilder(args);

// Register gRPC services
builder.Services.AddGrpc();

var app = builder.Build();

// Map gRPC services
app.MapGrpcService<MedicalRecordService>();


// Basic HTTP GET response (for browser info)
app.MapGet("/", () =>
"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909"
);

app.Run("http://0.0.0.0:5000");