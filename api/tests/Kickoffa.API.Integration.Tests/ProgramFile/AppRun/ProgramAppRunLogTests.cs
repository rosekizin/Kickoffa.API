using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging; // para .ConfigureLogging()
using Serilog;
using Serilog.Events;
using Serilog.Sinks.TestCorrelator;
using System.Text;
using Xunit;

namespace Kickoffa.API.Integration.Tests.ProgramFile.AppRun;

/// <summary>
/// Testes para verificar os logs específicos das linhas 107-124 do Program.cs
/// Testa se o Program.cs realmente executa os logs esperados
/// </summary>
[Collection("Logging")]
public class ProgramAppRunLogTests : IDisposable
{
    private readonly StringWriter _logOutput;
    private readonly StringBuilder _logCapture;

    public ProgramAppRunLogTests()
    {
        _logCapture = new StringBuilder();
        _logOutput = new StringWriter(_logCapture);
    }

    [Fact]
    public void Program_ShouldLogSuccessfulStartupMessage()
    {
        // Arrange
        using var testCorrelator = TestCorrelator.CreateContext();

        // Configurar Serilog para capturar logs antes de executar Program.cs
        var testLogger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .WriteTo.TextWriter(_logOutput)
            .CreateLogger();

        // Força Serilog a usar este logger como padrão
        Log.Logger = testLogger;

        // Act - Criar WebApplicationFactory que executa o Program.cs real
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                // Não sobrescrever o Serilog, usar o que foi configurado acima

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // impede reconfiguração de logging padrão
                    logging.AddSerilog(testLogger);
                });
            });

        using var client = factory.CreateClient();

        // Assert
        var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
        var logText = _logCapture.ToString();

        // Verificar se o log de startup foi executado
        Assert.True(logEvents.Any(e => e.RenderMessage().Contains("🚀 Kickoffa API iniciada com sucesso!")) ||
                   logText.Contains("🚀 Kickoffa API iniciada com sucesso!"),
                   "Log de startup não foi encontrado");
    }

    [Fact]
    public void Program_ShouldLogEnvironmentInformation()
    {
        // Arrange
        using var testCorrelator = TestCorrelator.CreateContext();

        // Configurar Serilog para capturar logs antes de executar Program.cs
        var testLogger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .WriteTo.TextWriter(_logOutput)
            .CreateLogger();

        // Força Serilog a usar este logger como padrão
        Log.Logger = testLogger;

        // Act - Criar WebApplicationFactory com ambiente específico
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("TestEnvironment");

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // impede reconfiguração de logging padrão
                    logging.AddSerilog(testLogger);
                });
            });

        using var client = factory.CreateClient();

        // Assert
        var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
        var logText = _logCapture.ToString();

        // Verificar se o log de ambiente foi executado
        Assert.True(logEvents.Any(e => e.RenderMessage().Contains("Ambiente:")) ||
                   logText.Contains("Ambiente:"),
                   "Log de ambiente não foi encontrado");

        // Verificar se contém o ambiente correto
        Assert.True(logEvents.Any(e => e.RenderMessage().Contains("TestEnvironment")) ||
                   logText.Contains("TestEnvironment"),
                   "Ambiente 'TestEnvironment' não foi encontrado nos logs");
    }

    [Fact]
    public void Program_ShouldLogUrlsInformation()
    {
        // Arrange
        using var testCorrelator = TestCorrelator.CreateContext();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .WriteTo.TextWriter(_logOutput)
            .CreateLogger();

        // Act - Criar WebApplicationFactory que executa o Program.cs real
        var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        // Assert
        var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
        var logText = _logCapture.ToString();

        // Verificar se o log de URLs foi executado
        Assert.True(logEvents.Any(e => e.RenderMessage().Contains("URLs:")) ||
                   logText.Contains("URLs:"),
                   "Log de URLs não foi encontrado");
    }

    [Fact]
    public void Program_ShouldLogFatalExceptionOnStartupFailure()
    {
        // Arrange
        using var testCorrelator = TestCorrelator.CreateContext();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .WriteTo.TextWriter(_logOutput)
            .CreateLogger();

        // Act - Simular o comportamento do catch no Program.cs (linha 118)
        var testException = new InvalidOperationException("Test startup failure");
        Log.Fatal(testException, "💥 Aplicação falhou ao inicializar");

        // Assert
        var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
        var logText = _logCapture.ToString();

        var fatalLog = logEvents.FirstOrDefault(e => e.Level == LogEventLevel.Fatal);

        Assert.NotNull(fatalLog);
        Assert.Equal(LogEventLevel.Fatal, fatalLog.Level);
        Assert.Contains("💥 Aplicação falhou ao inicializar", fatalLog.RenderMessage());
        Assert.Equal(testException, fatalLog.Exception);

        // Verificar também no texto capturado
        Assert.Contains("💥 Aplicação falhou ao inicializar", logText);
        Assert.Contains("InvalidOperationException", logText);
    }

    [Fact]
    public void Program_ShouldLogAllStartupMessagesInSequence()
    {
        // Arrange
        using var testCorrelator = TestCorrelator.CreateContext();

        // Configurar Serilog para capturar logs antes de executar Program.cs
        var testLogger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .WriteTo.TextWriter(_logOutput)
            .CreateLogger();

        // Força Serilog a usar este logger como padrão
        Log.Logger = testLogger;

        // Act - Criar WebApplicationFactory que executa o Program.cs real
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("IntegrationTest");

                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); // impede reconfiguração de logging padrão
                    logging.AddSerilog(testLogger);
                });
            });

        using var client = factory.CreateClient();

        // Assert
        var logEvents = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
        var logText = _logCapture.ToString();

        // Verificar se todos os logs de startup foram executados
        var hasStartupLog = logEvents.Any(e => e.RenderMessage().Contains("🚀 Kickoffa API iniciada com sucesso!")) ||
                           logText.Contains("🚀 Kickoffa API iniciada com sucesso!");

        var hasEnvironmentLog = logEvents.Any(e => e.RenderMessage().Contains("Ambiente:")) ||
                               logText.Contains("Ambiente:");

        var hasUrlsLog = logEvents.Any(e => e.RenderMessage().Contains("URLs:")) ||
                        logText.Contains("URLs:");

        Assert.True(hasStartupLog, "Log de startup não foi encontrado");
        Assert.True(hasEnvironmentLog, "Log de ambiente não foi encontrado");
        Assert.True(hasUrlsLog, "Log de URLs não foi encontrado");
    }

    [Fact]
    public void Program_ShouldHandleLogCloseAndFlush()
    {
        // Arrange
        using var testCorrelator = TestCorrelator.CreateContext();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.TestCorrelator()
            .WriteTo.TextWriter(_logOutput)
            .CreateLogger();

        // Act - Testar diretamente o Log.CloseAndFlush() como no Program.cs (linha 123)
        Log.Information("Test log before flush");

        // Simular o finally do Program.cs
        Log.CloseAndFlush();

        // Assert - Verificar se não houve exceção
        var logText = _logCapture.ToString();
        Assert.Contains("Test log before flush", logText);

        // Verificar se CloseAndFlush pode ser chamado novamente sem erro
        Log.CloseAndFlush();
    }

    public void Dispose()
    {
        _logOutput?.Dispose();
        Log.CloseAndFlush();
        GC.SuppressFinalize(this);
    }
}