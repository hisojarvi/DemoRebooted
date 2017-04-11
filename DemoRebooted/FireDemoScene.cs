using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class FireDemoScene : DemoScene
    {
        Fire.Fire8Bit FireEffect;
        Billboard.BillboardEffect FireBackground;
        Framebuffer FireFrameBuffer;
        CRTMonitor.CRTMonitor CRTMonitorEffect;
        public FireParticles.FireParticleSystem FireParticles;

        bool IsFireBackgroundVisible;

        public FireDemoScene(DemoEngine engine) : base(engine)
        {
            FireFrameBuffer = new Framebuffer(320, 200, 3);           
            FireEffect = new Fire.Fire8Bit(320, 200);
            FireEffect.BitBlend = 0.0f;
            FireEffect.Init();
            CRTMonitorEffect = new CRTMonitor.CRTMonitor(1920, 1080, FireFrameBuffer.Texture);
            CRTMonitorEffect.Init();
            FireBackground = new Billboard.BillboardEffect(1920, 1080, FireFrameBuffer.Texture);

            var fireParticleCamera = new Camera();
            fireParticleCamera.View = Matrix4.LookAt(new Vector3(0.0f, 0.3f, 1.7f),
                            new Vector3(0.0f, 0.8f, 0.0f),
                            new Vector3(0.0f, 1.0f, 0.0f));
            FireParticles = new FireParticles.FireParticleSystem(24000, fireParticleCamera);
            FireParticles.Opacity = 0.0f;                    
            FireParticles.Init();

            IsFireBackgroundVisible = false;
        }

        public override void Update(long deltaMillis)
        {
            FireEffect.Update(deltaMillis);
            CRTMonitorEffect.Update(deltaMillis);
            FireParticles.Update(deltaMillis);
            base.Update(deltaMillis);

            if (ElapsedMillis >= 5000)
            {
                IsFireBackgroundVisible = true;
            }
            if (ElapsedMillis > 14000)
            {
                var blend = Math.Min(1.0f, (ElapsedMillis - 14000) / 5000.0f);
                FireEffect.BitBlend = blend;
            }
            if (ElapsedMillis > 21000)
            {
                FireEffect.EmitProbability = 0.0;
            }
            if (ElapsedMillis > 21000)
            {
                var fireParticleOpacity = Math.Min(1.0f, (ElapsedMillis-21000)/4000.0f);
                FireParticles.Opacity = fireParticleOpacity;
            }           
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
            if (IsFireBackgroundVisible)
            {
                FireBackground.Render();
            }
            GL.Enable(EnableCap.DepthTest);
            CRTMonitorEffect.Render();
            FireParticles.Render();
        }
    }
}
