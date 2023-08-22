using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Autenticacao
{
    public interface IAutenticacaoService
    {
        bool EsqueciSenha(string email);

        AutenticacaoResponse Autenticar(AutenticacaoRequest request);
    }
}
