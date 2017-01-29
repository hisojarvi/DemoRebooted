using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
namespace DemoRebooted
{
    public class Camera
    {
        public Matrix4 Projection;
        public Matrix4 View;

        public Camera()
        {
            View = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f),
                                  new Vector3(0.0f, 0.0f, 0.0f),
                                  new Vector3(0.0f, 1.0f, 0.0f));

            Projection = Matrix4.CreatePerspectiveFieldOfView(45.0f / 360.0f * (float)(2 * Math.PI), 16.0f / 9.0f, 1.0f, 10.0f);
        }


    }
}
