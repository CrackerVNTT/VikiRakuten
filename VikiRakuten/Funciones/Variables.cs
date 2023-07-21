using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikiRakuten.Funciones
{
    public class Variables
    {
        public static ConcurrentQueue<string> cooque = new ConcurrentQueue<string>();
        public static List<string> proxys = new List<string>();
        public static List<Task> tasks = new List<Task>();
        public static Request.ProxyType proxyType = new Request.ProxyType();
        public static int Hits = 0;
        public static int retrys = 0;
        public static int Freee = 0;
        public static int invalid = 0;
    }
}
