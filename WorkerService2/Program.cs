using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkerService2;
using Microsoft.Extensions.Hosting.WindowsServices;



var host = Host.CreateDefaultBuilder(args)
    .UseWindowsService() // <- quan trọng: cho phép chạy dưới Windows Service
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // Supabase
        var supabaseUrl = configuration["Supabase:Url"];
        var supabaseKey = configuration["Supabase:Key"];
        var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
        services.AddSingleton(supabaseClient);

        // Repository + BLL
        services.AddScoped<IImageRepository>(provider => new ImageRepository(connectionString));
        services.AddScoped<IImageService, ImageService>();

        // Hosted Worker
        services.AddHostedService<WorkerService2.Worker>();
    })
    .Build();

await host.RunAsync();