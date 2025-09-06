using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class RelatorioFiltroModel
    {
        [Display(Name = "Tipo de Relatório")]
        public string TipoRelatorio { get; set; } = string.Empty;

        [Display(Name = "Título do Relatório")]
        public string TituloRelatorio { get; set; } = string.Empty;

        // Filtros de Alunos
        [Display(Name = "Pesquisar por Nome")]
        public string? FiltroNome { get; set; }

        [Display(Name = "Sexo")]
        public string? FiltroSexo { get; set; }

        [Display(Name = "Cidade")]
        public int? FiltroCidadeId { get; set; }

        [Display(Name = "Data de Nascimento - De")]
        [DataType(DataType.Date)]
        public DateTime? FiltroDataNascimentoDe { get; set; }

        [Display(Name = "Data de Nascimento - Até")]
        [DataType(DataType.Date)]
        public DateTime? FiltroDataNascimentoAte { get; set; }

        [Display(Name = "Idade Mínima")]
        [Range(0, 120, ErrorMessage = "Idade deve estar entre 0 e 120 anos")]
        public int? FiltroIdadeMinima { get; set; }

        [Display(Name = "Idade Máxima")]
        [Range(0, 120, ErrorMessage = "Idade deve estar entre 0 e 120 anos")]
        public int? FiltroIdadeMaxima { get; set; }

        // Filtros de Cidades
        [Display(Name = "Pesquisar por Cidade")]
        public string? FiltroNomeCidade { get; set; }

        [Display(Name = "UF")]
        public string? FiltroUF { get; set; }

        [Display(Name = "Código IBGE")]
        public string? FiltroCodigoIBGE { get; set; }

        // Listas para dropdowns (não usar SelectList aqui)
        public List<Cidade>? ListaCidades { get; set; }
        public List<string>? ListaUFs { get; set; }

        // Configurações do PDF
        [Display(Name = "Incluir Cabeçalho")]
        public bool IncluirCabecalho { get; set; } = true;

        [Display(Name = "Incluir Rodapé")]
        public bool IncluirRodape { get; set; } = true;

        [Display(Name = "Incluir Numeração de Páginas")]
        public bool IncluirNumeroPagina { get; set; } = true;

        [Display(Name = "Orientação Paisagem")]
        public bool OrientacaoPaisagem { get; set; } = false;
    }

    public enum TipoRelatorioEnum
    {
        [Display(Name = "Relatório Geral de Alunos")]
        RelatorioGeralAlunos,

        [Display(Name = "Relatório de Alunos por Cidade")]
        RelatorioAlunosPorCidade,

        [Display(Name = "Relatório de Alunos por Faixa Etária")]
        RelatorioAlunosPorIdade,

        [Display(Name = "Lista de Cidades")]
        RelatorioCidades,

        [Display(Name = "Relatório de Cidades por UF")]
        RelatorioCidadesPorUF,

        [Display(Name = "Certificado Individual")]
        CertificadoIndividual
    }
}