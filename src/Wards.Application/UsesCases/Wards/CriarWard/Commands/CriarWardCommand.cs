﻿using Microsoft.Extensions.Logging;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Queries;
using Wards.Domain.Entities;
using Wards.Infrastructure.Data;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Wards.CriarWard.Commands
{
    public sealed class CriarWardCommand : ICriarWardCommand
    {
        private readonly WardsContext _context;
        private readonly ILogger _logger;

        public CriarWardCommand(WardsContext context, ILogger<ListarEstadoQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Execute(Ward input)
        {
            try
            {
                await _context.AddAsync(input);
                await _context.SaveChangesAsync();

                return input.WardId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, HorarioBrasilia().ToString());
                throw;
            }
        }
    }
}