﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TimeZoneConverter;
using Wards.Utils.Entities;

namespace Wards.Utils
{
    public static class Common
    {
        /// <summary>
        /// Pegar informações do appsettings;
        /// https://stackoverflow.com/a/58432834 (Necessário instalar o pacote "Microsoft.Extensions.Configuration.Json");
        /// </summary>
        static readonly string _emailDominio = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Domain"] ?? string.Empty;
        static readonly string _emailPorta = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Port"] ?? string.Empty;
        static readonly string _emailEmail = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Email"] ?? string.Empty;
        static readonly string _emailChave = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Key"] ?? string.Empty;
        static readonly string _emailRemetente = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("EmailSettings")["Name"] ?? string.Empty;

        static readonly string _urlFrontDev = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("URLSettings")["FrontDev"] ?? string.Empty;
        static readonly string _urlFrontProd = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("URLSettings")["FrontProd"] ?? string.Empty;

        /// <summary>
        /// Converter para o horário de Brasilia;
        /// https://blog.yowko.com/timezoneinfo-time-zone-id-not-found/;
        /// </summary>
        public static DateTime HorarioBrasilia()
        {
            TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo("E. South America Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
        }

        /// <summary>
        /// Gerar Lorem Ipsum;
        /// https://stackoverflow.com/questions/4286487/is-there-any-lorem-ipsum-generator-in-c;
        /// </summary>
        public static string LoremIpsum(int minWords, int maxWords, int minSentences, int maxSentences, int numParagraphs, bool isAdicionarTagP)
        {

            var words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer", "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod", "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat" };

            var rand = new Random();
            int numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            StringBuilder result = new();
            for (int p = 0; p < numParagraphs; p++)
            {
                if (isAdicionarTagP)
                {
                    result.Append("<p>");
                }

                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        if (w > 0) { result.Append(" "); }
                        result.Append(words[rand.Next(words.Length)]);
                    }

                    result.Append(". ");
                }

                if (isAdicionarTagP)
                {
                    result.Append("</p>");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Criptografar senha (Nuget BCrypt.Net-Next);
        /// </summary>
        public static string Criptografar(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        /// <summary>
        /// Verificar senha (Nuget BCrypt.Net-Next);
        /// </summary>
        public static bool VerificarCriptografia(string senha, string senhaCriptografada)
        {
            if (!BCrypt.Net.BCrypt.Verify(senha, senhaCriptografada))
                return false;

            return true;
        }

        /// <summary>
        /// Pegar a descrição de um enum;
        /// https://stackoverflow.com/questions/50433909/get-string-name-from-enum-in-c-sharp;
        /// </summary>
        public static string ObterDescricaoEnum(Enum enumVal)
        {
            MemberInfo[] memInfo = enumVal.GetType().GetMember(enumVal.ToString());
            DescriptionAttribute? attribute = CustomAttributeExtensions.GetCustomAttribute<DescriptionAttribute>(memInfo[0]);

            return attribute!.Description;
        }

        /// <summary>
        /// Formatar bytes para B, KB, MB, etc;
        /// </summary>
        public static string FormatarBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;

            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        /// <summary>
        /// Validar se o e-mail do usuário;
        /// </summary>
        public static bool ValidarEmail(string email)
        {
            EmailAddressAttribute e = new();
            return e.IsValid(email);
        }

        /// <summary>
        /// Validar se a senha do usuário é forte o suficiente verificando requisitos de senha:
        /// #1 - Tem número;
        /// #2 - Tem letra maiúscula;
        /// #3 - Tem pelo menos X caracteres;
        /// #4 - A senha não contém o nome completo, nome de usuário ou e-mail;
        /// </summary>
        public static Tuple<bool, string> ValidarSenha(string senha, string nomeCompleto, string nomeUsuario, string email)
        {
            bool isValido = true;
            string msgErro = string.Empty;

            var temNumero = new Regex(@"[0-9]+");
            if (!temNumero.IsMatch(senha))
            {
                isValido = false;
                msgErro = "A senha deve conter ao menos um número";
                return Tuple.Create(isValido, msgErro);
            }

            var temMaiusculo = new Regex(@"[A-Z]+");
            if (!temMaiusculo.IsMatch(senha))
            {
                isValido = false;
                msgErro = "A senha deve conter ao menos uma letra maiúscula";
                return Tuple.Create(isValido, msgErro);
            }

            int minCaracteres = 6;
            var temXCaracteres = new Regex(@".{" + minCaracteres + ",}");
            if (!temXCaracteres.IsMatch(senha))
            {
                isValido = false;
                msgErro = $"A senha deve conter ao menos {minCaracteres} caracteres";
                return Tuple.Create(isValido, msgErro);
            }

            string nomeCompletoPrimeiraParte = nomeCompleto.Split(' ')[0].ToLowerInvariant();
            bool isRepeteNomeCompleto = senha.ToLowerInvariant().Contains(nomeCompletoPrimeiraParte);
            if (isRepeteNomeCompleto)
            {
                isValido = false;
                msgErro = "A senha não pode conter o seu primeiro nome";
                return Tuple.Create(isValido, msgErro);
            }

            bool isRepeteNomeUsuario = senha.ToLowerInvariant().Contains(nomeUsuario.ToLowerInvariant());
            if (isRepeteNomeUsuario)
            {
                isValido = false;
                msgErro = "A senha não pode conter o seu nome de usuário";
                return Tuple.Create(isValido, msgErro);
            }

            string emailAntesDoArroba = email.Split('@')[0].ToLowerInvariant();
            bool isRepeteEmail = senha.ToLowerInvariant().Contains(emailAntesDoArroba.ToLowerInvariant());
            if (isRepeteEmail)
            {
                isValido = false;
                msgErro = "A senha não pode conter o seu e-mail";
                return Tuple.Create(isValido, msgErro);
            }

            return Tuple.Create(isValido, msgErro);
        }

        /// <summary>
        /// Gerar um número aleatório com base na em um valor mínimo e máximo;
        /// </summary>
        public static int GerarNumeroAleatorio(int min, int max)
        {
            Random random = new();
            int numeroAleatorio = random.Next(min, max - 1);

            return numeroAleatorio;
        }

        /// <summary>
        /// Gerar uma string aleatória com base na quantidade de caracteres desejados;
        /// </summary>
        public static string GerarStringAleatoria(int qtdCaracteres, bool isApenasMaiusculas)
        {
            Random random = new();
            string caracteres = (isApenasMaiusculas ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
            string? stringAleatoria = new(Enumerable.Repeat(caracteres, qtdCaracteres).Select(s => s[random.Next(s.Length)]).ToArray());

            return stringAleatoria;
        }

        /// <summary>
        /// Gerar um código hash para o usuário com base no usuarioId + string aleatória;
        /// </summary>
        public static string GerarHashUsuario(int usuarioId)
        {
            string palavraAleatoria = $"{usuarioId}_{GerarStringAleatoria(GerarNumeroAleatorio(10, 15), false)}";
            string hash = Criptografar(palavraAleatoria).Replace("/", string.Empty);

            return hash;
        }

        /// <summary>
        /// Converter IFormFile para bytes[];
        /// https://stackoverflow.com/questions/36432028/how-to-convert-a-file-into-byte-array-in-memory;
        /// </summary>
        public static async Task<byte[]> IFormFileParaBytes(IFormFile formFile)
        {
            await using var memoryStream = new MemoryStream();
            await formFile.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Converter Base64 para arquivo;
        /// </summary>
        public static IFormFile Base64ToFile(string base64)
        {
            List<IFormFile> formFiles = new();
            string split = ";base64,";
            string normalizarBase64 = base64;

            if (base64.Contains(split))
            {
                normalizarBase64 = base64.Substring(base64.IndexOf(split) + split.Length);
            }

            byte[] bytes = Convert.FromBase64String(normalizarBase64);
            MemoryStream stream = new(bytes);

            IFormFile file = new FormFile(stream, 0, bytes.Length, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            formFiles.Add(file);

            return formFiles[0];
        }

        /// <summary>
        /// Converter Base64 para imagem;
        /// </summary>
        public static IFormFile Base64ToImage(string base64)
        {
            List<IFormFile> formFiles = new();

            string split = ";base64,";
            string normalizarBase64 = base64.Substring(base64.IndexOf(split) + split.Length);
            byte[] bytes = Convert.FromBase64String(normalizarBase64);
            MemoryStream stream = new(bytes);

            IFormFile file = new FormFile(stream, 0, bytes.Length, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            formFiles.Add(file);

            return formFiles[0];
        }

        /// <summary>
        /// Converter caminho de um arquivo para imagem;
        /// </summary>
        public static IFormFile PathToFile(string path, string fileName, string contentType)
        {
            var fileStream = new FileStream(path, FileMode.Open);

            var formFile = new FormFile(fileStream, 0, fileStream.Length, string.Empty, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return formFile;
        }

        /// <summary>
        /// Verificar se a aplicação está sendo executada em localhost ou publicada;
        /// </summary>
        public static bool IsDebug()
        {
            // https://stackoverflow.com/questions/12135854/best-way-to-tell-if-in-production-or-development-environment-in-net
#if DEBUG
            return true;
#else
        return false;
#endif
        }

        /// <summary>
        /// Verificar se o front-end está sendo executado em localhost ou publicado;
        /// </summary>
        public static string CaminhoFront()
        {
            string urlApi = _urlFrontProd;

            if (IsDebug())
            {
                urlApi = _urlFrontDev;
            }

            return urlApi;
        }

        /// <summary>
        /// SMTP Gmail;
        /// https://www.youtube.com/watch?v=FZfneLNyE4o&ab_channel=AWPLife 
        /// </summary>
        public static async Task<bool> EnviarEmail(string emailTo, string assunto, string nomeArquivo, List<EmailDadosReplace> listaDadosReplace)
        {
            if (string.IsNullOrEmpty(emailTo) || string.IsNullOrEmpty(assunto) || string.IsNullOrEmpty(nomeArquivo))
            {
                return false;
            }

            string caminhoFinalArquivoHTML = $"{Directory.GetCurrentDirectory()}/Emails/{nomeArquivo}";
            string conteudoEmailHTML = AjustarConteudoEmailHTML(caminhoFinalArquivoHTML, listaDadosReplace);

            try
            {
                MailMessage mail = new()
                {
                    From = new MailAddress(_emailEmail, _emailRemetente)
                };

                mail.To.Add(new MailAddress(emailTo));
                // mail.CC.Add(new MailAddress(emailTo));

                mail.Subject = assunto;
                mail.Body = conteudoEmailHTML;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                // mail.Attachments.Add(new Attachment(arquivo));

                using SmtpClient smtp = new(_emailDominio, Convert.ToInt32(_emailPorta));
                smtp.Credentials = new NetworkCredential(_emailEmail, _emailChave);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ajustar HTML de um e-mail;
        /// </summary>
        private static string AjustarConteudoEmailHTML(string caminhoFinalArquivoHTML, List<EmailDadosReplace>? listaDadosReplace)
        {
            string conteudoEmailHtml = string.Empty;

            using (var reader = new StreamReader(caminhoFinalArquivoHTML))
            {
                // #1 - Ler arquivo;
                string readFile = reader.ReadToEnd();
                string strContent = readFile;

                // #2 - Remover tags desnecessárias;
                strContent = strContent.Replace("\r", string.Empty);
                strContent = strContent.Replace("\n", string.Empty);

                // #3 - Replaces utilizando o parâmetro "listaDadosReplace";[
                if (listaDadosReplace?.Count > 0)
                {
                    foreach (var item in listaDadosReplace)
                    {
                        strContent = strContent.Replace($"[{item.Key}]", item.Value);
                    }
                }

                // #4 - Gerar resultado final;
                conteudoEmailHtml = strContent.ToString();
            }

            return conteudoEmailHtml;
        }
    }
}