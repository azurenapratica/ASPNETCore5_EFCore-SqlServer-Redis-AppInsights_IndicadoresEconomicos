using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIIndicadores.Models
{
    public class Indicador
    {
        public string Sigla { get; set; }
        public string NomeIndicador { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal Valor { get; set; }
        
    }
}