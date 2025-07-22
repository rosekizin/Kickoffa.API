using Microsoft.AspNetCore.Mvc;

namespace Kickoffa.API.Controllers
{
    /// <summary>
    /// Controller apenas para teste de integração. Não será acessível por fora.
    /// O intuito é retornar uma exceção para testar se o GlobalExceptionHandler foi corretamente
    /// configurado no program.cs
    /// </summary>
    [ApiController]
    [Route("crash")]
    [ApiExplorerSettings(IgnoreApi = true)] // oculta do Swagger
    public class CrashController : ControllerBase
    {
        private readonly IHostEnvironment _env;

        public CrashController(IHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("fail")]
        public IActionResult Fail()
        {
            if (!_env.IsEnvironment("Test"))
                return NotFound();

            throw new Exception("Forçando exceção para testes");
        }
    }
}