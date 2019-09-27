using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWasmApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INavigationInterception>(new DisabledNavigationInterception());
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("BlazorWasmApp");
        }

        public class DisabledNavigationInterception : INavigationInterception
        {
            public Task EnableNavigationInterceptionAsync() => Task.CompletedTask;
        }
    }
}