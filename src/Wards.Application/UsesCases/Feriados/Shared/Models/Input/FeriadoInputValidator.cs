﻿using FluentValidation;

namespace Wards.Application.UsesCases.Feriados.Shared.Models.Input
{
    public sealed class FeriadoInputValidator : AbstractValidator<FeriadoInput>
    {
        public FeriadoInputValidator()
        {
            RuleFor(f => f.Tipo).NotNull().NotEmpty();
            RuleFor(f => f.Nome).NotNull().NotEmpty();

            RuleFor(f => f.Data).NotNull().NotEmpty();
            RuleFor(f => f.DistribuidoraId).NotNull().NotEmpty();
            RuleFor(f => f.EstadoId).NotNull().NotEmpty();
        }
    }
}