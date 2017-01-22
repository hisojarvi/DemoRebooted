using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class DemoWindow : GameWindow
    {
        DemoEngine Engine;
        public DemoWindow(DemoEngine engine)
        {
            Engine = engine;
            Width = engine.Width;
            Height = engine.Height;
            CursorVisible = false;
            WindowState = WindowState.Fullscreen;
            WindowBorder = WindowBorder.Hidden;
            VSync = VSyncMode.On;
            Title = "Demo Rebooted";
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
            if (Keyboard[OpenTK.Input.Key.Escape])
            {
                Exit();
            }

            base.OnUpdateFrame(e);
            Engine.Update();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Engine.Resize(Width, Height);
        }

    }
}
