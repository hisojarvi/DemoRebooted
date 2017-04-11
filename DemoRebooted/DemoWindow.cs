using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class DemoWindow : GameWindow
    {
        DemoEngine Engine;
        public DemoWindow() : base(800, 450, new GraphicsMode(32, 0, 0, 4))
        {
            CursorVisible = false;
            WindowState = WindowState.Fullscreen;
            WindowBorder = WindowBorder.Hidden;
            VSync = VSyncMode.On;
            Title = "Demo Rebooted";                         
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Engine = new DemoEngine();
            var FireScene = new FireDemoScene(Engine);
            var StarfieldScene = new StarfieldDemoScene(Engine, FireScene.FireParticles);
            Engine.AddScene(FireScene, 24000);
            Engine.AddScene(StarfieldScene, 10000);
            Engine.Init();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Engine.Render();
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

            base.OnUpdateFrame(e);
            Engine.Update();
            if (Keyboard[OpenTK.Input.Key.Escape] || Engine.Finished)
            {
                Exit();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Engine.Resize(Width, Height);
        }
    }
}
