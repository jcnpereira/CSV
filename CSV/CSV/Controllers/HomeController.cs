using CSV.Data;
using CSV.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSV.Controllers {
   public class HomeController : Controller {
      private readonly ILogger<HomeController> _logger;
      private readonly IWebHostEnvironment _webHostEnvironment;
      private readonly ApplicationDbContext _db;

      public HomeController(
         ILogger<HomeController> logger,
         IWebHostEnvironment webHostEnvironment,
         ApplicationDbContext db) {
         _logger = logger;
         _webHostEnvironment = webHostEnvironment;
         _db = db;
      }

      public IActionResult Index() {
         return View();
      }

      /// <summary>
      /// Classe para ler e processar o ficheiro CSV.
      /// Não é feita uma validação do ficheiro. 
      /// É da responsabilidade do utilizador garantir a qualidade do ficheiro
      /// </summary>
      /// <param name="ficheiro">ficheiro com os dados CSV</param>
      /// <param name="separador">separador dos campos</param>
      /// <returns></returns>
      [HttpPost]
      public async Task<IActionResult> Index(IFormFile ficheiro, string separador) {

         if (ficheiro != null) {

            string extensao = Path.GetExtension(ficheiro.FileName).ToLower();

            if (extensao.ToLower().Contains("csv")) {

               // gravar ficheiro
               //***********************************************

               string tempo = DateTime.Now.ToString("yyyyMMddTHHmmss");
               string nomeFicheiro = tempo + "_Dias" + extensao;

               // onde guardar o ficheiro
               if (!System.IO.Directory.Exists(Path.Combine(_webHostEnvironment.ContentRootPath, "CSV")))
                  System.IO.Directory.CreateDirectory(Path.Combine(_webHostEnvironment.ContentRootPath, "CSV"));

               string caminhoCompleto = Path.Combine(_webHostEnvironment.ContentRootPath, "CSV", nomeFicheiro);

               using var stream = new FileStream(caminhoCompleto, FileMode.Create);
               await ficheiro.CopyToAsync(stream);
               stream.Dispose();

               try {
                  // processar o ficheiro
                  //***********************************************

                  // vars auxiliares
                  // separador dos campos do ficheiro CSV
                  char charSeparador = Convert.ToChar((string.IsNullOrEmpty(separador) ? ',' : separador));
                  // num de campos a processar no ficheiro
                  int numCampos = 3;

                  List<Dias> listaDias = new List<Dias>();
                  // Open the stream and read it back.
                  using (StreamReader sr = System.IO.File.OpenText(caminhoCompleto)) {
                     string s = "";
                     while ((s = sr.ReadLine()) != null) {
                        string[] dados = s.Split(charSeparador);
                        // processar os dados
                        // 0 - nome do dia da semana
                        // 1 - data
                        // 2 - nº de pessoas envolvidas

                        if (dados.Length == numCampos) {
                           Dias dia = new Dias {
                              DiaSemana = dados[0].Trim(),
                              Data = DateTime.Parse(dados[1].Trim()),
                              NumPessoas = Convert.ToInt32(dados[2].Trim())
                           };
                           // adicionar o dia
                           listaDias.Add(dia);
                        }
                     }
                  }
                  _db.Dias.AddRange(listaDias);
                  _db.SaveChanges();
                  // não esquecer! Mandar msg para o user
                  TempData["Mensagem"] = "Foram adicionados com sucesso os dias.";
                  return RedirectToAction("Dias", "Home");
               }
               catch (Exception) {
                  TempData["Mensagem"] = "Houve um erro na adição de Dias, via CSV.";
                  return View();
               }
            }
         }

         // não esquecer! Mandar msg para o user
         TempData["Mensagem"] = "Erro: É necessário que forneça um ficheiro CSV.";

         return View();
      }

      /// <summary>
      /// Mostra os dados existentes na classe Dias, obtidos através de um ficheiro CSV
      /// </summary>
      /// <returns></returns>
      public async Task<IActionResult> Dias() {

         var lista = await _db.Dias.OrderBy(d => d.Data).ToListAsync();

         return View(lista);
      }


      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error() {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
