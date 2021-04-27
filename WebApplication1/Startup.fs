namespace WebApplication1

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Hangfire
open Hangfire.Mongo
open System

type Startup() =

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    member this.ConfigureServices(services: IServiceCollection) =
        services.AddHangfire(fun (configuration: IGlobalConfiguration) -> configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                                                                                       .UseSimpleAssemblyNameTypeSerializer()
                                                                                       .UseRecommendedSerializerSettings()
                                                                                       .UseMongoStorage("mongodb://localhost:27017/HangfireJobs",
                                                                                                        new MongoStorageOptions(MigrationOptions = new MongoMigrationOptions(MigrationStrategy = new Migration.Strategies.DropMongoMigrationStrategy()),
                                                                                                                                QueuePollInterval = TimeSpan.FromMilliseconds(2000.0),
                                                                                                                                CheckConnection = false)) |> ignore) |> ignore

        //services.AddHangfireServer() |> ignore

        services.AddSingleton<BackgroundWorker.Worker>() |> ignore
        services.AddSingleton<GoferWorker.Worker>() |> ignore
        ()

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment, serviceProvider: IServiceProvider) =
        if env.IsDevelopment() then
            app.UseDeveloperExceptionPage() |> ignore

        app.UseRouting() |> ignore

        app.UseEndpoints(fun endpoints -> ()) |> ignore

        let worker = serviceProvider.GetService<GoferWorker.Worker>()
        worker.Start() |> Async.RunSynchronously

        //let worker = serviceProvider.GetService<BackgroundWorker.Worker>()
        //worker.StartJob()
