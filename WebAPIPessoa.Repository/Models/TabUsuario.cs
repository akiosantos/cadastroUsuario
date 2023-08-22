using System;
using System.Collections.Generic;
using System.Text;

namespace WebAPIPessoa.Repository.Models
{
    public class TabUsuario
    {
        public int id { get; set; } 

        public string nome { get; set; }

        public string usuario { get; set;}

        public string senha { get; set; }

        public string email { get; set; }
    }
}
