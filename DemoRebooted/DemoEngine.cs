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
        long ElapsedTotal = 0;
        long ElapsedCurrentScene = 0;
        Stopwatch Stopwatch;

        public int Width;
        public int Height;

        public bool Finished = false;

        List<DemoSceneSeqElement> SceneSequence;
        DemoSceneSeqElement ActiveScene;
        int ActiveSceneIndex;

        public DemoEngine()
        {
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
            SceneSequence = new List<DemoSceneSeqElement>();
        }

        public void AddScene(DemoScene scene, long durationMillis)
        {
            var e = new DemoSceneSeqElement(scene, durationMillis);
            SceneSequence.Add(e);
        }

        public void Init()
        {
            ActiveSceneIndex = 0;
            ActiveScene = SceneSequence[ActiveSceneIndex];
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void Update()
        {
            Frame++;
            var deltaMillis = Stopwatch.ElapsedMilliseconds - ElapsedTotal;
            ElapsedTotal = Stopwatch.ElapsedMilliseconds;
            ElapsedCurrentScene += deltaMillis;
            if (ElapsedCurrentScene >= ActiveScene.DurationMillis)
            {
                ActiveSceneIndex++;
                if (ActiveSceneIndex < SceneSequence.Count)
                {
                    ElapsedCurrentScene -= ActiveScene.DurationMillis;
                    ActiveScene = SceneSequence[ActiveSceneIndex];
                }
                else
                {
                    Finished = true;
                }
            }
            ActiveScene.Scene.Update(deltaMillis);
        }

        public void Render()
        {
            ActiveScene.Scene.Render();
        }

        private void RenderSanityCheck()
        {
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
           
            // Animate trianGLe
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Rotate(ElapsedTotal*0.01f, 0.0f, 0.0f, 1.0f);

            // Old school OpenGL
            GL.Begin(PrimitiveType.Triangles);
            GL.Color3(1.0f, 0.0f, 0.0f); GL.Vertex2(0.0f, 0.0f);
            GL.Color3(0.0f, 1.0f, 0.0f); GL.Vertex2(0.5f, 1.0f);
            GL.Color3(0.0f, 0.0f, 1.0f); GL.Vertex2(1.0f, 0.0f);
            GL.End();
        }
    }

    class DemoSceneSeqElement
    {
        public DemoScene Scene;
        public long DurationMillis;

        public DemoSceneSeqElement(DemoScene scene, long durationMillis)
        {
            Scene = scene;
            DurationMillis = durationMillis;
        }
    }
}
