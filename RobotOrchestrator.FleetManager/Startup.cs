using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace RobotOrchestrator.FleetManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IotHubRegistryClientOptions>(Configuration);
            services.AddSingleton<IIotHubRegistryClient, IotHubRegistryClient>();

            services.AddSingleton<ITelemetryHandler, TelemetryHandler>();
            services.AddSingleton<IFleetManager, FleetManager>();

            ConfigureDatabaseServices(services);
            ConfigureEventProcessorHostServices(services);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigin",
                    builder => builder.WithOrigins("*"));
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // add versioning, the default is 1.0
            services.AddApiVersioning();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "RobotOrchestrator.FleetManager", Version = "v1" });

                c.DescribeAllEnumsAsStrings();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RobotOrchestrator.FleetManager V1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void ConfigureDatabaseServices(IServiceCollection services)
        {
            services.Configure<CosmosDbOptions<RobotTelemetry>>(Configuration.GetSection("Telemetry"));
            services.AddSingleton<ICosmosDbClient<Robot>, CosmosDbClient<Robot>>();

            services.Configure<CosmosDbOptions<Robot>>(Configuration.GetSection("Robot"));
            services.AddSingleton<ICosmosDbClient<RobotTelemetry>, CosmosDbClient<RobotTelemetry>>();
        }

        private void ConfigureEventProcessorHostServices(IServiceCollection services)
        {
            services.AddSingleton<IEventProcessor, TelemetryEventProcessor>();
            services.AddSingleton<IEventProcessorFactory, IotHubEventProcessorFactory>();
            services.AddSingleton<IEventProcessorHostConfig>(new EventProcessorHostConfig()
            {
                EventHubConnectionString = Configuration.GetValue<string>("FleetManagerEventHubConnectionString"),
                ConsumerGroupName = Configuration.GetValue<string>("FleetManagerEventHubConsumerGroup"),
                EventHubPath = Configuration.GetValue<string>("FleetManagerEventHubPath"),
                StorageConnectionString = Configuration.GetValue<string>("BlobStorageConnectionString"),
                LeaseContainerName = "telemetryleases"
            });

            services.Configure<EventProcessorOptions>(options =>
            {
                options.MaxBatchSize = 10;
                options.PrefetchCount = 100;
                options.InitialOffsetProvider = (partitionId) => EventPosition.FromEnqueuedTime(DateTime.UtcNow);
            });

            services.AddHostedService<IotHubListener>();
        }
    }
}
