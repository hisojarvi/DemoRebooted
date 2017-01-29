using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;


namespace DemoRebooted
{
    public class DemoEngine
    {
        long Frame = 0;
        long Elapsed = 0;
        Stopwatch Stopwatch;

        public int Width;
        public int Height;



        FireDemoScene FireScene;

        public DemoEngine()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            FireScene = new FireDemoScene(this);
        }

        public void Init()
        {

        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Update()
        {
            Frame++;
            var deltaMillis = Stopwatch.ElapsedMilliseconds - Elapsed;
            Elapsed = Stopwatch.ElapsedMilliseconds;
            FireScene.Update(deltaMillis);
        }

        public void Render()
        {
            FireScene.Render();
        }

        private void RenderSanityCheck()
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
           
            // Animate trianGLe
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Rotate(Elapsed*0.01f, 0.0f, 0.0f, 1.0f);

            // Old school OpenGL
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex2(0.0f, 0.0f);
            GL.Color3(0.0f, 1.0f, 0.0f); GL.Vertex2(0.5f, 1.0f);
            GL.Color3(0.0f, 0.0f, 1.0f); GL.Vertex2(1.0f, 0.0f);
            GL.End();
        }
    }
}
