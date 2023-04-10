﻿using Microsoft.AspNetCore.Http;
using Quartz;
using Wards.Application.UsesCases.Logs.CriarLog.Commands;
using Wards.Application.UsesCases.Usuarios.ListarUsuario.Queries;
using Wards.Domain.Entities;
using static Wards.Utils.Common;

namespace Wards.WorkersServices.Workers.Temperatura.Jobs.ObterTemperatura
{
    [DisallowConcurrentExecution]
    public sealed class ObterTemperaturaJob : IJob, IObterTemperaturaJob
    {
        private readonly ICriarLogCommand _criarLogCommand;
        private readonly IListarUsuarioQuery _listarUsuarioQuery;

        public ObterTemperaturaJob(ICriarLogCommand criarLogCommand, IListarUsuarioQuery listarUsuarioQuery)
        {
            _criarLogCommand = criarLogCommand;
            _listarUsuarioQuery = listarUsuarioQuery;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                // Console.Clear();
                await Console.Out.WriteLineAsync($"Olá, agora são {HorarioBrasilia()}");

                Log log = new() { Descricao = $"Sucesso no Worker {typeof(ObterTemperaturaJob)}", StatusResposta = StatusCodes.Status200OK };
                await _criarLogCommand.Execute(log);
            }
            catch (Exception ex)
            {
                Log log = new() { Descricao = $"Houve um erro no Worker {typeof(ObterTemperaturaJob)}: {ex.Message}", StatusResposta = StatusCodes.Status500InternalServerError };
                await _criarLogCommand.Execute(log);
            }
        }
    }
}