using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace DemoRebooted
{
    class FireParticleTestScene : DemoScene
    {

        FireParticles.FireParticleSystem FireParticles;

        public FireParticleTestScene(DemoEngine engine) : base(engine)
        {
            Camera camera = new Camera();
            camera.View = Matrix4.LookAt(new Vector3(0.0f, 0.2f, 3.0f), new Vector3(0.0f, 0.0f, -10.0f), new Vector3(0.0f, 1.0f, 0.0f));
            FireParticles = new DemoRebooted.FireParticles.FireParticleSystem(24000, camera);
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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            FireParticles.Render();
        }
    }
}
