
using Owin;

namespace vercom
{
    public class Startup
    {
        public void Configuration(Owin.IAppBuilder app)
        {
            app.MapSignalR(); // Configura SignalR       
        }
    }
}
