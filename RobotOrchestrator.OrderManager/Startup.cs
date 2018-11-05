using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace RobotOrchestrator.OrderManager
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
            services.AddSingleton<IOrderManager, OrderManager>();
            services.AddSingleton<IDispatcherClient>(new DispatcherClient(Configuration.GetValue<string>("DispatcherUrl")));

            services.Configure<FleetManagerClientOptions>(Configuration);
            services.AddSingleton<IFleetManagerClient, FleetManagerClient>();

            services.Configure<CosmosDbOptions<Order>>(Configuration.GetSection("Order"));
            services.AddSingleton<ICosmosDbClient<Order>, CosmosDbClient<Order>>();
            services.AddSingleton<IJobMessageHandler, JobMessageHandler>();

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
                c.SwaggerDoc("v1", new Info { Title = "RobotOrchestrator.OrderManager", Version = "v1" });

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

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RobotOrchestrator.OrderManager V1");
            });

            app.UseMvc();
        }

        private void ConfigureEventProcessorHostServices(IServiceCollection services)
        {
            services.AddSingleton<IEventProcessor, JobEventProcessor>();
            services.AddSingleton<IEventProcessorFactory, IotHubEventProcessorFactory>();
            services.AddSingleton<IEventProcessorHostConfig>(new EventProcessorHostConfig()
            {
                EventHubConnectionString = Configuration.GetValue<string>("OrderManagerEventHubConnectionString"),
                ConsumerGroupName = Configuration.GetValue<string>("OrderManagerEventHubConsumerGroup"),
                EventHubPath = Configuration.GetValue<string>("OrderManagerEventHubPath"),
                StorageConnectionString = Configuration.GetValue<string>("BlobStorageConnectionString"),
                LeaseContainerName = "orderleases"
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
