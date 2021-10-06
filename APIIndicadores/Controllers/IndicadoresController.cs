using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using APIIndicadores.Data;
using APIIndicadores.Models;

namespace APIIndicadores.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndicadoresController : ControllerBase
    {
        private static object syncObject = Guid.NewGuid();
        private readonly ILogger<IndicadoresController> _logger;
        private readonly IndicadoresContext _context;
        private readonly IDistributedCache _cache;
        private readonly TelemetryConfiguration _telemetryConfig;

        public IndicadoresController(
            ILogger<IndicadoresController> logger,
            IndicadoresContext context,
            IDistributedCache cache,
            TelemetryConfiguration telemetryConfig)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
            _telemetryConfig = telemetryConfig;
        }

        [HttpGet]
        public IEnumerable<Indicador> Get()
        {
            Indicador[] indicadoresEconomicos = null;
            DateTimeOffset inicio = DateTime.Now;
            Stopwatch watch = new Stopwatch();
            watch.Start();

            string valorJSON = _cache.GetString("Indicadores");
            if (valorJSON == null)
            {
                lock (syncObject)
                {
                    valorJSON = _cache.GetString("Indicadores");
                    if (valorJSON == null)
                    {
                        indicadoresEconomicos =
                            (from i in _context.Indicadores
                             select i).ToArray();

                        var opcoesCache = new DistributedCacheEntryOptions();
                        opcoesCache.SetAbsoluteExpiration(
                            TimeSpan.FromSeconds(30));

                        valorJSON = JsonSerializer.Serialize(indicadoresEconomicos);
                        _cache.SetString("Indicadores", valorJSON, opcoesCache);
                        _logger.LogInformation(
                            "Indicadores Economicos gravados no Cache Redis...");
                    }
                }
            }
            watch.Stop();

            var client = new TelemetryClient(_telemetryConfig);
            client.TrackDependency(
                "Redis", "Get", valorJSON, inicio, watch.Elapsed, true);

            if (indicadoresEconomicos is null && valorJSON is not null)
            {
                indicadoresEconomicos = JsonSerializer.Deserialize<Indicador[]>(valorJSON);
                _logger.LogInformation(
                    "Carregados Indicadores Economicos do Cache Redis...");
            }

            _logger.LogInformation(
                $"No. de Indicadores Economicos encontrados = {indicadoresEconomicos.Length}");

            return indicadoresEconomicos;
        }
    }
}