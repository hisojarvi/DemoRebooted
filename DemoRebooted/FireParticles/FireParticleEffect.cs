using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace DemoRebooted.FireParticles
{

    public class FireParticleSystem
    {
        float[] Particles;
        int NumParticles;

        int[] transformFOs;
        int[] VBOs;
        int CurrentTFO;
        int CurrentVBO;

        ShaderProgram UpdateProgram;
        ShaderProgram RenderProgram;
        int Stride = 8; // floats per particle

        public FireParticleSystem(int numParticles)
        {
            Particles = new float[Stride * numParticles];
            NumParticles = numParticles;

            string[] updateProgramVaryings = { "Position1", "Velocity1", "Size1", "Age1" };
            UpdateProgram = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleUpdate.vert"),
                                            ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleUpdate.geom"),
                                            null,
                                            updateProgramVaryings);
            
            RenderProgram = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleRender.vert"),
                                      ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleRender.geom"),
                                      ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleRender.frag"), null);

        }

        public void Init()
        {
            UpdateProgram.Use();
            InitParticles();
            CurrentTFO = 1;
            CurrentVBO = 0;

            transformFOs = new int[2];
            GL.GenTransformFeedbacks(2, transformFOs);
            VBOs = new int[2];
            GL.GenBuffers(2, VBOs);

            for (var i = 0; i < 2; i++)
            {
                GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, transformFOs[i]);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, Particles.Length * sizeof(float), Particles, BufferUsageHint.DynamicDraw);
                GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, VBOs[i]);              
            }
        }

        void InitParticles()
        {
            var r = new Random();
            for (var i = 0; i < NumParticles; i++)
            {
                Particles[Stride * i + 0] = (float)r.NextDouble();
                Particles[Stride * i + 1] = (float)r.NextDouble();
                Particles[Stride * i + 2] = (float)r.NextDouble(); 
                Particles[Stride * i + 3] = (float)r.NextDouble();
                Particles[Stride * i + 4] = (float)r.NextDouble();
                Particles[Stride * i + 5] = (float)r.NextDouble();
                Particles[Stride * i + 6] = (float)r.NextDouble();
                Particles[Stride * i + 7] = 0.0f;
            }
        }

        bool firstUpdate = true;
        public void Update(long deltaMillis)
        {
            UpdateProgram.Use();
            GL.Enable(EnableCap.RasterizerDiscard);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[CurrentVBO]);
            GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, transformFOs[CurrentTFO]);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Stride * sizeof(float), 0); // Position

            GL.BeginTransformFeedback(TransformFeedbackPrimitiveType.Points);
            if (firstUpdate)
            {
                GL.DrawArrays(PrimitiveType.Points, 0, NumParticles);
                firstUpdate = false;
            }
            else
            {
                GL.DrawTransformFeedback(PrimitiveType.Points, transformFOs[CurrentVBO]);
            }
            GL.EndTransformFeedback();

            GL.DisableVertexAttribArray(0);

            GL.Disable(EnableCap.RasterizerDiscard);            
        }

        public void Render()
        {
            RenderProgram.Use();
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[CurrentTFO]);            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0); // Position
            GL.DrawTransformFeedback(PrimitiveType.Points, transformFOs[CurrentTFO]);
            //GL.DrawArrays(PrimitiveType.Points, 0, NumParticles);
            GL.DisableVertexAttribArray(0);

            CurrentVBO = CurrentTFO;
            CurrentTFO = (CurrentTFO + 1) & 0x1;

            var error = GL.GetError();
            if (error.HasFlag(ErrorCode.InvalidValue))
            {
                Console.WriteLine(error.ToString());
            }
        }
    }
}
