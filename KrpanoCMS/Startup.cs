using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KrpanoCMS.Startup))]
namespace KrpanoCMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
