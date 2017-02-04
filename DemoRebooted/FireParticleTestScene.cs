using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    class FireParticleTestScene : DemoScene
    {

        FireParticles.FireParticleSystem FireParticles;

        public FireParticleTestScene(DemoEngine engine) : base(engine)
        {
            FireParticles = new DemoRebooted.FireParticles.FireParticleSystem(10);
            FireParticles.Init();
        }

        public override void Update(long deltaMillis)
        {
            FireParticles.Update(deltaMillis);
            base.Update(deltaMillis);
        }

        public override void Render()
        {
            GL.Viewport(0, 0, Engine.Width, Engine.Height);
            FireParticles.Render();
        }
    }
}
