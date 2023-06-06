using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Passion_Project.Startup))]
namespace Passion_Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
