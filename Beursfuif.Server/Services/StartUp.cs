using System.Web.Cors;
using Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;

namespace Beursfuif.Server.Services
{
    public class Startup
    {
        public static IDependencyResolver DependencyResolver
        {
            get;
            set;
        }
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            FileServerOptions fileServerOptions = new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\\wwwroot")
            };

            //In order to serve json files
            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileServerOptions.StaticFileOptions.DefaultContentType = "text";


            // Remap '/' to '.\public\'.
            // Turns on static files and public files.
            app.UseFileServer(fileServerOptions);

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.Map("/signalr", map =>
            {
                // Setup the cors middleware to run before SignalR.
                // By default this will allow all origins. You can 
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);

                DependencyResolver = new DefaultDependencyResolver();

                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    //EnableJSONP = true,
                    EnableDetailedErrors = true,
                    Resolver = DependencyResolver
                };

                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch is already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }

       
    }
}
