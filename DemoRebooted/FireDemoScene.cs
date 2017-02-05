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
        FireParticles.FireParticleSystem FireParticles;


        public FireDemoScene(DemoEngine engine) : base(engine)
        {

            FireFrameBuffer = new Framebuffer(320, 200, 3);           
            FireEffect = new Fire.Fire8Bit(320, 200);            
            CRTMonitorEffect = new CRTMonitor.CRTMonitor(1920, 1080, FireFrameBuffer.Texture);

            FireParticles = new FireParticles.FireParticleSystem(10, CRTMonitorEffect.Camera);
            FireEffect.BitBlend = 0.0f;
            FireEffect.Init();            
            CRTMonitorEffect.Init();
            FireParticles.Init();
            FireBackground = new Billboard.BillboardEffect(1920, 1080, FireFrameBuffer.Texture);                        
        }

        public override void Update(long deltaMillis)
        {
            //FireEffect.Update(deltaMillis);
            //CRTMonitorEffect.Update(deltaMillis);
            FireParticles.Update(deltaMillis);
            base.Update(deltaMillis);
            if (ElapsedMillis > 14000)
            {
                var blend = Math.Min(1.0f, (ElapsedMillis - 14000) / 5000.0f);
                FireEffect.BitBlend = blend;
            }
        }

        public override void Render()
        {
            GL.Viewport(0, 0, Engine.Width, Engine.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            /*
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.DepthTest);
            FireFrameBuffer.Bind();
            FireEffect.Render();
            FireFrameBuffer.Unbind();            
            if (ElapsedMillis >= 5000)
            {
                FireBackground.Render();
            }
            GL.Enable(EnableCap.DepthTest);
            CRTMonitorEffect.Render();
            */
            FireParticles.Render();
        }
    }
}
