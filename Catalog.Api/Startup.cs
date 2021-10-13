using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Api.Repositories;
using Catalog.Api.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Catalog.Api
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
            //To indicate how to serialize the date and the Guid to MongoDb
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
            var mongoDbSettings=Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();  

            //Configure MongoDbClient
            services.AddSingleton<IMongoClient>(ServiceProvider => 
            {              
              return new MongoClient(mongoDbSettings.ConnectionString);
            });

            //1-to register the dependency in the service provider 
            //singleton = one copy of the instance of a type across entire life time of our service  
            //services.AddSingleton<IItemsRepository,InMemItemsRepository>();
            
            //2-to switch to MongoRepository
             services.AddSingleton<IItemsRepository,MongoDbItemsRepository>();

            services.AddControllers(options=>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
            });
          
            //for check health service that it's run or not
            services.AddHealthChecks()
             .AddMongoDb(                      //for cheking healt of mongodb
                 mongoDbSettings.ConnectionString,
                 name:"mongodb",
                 timeout: TimeSpan.FromSeconds(3),
                 tags: new[]{"ready"}); //use tag to check that the service can receive requests
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                
                // endpoints.MapHealthChecks("/health/ready",new HealthCheckOptions{
                //     Predicate = (check) => check.Tags.Contains("ready"),
                //     ResponseWriter=async(context,report)=>
                //     {
                //         var result=JsonSerializer.serialize(
                            
                //                 new{
                //                     status=report.Status.ToString();
                //                     checks=report.Entries.Select(entry=>new {
                //                         name = entry.key,
                //                         status = entry.Value.Status.ToString(),
                //                         exception = entry.Value.Exception!=null ? entry.Value.Exception.Message:"none",
                //                         duration = entry.Value.Duration.ToString()
                //                     })
                //                 }                            
                //         );
                //     }
                // });
                
                //format the content in json 
                // context.Response.ContentType = MediaTypesNames.Application.Json;
                // await context.Response.WriteAsync(result)

                // endpoints.MapHealthChecks("/health/live",new HealthCheckOptions{
                //     Predicate = (_) => false
                // });

            });
        }
    }
}
