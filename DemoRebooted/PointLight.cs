using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace DemoRebooted
{
    public class PointLight
    {
        public Vector3 Position = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 Color = new Vector3(1.0f, 1.0f, 1.0f);
        public float Intensity = 1.0f;
    }
}
