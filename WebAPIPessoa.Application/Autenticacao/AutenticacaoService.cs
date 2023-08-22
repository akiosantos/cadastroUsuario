using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebAPIPessoa.Application.Cache;
using WebAPIPessoa.Application.Eventos;
using WebAPIPessoa.Application.Eventos.Models;
using WebAPIPessoa.Repository;
using WebAPIPessoa.Repository.Models;

namespace WebAPIPessoa.Application.Autenticacao
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly PessoaContext _context;
        private readonly IRabbitMQProducer _rabbitMqProducer;
        private readonly ICacheService _cacheService;
        private readonly ILogger<AutenticacaoService> _logger;


        public AutenticacaoService(PessoaContext context, IRabbitMQProducer rabbitMqProducer, ICacheService cacheService, ILogger<AutenticacaoService> logger)
        {
            _context = context;
            _rabbitMqProducer = rabbitMqProducer;
            _cacheService = cacheService;
            _logger = logger;
        }

        public bool EsqueciSenha(string email)
        {
            try
            {
                var usuarioExiste = _context.Usuarios.FirstOrDefault (x => x.email == email);
                if (usuarioExiste == null)
                {
                    return false;
                }

                var caminhoTemplate = $"{Environment.CurrentDirectory}.Application/Templates/EsqueciSenhaTemplate.html";
                var html = System.IO.File.ReadAllText(caminhoTemplate);
                var texto = html.Replace("@SENHA", usuarioExiste.senha);

                var esqueciSenhaModel = new EsqueciSenhaModel()
                {
                    Email = email,
                    Assunto = "Recuperação de senha",
                    Texto = texto
                };

                _rabbitMqProducer.EnviarMensagem(esqueciSenhaModel, "Var.Notificacao.Email", "Var.Notificacao", "Var.Notificacao");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public AutenticacaoResponse Autenticar(AutenticacaoRequest request)
        {
            _logger.LogInformation($"Buscando informação do cache. {request.Username}");
            var chave = $"{request.Username}{request.Password}";
            var usuarioCache = _cacheService.Get<AutenticacaoResponse>(chave);
            if (usuarioCache != null)
            {
                _logger.LogInformation($"Usuário encontrado no cache. {request.Username}");
                return usuarioCache;
            }

            _logger.LogInformation($"Usuário não encontrado no cachem buscando no banco. {request.Username}");

            var usuario = _context.Usuarios.FirstOrDefault(x => x.usuario == request.Username && x.senha == request.Password);
            if (usuario != null)
            {
                _logger.LogInformation($"Usuário encontrado no banco de dados. {request.Username}");
                var tokenString = GerarTokenJwt(usuario);

                var resposta = new AutenticacaoResponse()
                {
                    Token = tokenString,
                    UsuarioId = usuario.id
                };

                _logger.LogInformation($"Setando o usuário no cache. {request.Username}");
                _cacheService.Set(chave, resposta, 60);

                return resposta;
            }
            else
            {
                _logger.LogWarning($"Usuário não encontrado no banco de dados. {request.Username}");
                return null;
            }
        }
        private string GerarTokenJwt(TabUsuario usuario)
        {
            var issuer = "var";
            var audience = "var";
            var key = "4a698d73-0de4-4fc8-ae01-76521957e4f8";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("usuarioId", usuario.id.ToString())
            };

            var token = new JwtSecurityToken(issuer: issuer, claims: claims, audience: audience, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
    }
}
