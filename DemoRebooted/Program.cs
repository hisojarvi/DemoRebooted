using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace DemoRebooted
{
    class Program
    {
        static void Main(string[] args)
        {
            var demo = new DemoEngine();
            var demoWindow = new DemoWindow(demo);
            demoWindow.Run();
        }
    }
}
