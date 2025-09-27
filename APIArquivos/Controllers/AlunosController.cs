
using APIArquivos.DTOs.Alunos;
using ArquivosLibrary.Entidades;
using ArquivosLibrary.Repository;
using ArquivosLibrary.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIArquivos.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public class AlunosController : ControllerBase
    {

        private readonly AlunosService _alunosService;

        public AlunosController(AlunosService alunosService)
        {
            _alunosService = alunosService;
        }


        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AdicionarAluno([FromBody] DTOs.Alunos.AlunoCriarRequest request)
        {
            try
            {
                Aluno aluno = new Aluno
                {
                    Nome = request.Nome
                };
                var result = await _alunosService.AdicionarAlunoAsync(aluno);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExcluirAluno(int id)
        {
            try
            {
                var result = await _alunosService.ExcluirAlunoAsync(id);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterAlunoPorId(int id)
        {
            try
            {
                var aluno = await _alunosService.ObterAlunoPorIdAsync(id);
                if (aluno == null)
                    return NotFound("Aluno não encontrado.");

                AlunoObterResponse alunoResponse = new()
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    RA = aluno.RA
                };

                return Ok(alunoResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodosAlunos()
        {
            try
            {
                var alunos = await _alunosService.ObterTodosAlunosAsync();


                List<AlunoObterResponse> alunosResponse = new List<AlunoObterResponse>();

                foreach (var aluno in alunos)
                {
                    alunosResponse.Add(new AlunoObterResponse
                    {
                        Id = aluno.Id,
                        Nome = aluno.Nome,
                        RA = aluno.RA,
                        ImagemUrl= aluno.ImagemUrl
                    });
                }


                return Ok(alunosResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }


        [HttpPost("{id}/foto")]
        public async Task<IActionResult> UploadFoto(int id, IFormFile foto)
        {
            if (foto == null || foto.Length == 0)
                return BadRequest("Arquivo inválido.");

            var storage = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
            if (!Directory.Exists(storage))
                Directory.CreateDirectory(storage);

            var caminhoArquivo = Path.Combine(storage, $"{id}{Path.GetExtension(foto.FileName)}");

            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await foto.CopyToAsync(stream);
            }

            var aluno = await _alunosService.ObterAlunoPorIdAsync(id);
            if (aluno == null)
                return NotFound("Aluno não encontrado.");


            var result = await _alunosService.AtualizarFotoAsync(id, caminhoArquivo);

            if (!result)
                return StatusCode(500, "Erro ao salvar o caminho da foto no banco.");

            return Ok("Foto enviada com sucesso.");
        }




        [HttpGet("{id}/foto")]
        public async Task<IActionResult> GetFotoBase64(int id)
        {
            var aluno = await _alunosService.ObterAlunoPorIdAsync(id);
            if (aluno == null || string.IsNullOrEmpty(aluno.ImagemUrl))
                return NotFound("Foto não encontrada.");

            if (!System.IO.File.Exists(aluno.ImagemUrl))
                return NotFound("Arquivo da foto não encontrado.");

            byte[] bytes = System.IO.File.ReadAllBytes(aluno.ImagemUrl);
            string base64 = Convert.ToBase64String(bytes);

            return Ok(base64);
        }


    }
}
