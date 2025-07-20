using Microsoft.Extensions.Hosting;

namespace Kickoffa.API.AspNet.Infrastructure.Wrappers
{
    public interface IHostEnvironmentWrapper
    {
        bool IsDevelopment();
    }

    public class HostEnvironmentWrapper : IHostEnvironmentWrapper
    {
        private readonly IHostEnvironment _env;

        public HostEnvironmentWrapper(IHostEnvironment env)
        {
            _env = env;
        }

        public bool IsDevelopment() => _env.IsDevelopment();
    }
}