using Beursfuif.TypedClient;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Beursfuif.TypedClient
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            FileServerOptions fileServerOptions = new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(@".\"),
            };

            //In order to serve json files
            fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fileServerOptions.StaticFileOptions.DefaultContentType = "text";


            // Remap '/' to '.\public\'.
            // Turns on static files and public files.
            app.UseFileServer(fileServerOptions);



            app.UseStageMarker(PipelineStage.MapHandler);
        }
    }
}
