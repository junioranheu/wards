﻿using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public interface IObterUsuarioUseCase
    {
        Task<UsuarioDTO?> Obter(int id = 0, string email = "");
        Task<Usuario> ObterByEmailOuUsuarioSistema(string? email, string? nomeUsuarioSistema);
    }
}