using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharePoint.FileServiceWeb.Models
{
    public class FilesViewModel
    {
        public string CurrentFolder { get; set; }
        public List<System.Net.FtpClient.FtpListItem> Items { get; set; }
    }
}