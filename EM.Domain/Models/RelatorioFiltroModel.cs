using System.ComponentModel.DataAnnotations;

namespace EM.Domain.Models
{
    public class RelatorioFiltroModel
    {
        [Display(Name = "Tipo de Relat�rio")]
        public string TipoRelatorio { get; set; } = string.Empty;

        [Display(Name = "T�tulo do Relat�rio")]
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

        [Display(Name = "Data de Nascimento - At�")]
        [DataType(DataType.Date)]
        public DateTime? FiltroDataNascimentoAte { get; set; }

        [Display(Name = "Idade M�nima")]
        [Range(0, 120, ErrorMessage = "Idade deve estar entre 0 e 120 anos")]
        public int? FiltroIdadeMinima { get; set; }

        [Display(Name = "Idade M�xima")]
        [Range(0, 120, ErrorMessage = "Idade deve estar entre 0 e 120 anos")]
        public int? FiltroIdadeMaxima { get; set; }

        // Filtros de Cidades
        [Display(Name = "Pesquisar por Cidade")]
        public string? FiltroNomeCidade { get; set; }

        [Display(Name = "UF")]
        public string? FiltroUF { get; set; }

        [Display(Name = "C�digo IBGE")]
        public string? FiltroCodigoIBGE { get; set; }

        // Listas para dropdowns (n�o usar SelectList aqui)
        public List<Cidade>? ListaCidades { get; set; }
        public List<string>? ListaUFs { get; set; }

        // Configura��es do PDF
        [Display(Name = "Incluir Cabe�alho")]
        public bool IncluirCabecalho { get; set; } = true;

        [Display(Name = "Incluir Rodap�")]
        public bool IncluirRodape { get; set; } = true;

        [Display(Name = "Incluir Numera��o de P�ginas")]
        public bool IncluirNumeroPagina { get; set; } = true;

        [Display(Name = "Orienta��o Paisagem")]
        public bool OrientacaoPaisagem { get; set; } = false;
    }

    public enum TipoRelatorioEnum
    {
        [Display(Name = "Relat�rio Geral de Alunos")]
        RelatorioGeralAlunos,

        [Display(Name = "Relat�rio de Alunos por Cidade")]
        RelatorioAlunosPorCidade,

        [Display(Name = "Relat�rio de Alunos por Faixa Et�ria")]
        RelatorioAlunosPorIdade,

        [Display(Name = "Lista de Cidades")]
        RelatorioCidades,

        [Display(Name = "Relat�rio de Cidades por UF")]
        RelatorioCidadesPorUF,

        [Display(Name = "Certificado Individual")]
        CertificadoIndividual
    }
}