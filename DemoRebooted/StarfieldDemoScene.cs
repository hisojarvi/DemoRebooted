using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoRebooted.Sky;
using OpenTK.Graphics.OpenGL4;
using DemoRebooted.FireParticles; 
namespace DemoRebooted
{
    public class StarfieldDemoScene : DemoScene
    {
        SkyBoxEffect SkyBoxEffect;
        FireParticleSystem FireParticles;

        Animator SkyBrightessAnimation = new Animator(0.0f, 1.0f, 5000, false);

        public StarfieldDemoScene(DemoEngine engine, FireParticleSystem fireParticles) : base(engine)
        {
            SkyBoxEffect = new SkyBoxEffect(fireParticles.Camera);
            FireParticles = fireParticles;
            Init();
        }

        void Init()
        {
            SkyBoxEffect.Init();
        }

        public override void Update(long deltaMillis)
        {
            base.Update(deltaMillis);
            SkyBrightessAnimation.Update(deltaMillis);
            SkyBoxEffect.Brightness = SkyBrightessAnimation.Value;
            SkyBoxEffect.Update(deltaMillis);            
            FireParticles.Opacity = 1.0f;
            FireParticles.Update(deltaMillis);
        }

        public override void Render()
        {
            GL.Viewport(0, 0, Engine.Width, Engine.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);
            SkyBoxEffect.Render();
            GL.Enable(EnableCap.DepthTest);
            FireParticles.Render();
        }
    }
}
