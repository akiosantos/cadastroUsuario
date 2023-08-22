using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Repository.Models
{
    public class TabPessoa
    {
        public int id { get; set; }

        public string nome { get; set; }

        public DateTime dataNascimento { get; set; }

        public decimal altura { get; set; }

        public decimal peso { get; set; }

        public decimal salario { get; set; }

        public decimal saldo { get; set; }

        public int idade { get; set; }

        public decimal imc { get; set; }

        public string classificacao { get; set; }

        public decimal inss { get; set; }

        public decimal aliquota { get; set; }

        public decimal salarioLiquido { get; set; }

        public decimal saldoDolar { get; set; }

        public int idUsuario { get; set; }

    }
}
