using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerUI;
using Wards.API;
using Wards.Application;
using Wards.Domain.Consts;
using Wards.Infrastructure;
using Wards.Infrastructure.Data;
using Wards.Infrastructure.Seed;
using Wards.WorkersServices;

#region builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddDependencyInjectionAPI();
    builder.Services.AddDependencyInjectionApplication(builder);
    builder.Services.AddDependencyInjectionInfrastructure(builder);
    builder.Services.AddDependencyInjectionWorkersServices(builder);
}
#endregion

#region app
WebApplication app = builder.Build();
{
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;

    if (app.Environment.IsDevelopment())
    {
        await DBInitialize(app, services, isInitialize: false);

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SistemaConst.NomeSistema}.API");
            // c.RoutePrefix = ""; 
            c.DocExpansion(DocExpansion.None);
        });

        app.UseDeveloperExceptionPage();
    }

    if (app.Environment.IsProduction())
    {
        app.UseHttpsRedirection();
    }

    app.UseCors(builder.Configuration["CORSSettings:Cors"]!);
    app.UseResponseCompression();

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    AddHealthCheck(app);
    AddStaticFiles(app);

    app.Run();
}
#endregion

#region metodos_auxiliares
static async Task DBInitialize(WebApplication app, IServiceProvider services, bool isInitialize)
{
    if (!isInitialize)
    {
        return;
    }

    try
    {
        WardsContext context = services.GetRequiredService<WardsContext>();
        await DbInitializer.Initialize(context, isAplicarMigrations: true, isResetar: true);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "{detalhes}", "Houve um problema ao resetar a base de dados");
    }
}

static void AddHealthCheck(WebApplication app)
{
    app.UseHealthChecks("/status", new HealthCheckOptions()
    {
        ResponseWriter = (httpContext, result) =>
        {
            httpContext.Response.ContentType = "application/json";

            #region objeto_json
            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("results", new JObject(result.Entries.Select(pair =>
                    new JProperty(pair.Key, new JObject(
                        new JProperty("status", pair.Value.Status.ToString()),
                        new JProperty("description", pair.Value.Description),
                        new JProperty("data", new JObject(pair.Value.Data.Select(p => new JProperty(p.Key, p.Value))))))))));
            #endregion

            return httpContext.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    });
}

static void AddStaticFiles(WebApplication app)
{
    IWebHostEnvironment env = app.Environment;

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Uploads")),
        RequestPath = "/Uploads",

        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            ctx.Context.Response.Headers.Append("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
        }
    });
}
#endregion