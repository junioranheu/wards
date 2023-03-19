﻿using Wards.Domain.Entities;
using Wards.Infrastructure.Data;

namespace Wards.Application.UsesCases.Wards.CriarWard.Commands
{
    public sealed class CriarWardCommand
    {
        private readonly WardsContext _context;

        public CriarWardCommand(WardsContext context)
        {
            _context = context;
        }

        public async Task<int> Execute(Ward input)
        {
            await _context.AddAsync(input);
            await _context.SaveChangesAsync();

            return input.WardId;
        }
    }
}