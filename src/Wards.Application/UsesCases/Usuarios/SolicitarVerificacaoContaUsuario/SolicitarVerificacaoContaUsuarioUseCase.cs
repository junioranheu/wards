﻿using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Application.UsesCases.Usuarios.SolicitarVerificacaoContaUsuario.Commands;
using Wards.Domain.Enums;
using Wards.Utils.Entities;
using static Wards.Utils.Common;

namespace Wards.Application.UsesCases.Usuarios.SolicitarVerificacaoContaUsuario
{
    public sealed class SolicitarVerificacaoContaUsuarioUseCase : ISolicitarVerificacaoContaUsuarioUseCase
    {
        private readonly ISolicitarVerificacaoContaUsuarioCommand _solicitarVerificacaoContaUsuarioCommand;

        public SolicitarVerificacaoContaUsuarioUseCase(ISolicitarVerificacaoContaUsuarioCommand solicitarVerificacaoContaUsuarioCommand)
        {
            _solicitarVerificacaoContaUsuarioCommand = solicitarVerificacaoContaUsuarioCommand;
        }

        public async Task<UsuarioOutput?> Execute(int usuarioId)
        {
            (string, string, string, string) resp = await _solicitarVerificacaoContaUsuarioCommand.Execute(usuarioId);
            string erros = resp.Item1;

            if (!string.IsNullOrEmpty(erros))
                return (new UsuarioOutput() { Messages = new string[] { erros } });

            string email = resp.Item2;
            string nomeCompleto = resp.Item3;
            string codigoVerificacao = resp.Item4;

            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(nomeCompleto) && !string.IsNullOrEmpty(codigoVerificacao))
                {
                    await EnviarEmailVerificacaoConta(email, nomeCompleto, codigoVerificacao);
                }
            }
            catch (Exception)
            {
                return (new UsuarioOutput() { Messages = new string[] { GetDescricaoEnum(CodigosErrosEnum.ContaNaoVerificadaComFalhaNoEnvioNovoEmailVerificacao) } });
            }

            return new UsuarioOutput();
        }

        private static async Task<bool> EnviarEmailVerificacaoConta(string emailTo, string nomeUsuario, string codigoVerificacao)
        {
            const string assunto = "Reenvio de e-mail de verificação de conta";
            string nomeArquivo = GetDescricaoEnum(EmailEnum.VerificarConta);

            List<EmailDadosReplace> listaDadosReplace = new()
                {
                    new EmailDadosReplace { Key = "Url", Value = $"{CaminhoFront()}/usuario/verificar-conta/{codigoVerificacao}"},
                    new EmailDadosReplace { Key = "NomeUsuario", Value = nomeUsuario }
                };

            bool resposta = await EnviarEmail(emailTo, assunto, nomeArquivo, listaDadosReplace);
            return resposta;
        }
    }
}