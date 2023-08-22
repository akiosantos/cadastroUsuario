using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Pessoa
{
    public interface IPessoaService
    {
        bool RemoverPessoa(int id);

        PessoaHistoricoResponse ObterHistoricoPessoa(int id);

        List<PessoaHistoricoResponse> ObterHistoricoPessoas();

        PessoaResponse ProcessarInformacoesPessoa(PessoaRequest request, int usuarioId);
    }
}
