using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KellyHoshira
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Kelly Hoshira");
            KellyHoshiraBot bot = new KellyHoshiraBot();
            bot.Connect();
        }
    }
}
