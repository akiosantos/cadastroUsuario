using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebAPIPessoa.Application.Autenticacao;
using WebAPIPessoa.Application.Eventos;
using WebAPIPessoa.Repository;

namespace WebAPIPessoa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        private readonly IAutenticacaoService _autenticacaoService;
        private readonly ILogger<AutenticacaoController> _logger;    

        public AutenticacaoController(IAutenticacaoService autenticacaoService, ILogger<AutenticacaoController> logger)
        {
            _autenticacaoService = autenticacaoService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult Login([FromBody] AutenticacaoRequest request)
        {
            try
            {
                _logger.LogInformation($"Iniciando autenticação {request.Username}");
                var resposta = _autenticacaoService.Autenticar(request);

                if (resposta == null)
                {
                    _logger.LogWarning($"Não foi possível realizar a autenticação {request.Username}");
                    return Unauthorized();
                }
                else
                {
                    _logger.LogInformation($"Autenticação realizada com sucesso; {request.Username}");
                    return Ok(resposta);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}{ex.InnerException?.Message}");
                return BadRequest("Não foi possível realizar a autenticação");
            }
        }

        [HttpPost]
        [Route("esqueciSenha")]
        public IActionResult EsqueciSenha([FromBody] EsqueciSenhaRequest request )
        {
           var resposta =  _autenticacaoService.EsqueciSenha(request.Email);
           if (resposta)
               return NoContent();
           else
               return BadRequest();
        }
    }
}
