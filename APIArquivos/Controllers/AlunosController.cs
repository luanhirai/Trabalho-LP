
using APIArquivos.DTOs.Alunos;
using ArquivosLibrary.Entidades;
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
                        RA = aluno.RA
                    });
                }


                return Ok(alunosResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

    }
}
