﻿using Wards.Domain.Entities;

namespace Wards.Application.UseCases.Feriados.CriarFeriado.Commands
{
    public interface ICriarFeriadoCommand
    {
        Task<int> ExecuteAsync(Feriado input);
    }
}
