using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using APIIndicadores.Data;
using APIIndicadores.Models;

namespace APIIndicadores.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BolsasController : ControllerBase
    {
        private readonly ILogger<BolsasController> _logger;
        private readonly IndicadoresContext _context;

        public BolsasController(
            ILogger<BolsasController> logger,
            IndicadoresContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<BolsaValores> Get()
        {
            var indicadoresBolsasValores =
                (from b in _context.BolsasValores
                 select b).ToArray();
            _logger.LogInformation(
                $"No. de Indicadores de Bolsas de Valores encontrados = {indicadoresBolsasValores.Length}");
            return indicadoresBolsasValores;
        }
    }
}