﻿using Wards.Application.UsesCases.Shared.Models;
using Wards.Application.UsesCases.Wards.Shared.Output;

namespace Wards.Application.UsesCases.Wards.ListarWard
{
    public interface IListarWardUseCase
    {
        Task<IEnumerable<WardOutput>?> Execute(PaginacaoInput input);
    }
}