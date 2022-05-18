using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSV.Models {

   /// <summary>
   /// Classe para receber os dados vindos do ficheiro CSV
   /// </summary>
   public class Dias {

      [Key]
      public int Id { get; set; }

      /// <summary>
      /// Identificação do dia da semana
      /// </summary>
      [Display(Name = "Dia da Semana")]
      public string DiaSemana { get; set; }

      /// <summary>
      /// data
      /// </summary>
      [Display(Name = "Data")]
      [DataType(DataType.Date)]
      [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
      public DateTime Data { get; set; }

      /// <summary>
      /// Número de pessoas envolvidas
      /// </summary>
      [Display(Name = "Nº de pessoas")]
      public int NumPessoas { get; set; }

   }
}
