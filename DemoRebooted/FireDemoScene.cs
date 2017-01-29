using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class FireDemoScene : DemoScene
    {
        Fire.Fire8Bit FireEffect;
        Billboard.BillboardEffect FireBackground;
        Framebuffer FireFrameBuffer;
        CRTMonitor.CRTMonitor CRTMonitorEffect;

        public FireDemoScene(DemoEngine engine) : base(engine)
        {
            FireFrameBuffer = new Framebuffer(320, 200, 3);
            FireEffect = new Fire.Fire8Bit(320, 200);            
            CRTMonitorEffect = new CRTMonitor.CRTMonitor(1920, 1080, FireFrameBuffer.Texture);
            FireEffect.BitBlend = 0.35f;
            FireEffect.Init();            
            CRTMonitorEffect.Init();
            FireBackground = new Billboard.BillboardEffect(1920, 1080, FireFrameBuffer.Texture);
        }

        public override void Update(long deltaMillis)
        {
            FireEffect.Update(deltaMillis);
            CRTMonitorEffect.Update(deltaMillis);
            base.Update(deltaMillis);
        }

        public override void Render()
        {
            GL.Viewport(0, 0, Engine.Width, Engine.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.DepthTest);
            FireFrameBuffer.Bind();
            FireEffect.Render();
            FireFrameBuffer.Unbind();
            FireBackground.Render();
            GL.Enable(EnableCap.DepthTest);
            CRTMonitorEffect.Render();
        }
    }
}
