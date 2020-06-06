using Faculty.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Data
{
    public class SeedData
    {
        public SeedData(IWebHostEnvironment env)
        {
            _env = env;
        }
        private readonly IWebHostEnvironment _env;
        public  List<T> GetItems<T>(string fileName) where T : IEntityBase, new() //fileName - только имя с расширением
        {
            var path = Path.Combine(_env.WebRootPath,"seeddata");//_env.WebRootPath -> wwwroot
            path = Path.Combine(path, fileName);
            var items = new List<T>();
            // for Encoding.GetEncoding(1251) to work properly in net.core install nuget System.Text.Encoding.CodePages and RegisterProvider
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StreamReader file = new StreamReader(path, Encoding.GetEncoding(1251));//Cyrillic (Windows)
            string line;
            int counter = 1;
            while ((line = file.ReadLine()) != null)
            {
                items.Add(new T() { Id = counter++, Name = line });
            }
            file.Close();
            return items;
        }
    }
}
