using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nano.Services;

namespace Nano.ServiceImpl
{
    public class Something : ISomething
    {
        public void Execute()
        {
            Console.WriteLine("Executed!");
        }
    }
}
