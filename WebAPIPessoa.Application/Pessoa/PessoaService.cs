using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebAPIPessoa.Application.Cache;
using WebAPIPessoa.Repository;
using WebAPIPessoa.Repository.Models;

namespace WebAPIPessoa.Application.Pessoa
{
    public class PessoaService : IPessoaService
    {
        private readonly PessoaContext _context;
        private readonly ICacheService _cacheService;
        private Dictionary<string, decimal> _cacheCalculoImcMemoriaAplicacao = new Dictionary<string, decimal>();
        public PessoaService(PessoaContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public PessoaHistoricoResponse ObterHistoricoPessoa(int id)
        {
            var chave = $"pessoa_{id}";
            var cachePessoa = _cacheService.Get<PessoaHistoricoResponse>(chave);
            if (cachePessoa != null) 
                return cachePessoa;

            var pessoaDb = _context.Pessoas.FirstOrDefault(x => x.id == id);
            if (pessoaDb == null)
                return null;

            var pessoa = new PessoaHistoricoResponse()
            {
                Aliquota = Convert.ToDouble(pessoaDb.aliquota),
                Altura = pessoaDb.altura,
                Classificacao = pessoaDb.classificacao,
                DataNascimento = pessoaDb.dataNascimento,
                Id = pessoaDb.id,
                Idade = pessoaDb.idade,
                IdUsuario = pessoaDb.idUsuario,
                Imc = pessoaDb.imc,
                Inss = Convert.ToDouble(pessoaDb.inss),
                Nome = pessoaDb.nome,
                Peso = pessoaDb.peso,
                Salario = Convert.ToDouble(pessoaDb.salario),
                SalarioLiquido = Convert.ToDouble(pessoaDb.salarioLiquido),
                Saldo = pessoaDb.saldo,
                SaldoDolar = pessoaDb.saldoDolar,
            };

            _cacheService.Set(chave, pessoa, 60);

            return pessoa;
        }

        public List<PessoaHistoricoResponse> ObterHistoricoPessoas()
        {
            var pessoasDb = _context.Pessoas.ToList();
            var pessoas = new List<PessoaHistoricoResponse>();

            foreach (var item in pessoasDb)
            {
                pessoas.Add(new PessoaHistoricoResponse()
                {
                    Aliquota = Convert.ToDouble(item.aliquota),
                    Altura = item.altura,
                    Classificacao = item.classificacao,
                    DataNascimento = item.dataNascimento,
                    Id = item.id,
                    Idade = item.idade,
                    IdUsuario = item.idUsuario,
                    Imc = item.imc,
                    Inss = Convert.ToDouble(item.inss),
                    Nome = item.nome,
                    Peso = item.peso,
                    Salario = Convert.ToDouble(item.salario),
                    SalarioLiquido = Convert.ToDouble(item.salarioLiquido),
                    Saldo = item.saldo,
                    SaldoDolar = item.saldoDolar,

                });
            }

            return pessoas;

        }
        public bool RemoverPessoa(int id)
        {
            try
            {
                var pessoaDb = _context.Pessoas.FirstOrDefault(x => x.id == id);
                if (pessoaDb == null)
                    return false;

                _context.Pessoas.Remove(pessoaDb);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

   

        public PessoaResponse ProcessarInformacoesPessoa(PessoaRequest request, int usuarioId)
        {
            var idade = CalcularIdade(request.DataNascimento);
            var imc = CalcularImc(request.Peso, request.Altura);
            var classificacao = CalcularClassificacao(imc);
            var aliquota = CalcularAliquota(request.Salario);
            var inss = CalcularInss(request.Salario, aliquota);
            var salarioLiquido = CalcularSalarioLiquido(request.Salario, inss);
            var saldoDolar = CalcularDolar(request.Saldo);


            var resposta = new PessoaResponse();
            resposta.SaldoDolar = saldoDolar;
            resposta.Aliquota = aliquota;
            resposta.SalarioLiquido = salarioLiquido;
            resposta.Classificacao = classificacao;
            resposta.Idade = idade;
            resposta.Imc = imc;
            resposta.Inss = inss;
            resposta.Nome = request.Nome;

            var pessoa = new TabPessoa()
            {
                aliquota = Convert.ToDecimal(aliquota),
                altura = request.Altura,
                classificacao = classificacao,
                dataNascimento = request.DataNascimento,
                idade = idade,
                idUsuario = usuarioId,
                imc = imc,
                inss = Convert.ToDecimal(inss),
                nome = request.Nome,
                peso = request.Peso,
                salario = Convert.ToDecimal(request.Salario),
                salarioLiquido = Convert.ToDecimal(salarioLiquido),
                saldo = request.Saldo,
                saldoDolar = saldoDolar,
            };

            _context.Pessoas.Add(pessoa);
            _context.SaveChanges();

            return resposta;
        }

        private int CalcularIdade(DateTime dataNascimento)
        {
            var anoAtual = DateTime.Now.Year;
            var idade = anoAtual - dataNascimento.Year;

            var mesAtual = DateTime.Now.Month;
            if (mesAtual < dataNascimento.Month)
            {
                idade = idade - 1;
            }

            return idade;
        }

        private decimal CalcularImc(decimal peso, decimal altura)
        {
            var chave = $"{peso} {altura}";
            if (_cacheCalculoImcMemoriaAplicacao.Any(x => x.Key == chave))
                return _cacheCalculoImcMemoriaAplicacao.FirstOrDefault(x => x.Key == chave).Value;
        
            var imc = Math.Round(peso / (altura * altura), 2);
            _cacheCalculoImcMemoriaAplicacao.Add(chave, imc);

            return imc;
        }

        private string CalcularClassificacao(decimal imc)
        {
            var classificacao = "";

            if (imc < (decimal)18.5)
            {
                classificacao = "Abaixo do peso ideal";
            }
            else if (imc >= (decimal)18.5 && imc <= (decimal)24.99)
            {
                classificacao = "Peso Normal";
            }
            else if (imc >= (decimal)25 && imc <= (decimal)29.99)
            {
                classificacao = "Pré-Obesidade";
            }
            else if (imc >= (decimal)30 && imc <= (decimal)34.99)
            {
                classificacao = "Obesidade Grau I";
            }
            else if (imc >= (decimal)35 && imc <= (decimal)39.99)
            {
                classificacao = "Obesidade Grau II";
            }
            else
            {
                classificacao = "Obesidade Grau III";
            }

            return classificacao;
        }

        private double CalcularAliquota(double salario)
        {
            var aliquota = 7.5;
            if (salario <= 1212)
            {
                aliquota = 7.5;
            }
            else if (salario >= 1212.01 && salario <= 2427.35)
            {
                aliquota = 9;
            }
            else if (salario >= 2427.36 && salario <= 3641.03)
            {
                aliquota = 12;
            }
            else
            {
                aliquota = 14;
            }

            return aliquota;
        }

        private double CalcularInss(double salario, double aliquota)
        {
            var inss = (salario * aliquota) / 100;

            return inss;
        }

        private double CalcularSalarioLiquido(double salario, double inss)
        {
            return salario - inss;
        }

        private decimal CalcularDolar(decimal saldo)
        {
            var dolar = (decimal)5.14;
            var saldoDolar = Math.Round(saldo / dolar, 2);

            return saldoDolar;
        }
    }
 }

