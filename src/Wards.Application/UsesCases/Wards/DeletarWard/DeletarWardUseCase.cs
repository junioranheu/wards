﻿using Wards.Application.UsesCases.Wards.DeletarWard.Commands;

namespace Wards.Application.UsesCases.Wards.DeletarWard
{
    public sealed class DeletarWardUseCase : IDeletarWardUseCase
    {
        private readonly IDeletarWardCommand _deletarCommand;

        public DeletarWardUseCase(IDeletarWardCommand deletarCommand)
        {
            _deletarCommand = deletarCommand;
        }

        public async Task<bool> Execute(int id)
        {
            return await _deletarCommand.Execute(id);
        }
    }
}