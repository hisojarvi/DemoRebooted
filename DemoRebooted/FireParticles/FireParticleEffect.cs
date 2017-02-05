using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using System.Drawing;
using System.Drawing.Imaging;


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

        Texture FireSprites;
        Texture FireSpritesPalette;
        Camera Camera;
        Random Rand = new Random();

        ShaderProgram UpdateProgram;
        ShaderProgram RenderProgram;
        int Stride = 8; // floats per particle

        public FireParticleSystem(int numParticles, Camera camera)
        {
            Particles = new float[Stride * numParticles];
            NumParticles = numParticles;
            Camera = camera;
            /*
            string[] updateProgramVaryings = { "Position1", "Velocity1", "Size1", "Age1" };
            UpdateProgram = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleUpdate.vert"),
                                            ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleUpdate.geom"),
                                            null,
                                            updateProgramVaryings);
            */
            RenderProgram = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleRender.vert"),
                                      ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleRender.geom"),
                                      ResourceUtils.ReadResourceFile("DemoRebooted.FireParticles.FireParticleRender.frag"), null);

        }

        public void Init()
        {
            RenderProgram.Use();
            InitParticles();
            CurrentTFO = 1;
            CurrentVBO = 0;

            transformFOs = new int[2];
            GL.GenTransformFeedbacks(2, transformFOs);
            VBOs = new int[2];
            GL.GenBuffers(2, VBOs);

            for (var i = 0; i < 2; i++)
            {
                //GL.BindTransformFeedback(TransformFeedbackTarget.TransformFeedback, transformFOs[i]);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, Particles.Length * sizeof(float), Particles, BufferUsageHint.DynamicDraw);
                //GL.BindBufferBase(BufferRangeTarget.TransformFeedbackBuffer, 0, VBOs[i]);              
            }
            InitTextures();
        }

        void InitTextures()
        {
            // Fire sprites

            Bitmap texBitmap = ResourceUtils.ReadResourceImage("DemoRebooted.Resources.FireSprites2.png");
            BitmapData data = texBitmap.LockBits(new System.Drawing.Rectangle(0, 0, texBitmap.Width, texBitmap.Height),
                                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            FireSprites = new Texture(5);
            FireSprites.Activate();
            GL.BindTexture(TextureTarget.Texture2D, FireSprites.Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texBitmap.Width, texBitmap.Height, 0,
                          OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            texBitmap.UnlockBits(data);
            texBitmap.Dispose();

            GL.Uniform1(RenderProgram.Uniform("texFireSprites"), FireSprites.TextureUnit);
            GL.Uniform1(RenderProgram.Uniform("numSprites"), 3);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            

            // Fire palette        
            texBitmap = ResourceUtils.ReadResourceImage("DemoRebooted.Resources.FireSpritesPalette.png");
            data = texBitmap.LockBits(new System.Drawing.Rectangle(0, 0, texBitmap.Width, texBitmap.Height),
                                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            FireSpritesPalette = new Texture(6);
            FireSpritesPalette.Activate();
            GL.BindTexture(TextureTarget.Texture2D, FireSpritesPalette.Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texBitmap.Width, texBitmap.Height, 0,
                          OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            texBitmap.UnlockBits(data);
            texBitmap.Dispose();

            GL.Uniform1(RenderProgram.Uniform("texFireSpritesPalette"), FireSpritesPalette.TextureUnit);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }


        // http://www.astralax.ru/en/articles/inflames
        void InitParticles()
        {
            
            for (var i = 0; i < NumParticles; i++)
            {
                InitParticle(i * Stride);
            }
        }

        float maxSideSpeed = 1.0f;
        void InitParticle(int offset)
        {
            Particles[offset + 0] = (float)Rand.NextDouble(); // Position
            Particles[offset + 1] = (float)Rand.NextDouble();
            Particles[offset + 2] = (float)Rand.NextDouble();
            Particles[offset + 3] = maxSideSpeed * (float)(Rand.NextDouble() - 0.5); // Velocity
            Particles[offset + 4] = 0.5f;
            Particles[offset + 5] = maxSideSpeed * (float)(Rand.NextDouble() - 0.5); 
            Particles[offset + 6] = (float)Rand.Next(0, 3);    // Type
            Particles[offset + 7] = 0.0f;                  // Age 
        }

        public void Update(long deltaMillis)
        {
            UpdateParticles(deltaMillis);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[CurrentTFO]);
            GL.BufferData(BufferTarget.ArrayBuffer, Particles.Length * sizeof(float), Particles, BufferUsageHint.StreamDraw);
        }

        void UpdateParticles(long deltaMillis)
        {
            var gravity = 1.0f;
            var horizontalDemp = 0.3f;
            for (var i = 0; i < NumParticles; i++)
            {
                Particles[Stride * i + 0] += deltaMillis / 1000.0f * Particles[Stride * i + 3];
                Particles[Stride * i + 1] += deltaMillis / 1000.0f * Particles[Stride * i + 4];
                Particles[Stride * i + 2] += deltaMillis / 1000.0f * Particles[Stride * i + 5];
                // Velocity

                var dx = Particles[Stride * i + 3];
                dx = dx / (1 + (deltaMillis / 1000.0f));
                Particles[Stride * i + 3] = dx;

                Particles[Stride * i + 4] += deltaMillis / 1000.0f * gravity; // Gravity 

                var dz = Particles[Stride * i + 5];
                dz = dz / (1 + (deltaMillis / 1000.0f));
                Particles[Stride * i + 5] = dz;

                Particles[Stride * i + 7] += (float)deltaMillis / 1000.0f; // age
            }
        }

        public void Render()
        {
            RenderProgram.Use();

            var ModelMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            GL.UniformMatrix4(RenderProgram.Uniform("model"), false, ref ModelMatrix);
            GL.UniformMatrix4(RenderProgram.Uniform("view"), false, ref Camera.View);
            GL.UniformMatrix4(RenderProgram.Uniform("projection"), false, ref Camera.Projection);

            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.DepthTest);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[CurrentTFO]);            
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Stride * sizeof(float), 0); // Position
            GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, Stride * sizeof(float), 6 * sizeof(float)); // Size
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, Stride * sizeof(float), 7 * sizeof(float)); // Age
            GL.DrawArrays(PrimitiveType.Points, 0, NumParticles);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            //CurrentVBO = CurrentTFO;
            //CurrentTFO = (CurrentTFO + 1) & 0x1;
        }
    }
}
/*
        bool firstUpdate = true;
        void UpdateTransformFeedback(long deltaMillis)
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
        */
