﻿using Dapper;
using System.Data;
using Wards.Application.UsesCases.Usuarios.Shared.Models;

namespace Wards.Application.UsesCases.Usuarios.AtualizarUsuario.Commands
{
    public sealed class AtualizarUsuarioCommand : IAtualizarUsuarioCommand
    {
        private readonly IDbConnection _dbConnection;

        public AtualizarUsuarioCommand(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> ExecuteAsync(UsuarioDTO dto)
        {
            string sql = "";

            return await _dbConnection.ExecuteAsync(sql, dto);
        }
    }
}
