using SharePoint.FileServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.FtpClient;
using System.Web;
using System.Web.Mvc;

namespace SharePoint.FileServiceWeb.Controllers
{
    public class FilesWebPartController : Controller
    {
        private string ftpUrl = System.Configuration.ConfigurationManager.AppSettings.Get("URL");
        private string ftpUser = System.Configuration.ConfigurationManager.AppSettings.Get("USER");
        private string ftpPassword = System.Configuration.ConfigurationManager.AppSettings.Get("PASSWORD");

        [SharePointContextFilter]
        public ActionResult Index(string id)
        {
            id = id ?? "/";
            var instance = new FtpClient();
            instance.Host = ftpUrl;
            instance.Credentials = new System.Net.NetworkCredential(ftpUser, ftpPassword);
            instance.ValidateCertificate += (c, e) => { e.Accept = true; };


            if (id.EndsWith("/"))
            {
                var vm = new FilesViewModel { CurrentFolder = id };
                vm.Items = instance.GetListing(id, FtpListOption.ForceList).OrderByDescending(x => x.Type).ThenBy(x => x.Name).ToList();
                if (id != "/") vm.Items.Insert(0, new FtpListItem { FullName = GetParent(id), Name = "..", Type = FtpFileSystemObjectType.Directory });

                return View(vm);
            }
            else
            {
                using (Stream istream = instance.OpenRead(id))
                {
                    Response.ContentType = "application/octet-stream";
                    istream.CopyTo(Response.OutputStream);
                    return new EmptyResult();
                }
            }
        }

        private string GetParent(string path)
        {
            if (path == "/") return null;
            if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
            var items = path.Split('/');
            return path.Substring(0, path.Length - items[items.Length - 1].Length);
        }
    }
}
