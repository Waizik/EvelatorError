using System;
using System.IO;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin.StaticFiles.ContentTypes;
using Owin;

namespace EvelatorError
{
    public class WebHost
    {
        public void Run()
        {
            Console.WriteLine("Web server starting");
            try
            {
                // port pro web
                var baseUrl = "http://localhost:8089/";

                // spusteni
                using (
                    WebApp.Start<Startup>(new StartOptions(baseUrl) {ServerFactory = "Microsoft.Owin.Host.HttpListener"})
                    )
                {
                    Console.WriteLine("UnicamEmbeddedWeb is running on {0}", baseUrl);
                    Console.ReadKey();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Main exception. {0}", e);
            }
        }
    }


    public class Startup
    {
        private readonly HttpConfiguration config = new HttpConfiguration();

        public void Configuration(IAppBuilder app)
        {
            try
            {
                // vytvorit slozku pro web, pokud neexistuje
                if (!Directory.Exists(@"." + Path.DirectorySeparatorChar + "www"))
                    Directory.CreateDirectory(@"." + Path.DirectorySeparatorChar + "www");


                config.MapHttpAttributeRoutes();
                config.Formatters.Remove(config.Formatters.XmlFormatter);

                app = app.UseWebApi(config);

                var fileServerOptions = new FileServerOptions
                {
                    RequestPath = PathString.Empty,
                    // cesta ke slozce webu
                    FileSystem = new PhysicalFileSystem(@"." + Path.DirectorySeparatorChar + "www"),
                    EnableDefaultFiles = true,
                    EnableDirectoryBrowsing = false
                };

                var contentTypes =
                    (FileExtensionContentTypeProvider) fileServerOptions.StaticFileOptions.ContentTypeProvider;

                // odesilat obsah souboru .json jako json
                contentTypes.Mappings[".json"] = "application/json";

                // spustit server s nastavenim
                app = app.UseFileServer(fileServerOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception during startup. {0}", e);
            }
        }
    }
}