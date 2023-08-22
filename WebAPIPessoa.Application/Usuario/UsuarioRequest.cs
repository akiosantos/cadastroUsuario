using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Usuario
{
    public class UsuarioRequest
    {
        public string Nome { get; set; }

        public string Usuario { get; set; }

        public string Senha { get; set; }

        public string Email { get; set; }

    }
}
