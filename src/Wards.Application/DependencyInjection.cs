﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wards.Application.AutoMapper;
using Wards.Application.Services.Import;
using Wards.Application.Services.Sistemas;
using Wards.Application.Services.Usuarios;
using Wards.Application.UseCases.Feriados;
using Wards.Application.UseCases.FeriadosDatas;
using Wards.Application.UseCases.FeriadosEstados;
using Wards.Application.UsesCases.Auxiliares;
using Wards.Application.UsesCases.Imports;
using Wards.Application.UsesCases.Logs;
using Wards.Application.UsesCases.Tokens;
using Wards.Application.UsesCases.Usuarios;
using Wards.Application.UsesCases.UsuariosRoles;
using Wards.Application.UsesCases.Wards;

namespace Wards.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjectionApplication(this IServiceCollection services, WebApplicationBuilder builder)
        {
            AddAutoMapper(services);
            AddLogger(builder);

            // UseCases;
            services.AddImportsApplication();
            services.AddLogsApplication();
            services.AddTokensApplication();
            services.AddUsuariosApplication();
            services.AddUsuariosRolesApplication();
            services.AddWardsApplication();
            services.AddAuxiliaresApplication();
            services.AddFeriadosApplication();
            services.AddFeriadosDatasApplication();
            services.AddFeriadosEstadosApplication();

            // Services;
            services.AddCsvImportService();
            services.AddUsuariosService();
            services.AddResetarBancoDadosService();

            return services;
        }

        private static void AddAutoMapper(IServiceCollection services)
        {
            var mapperConfig = new MapperConfiguration(x =>
            {
                x.AddProfile(new AutoMapperConfig());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private static void AddLogger(WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
        }
    }
}