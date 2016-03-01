using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dengue.Startup))]
namespace Dengue
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
