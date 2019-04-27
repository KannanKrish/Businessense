using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Businessense.Startup))]

namespace Businessense
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
