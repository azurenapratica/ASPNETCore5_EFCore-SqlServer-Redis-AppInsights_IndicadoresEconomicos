using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIIndicadores.Models
{
    public class BolsaValores
    {
        public string Sigla { get; set; }
        public string NomeBolsa { get; set; }
        public DateTime DataReferencia { get; set; }
        [Column(TypeName = "numeric(10,4)")]
        public decimal Variacao { get; set; }
       
    }
}