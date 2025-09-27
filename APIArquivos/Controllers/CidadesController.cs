using APIArquivos.DTOs.Alunos;
using APIArquivos.DTOs.Cidades;
using ArquivosLibrary.Entidades;
using ArquivosLibrary.Repository;
using ArquivosLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace APIArquivos.Controllers
{
    [Route("api/cidades")]
    [ApiController]
    public class CidadesController : ControllerBase
    {

        private readonly CidadesService _cidadesService;

        public CidadesController(CidadesService cidadesService)
        {
            _cidadesService = cidadesService;
        }

        [HttpPost("importar")]
        public IActionResult DataImport(IFormFile file)
        {
            List<Cidade> cidades = new List<Cidade>();

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var linha = reader.ReadLine();
                        var dados = linha.Split(',');

                        if (dados.Length == 6)
                        {
                            int cidadeId = int.Parse(dados[0]);
                            string nome = dados[1];
                            string sigla = dados[2];
                            int ibgeMunicipio = int.Parse(dados[3]);
                            double latitude = double.Parse(dados[4], CultureInfo.InvariantCulture);
                            double longitude = double.Parse(dados[5], CultureInfo.InvariantCulture);

                            Cidade cidade = new Cidade(cidadeId, nome, sigla, ibgeMunicipio, latitude, longitude);
                            cidades.Add(cidade);
                        }
                    }
                }

                foreach (Cidade cidade in cidades)
                {
                    _cidadesService.add(cidade);
                }

                return Ok("Cidades importadas com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex}");
                return StatusCode(500, "Erro ao importar cidades.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCity()
        {
            try
            {
                var cidades = await _cidadesService.GetAllCity();

                List<CidadeObterResponse> cidadesResponse = new List<CidadeObterResponse>();

                foreach (var cidade in cidades)
                {
                    cidadesResponse.Add(new CidadeObterResponse
                    {
                        CidadeId = cidade.CidadeId,
                        Nome = cidade.Nome,
                        Sigla = cidade.Sigla,
                        IBGEMunicipio = cidade.IBGEMunicipio,
                        Latitude = cidade.Latitude,
                        Longitude = cidade.Longitude
                    });
                }

                return Ok(cidadesResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCityById(int id)
        {
            try
            {
                var cidade = await _cidadesService.GetCityById(id);

                if (cidade != null)
                {
                    CidadeObterResponse cidadeResponse = new()
                    {
                        CidadeId = cidade.CidadeId,
                        Nome = cidade.Nome,
                        Sigla = cidade.Sigla,
                        IBGEMunicipio = cidade.IBGEMunicipio,
                        Latitude = cidade.Latitude,
                        Longitude = cidade.Longitude
                    };

                    return Ok(cidadeResponse);
                }

                return NotFound("Cidade não encontrada.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }



        [HttpGet("estados")]
        public async Task<IActionResult> GetEstates() 
        {
            var cidades = await _cidadesService.GetAllCity();

            List<EstadoResponse> cidadesResponse = new List<EstadoResponse>();

            foreach (var cidade in cidades)
            {
                cidadesResponse.Add(new EstadoResponse
                {
                    Sigla = cidade.Sigla,
                });
            }

            return Ok(cidadesResponse);
        }


        [HttpGet("estado/{uf}")]
        public async Task<IActionResult> GetCitysByEstate(string uf)
        {
            try
            {
                var cidades = await _cidadesService.GetAllCityByUF(uf);

                List<CidadeObterResponse> cidadesResponse = new List<CidadeObterResponse>();

                foreach (var cidade in cidades)
                {
                    if (cidade.Sigla != uf)
                    {
                        cidadesResponse.Add(new CidadeObterResponse
                        {
                            CidadeId = cidade.CidadeId,
                            Nome = cidade.Nome,
                            Sigla = cidade.Sigla,
                            IBGEMunicipio = cidade.IBGEMunicipio,
                            Latitude = cidade.Latitude,
                            Longitude = cidade.Longitude
                        });
                    } 
                }

                return Ok(cidadesResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}
