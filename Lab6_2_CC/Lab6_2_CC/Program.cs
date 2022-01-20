using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lab6_2_CC
{
    class Program
    {
        private const string APP_PATH = "https://localhost:44338";

        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();
            Menu menu = new Menu(APP_PATH,httpClient);
            menu.MenuInit();
        }
    }
}
