﻿using Dapper;
using System.Data;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Logs.CriarLog.Commands
{
    public sealed class CriarLogCommand : ICriarLogCommand
    {
        private readonly IDbConnection _dbConnection;

        public CriarLogCommand(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> ExecuteAsync(Log dto)
        {
            string sql = "";

            return await _dbConnection.ExecuteAsync(sql, dto);
        }
    }
}
