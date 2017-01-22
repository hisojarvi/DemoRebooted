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
            /*
            var demo = new DemoEngine();
            Application.Run(new FormDemoMain(demo));
            */
            var demo = new DemoEngine(800, 450);
            var demoWindow = new DemoWindow(demo);
            demoWindow.Run();
        }
    }
}
