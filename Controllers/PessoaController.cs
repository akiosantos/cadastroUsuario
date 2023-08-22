using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPIPessoa.Application.Cache;
using WebAPIPessoa.Application.Pessoa;
using WebAPIPessoa.Application.Usuario;
using WebAPIPessoa.Repository;

namespace WebAPIPessoa.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PessoaController : ControllerBase
    {
        private readonly PessoaContext _context;
        private readonly ICacheService _cacheService;
        private readonly IPessoaService _pessoaService;

        public PessoaController(PessoaContext context, ICacheService cacheService, IPessoaService pessoaService) 
        {
            _context = context;
            _cacheService = cacheService;
            _pessoaService = pessoaService;
        }

        /// <summary>
        /// Rota responsável por realizar o processamento de dados de uma pessoa - Calcula idade, calcula imc, calcula inss, realiza conversão de saldo para dolar. 
        /// </summary>
        /// <returns>Retorna os dados processados da pessoa</returns>
        /// <response code="200">Retorna os dados processados com sucesso</response>
        /// <response code="400">Erro de validação</response>

        [HttpPost]
        [Authorize]
        public PessoaResponse ProcessarInformacoesPessoa([FromBody] PessoaRequest request)
        {
            var identidade = (ClaimsIdentity)HttpContext.User.Identity;
            var usuarioId = identidade.FindFirst("usuarioId").Value;

            var pessoaResponse = _pessoaService.ProcessarInformacoesPessoa(request, Convert.ToInt32(usuarioId));

            return pessoaResponse;
        }

        [HttpGet]
        [Authorize]
        public List<PessoaHistoricoResponse> ObterHistoricoPessoas()
        {
            var pessoas = _pessoaService.ObterHistoricoPessoas();

            return pessoas;
        }

        [HttpGet]
        [Authorize]
        [Route("{id}")]
        public PessoaHistoricoResponse ObterHistoricoPessoa([FromRoute] int id)
        {
            var pessoa = _pessoaService.ObterHistoricoPessoa(id);

            return pessoa;
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoverPessoa([FromRoute] int id)
        {
            var sucesso = _pessoaService.RemoverPessoa(id);

            if (sucesso == true)
            {
                return NoContent();
            }
            else
            {
                return BadRequest();
            }

        }
    }
}
