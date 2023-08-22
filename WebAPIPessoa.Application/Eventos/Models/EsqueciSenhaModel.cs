using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Application.Eventos.Models
{
    public class EsqueciSenhaModel
    {
        public string Email { get; set; }

        public string Assunto { get; set; }

        public string Texto { get; set; }

    }
}
