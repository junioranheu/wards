﻿using Wards.Application.UsesCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UsesCases.Shared.Models;

namespace Wards.Application.UsesCases.Auxiliares.ListarEstado
{
    public interface IListarEstadoUseCase
    {
        Task<IEnumerable<EstadoOutput>> Execute(PaginacaoInput input);
    }
}