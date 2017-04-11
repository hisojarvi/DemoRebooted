using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace DemoRebooted.FireParticles
{

    public class FireParticleSystem
    {

        int NumParticles;
        FireParticle[] Particles;        

        int[] transformFOs;
        int[] VBOs;
        int CurrentTFO;
        int CurrentVBO;

        Texture FireSprites;
        Texture FireSpritesPalette;
        public Camera Camera;
        Random Rand = new Random();

        ShaderProgram UpdateProgram;
        ShaderProgram RenderProgram;
        int Stride = 12; // floats per particle

        public float Opacity = 1.0f;

        public FireParticleSystem(int numParticles, Camera camera)
        {
            Particles = new FireParticle[numParticles];
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
                GL.BufferData(BufferTarget.ArrayBuffer, Particles.Length * Stride * sizeof(float), Particles, BufferUsageHint.DynamicDraw);
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

        void InitParticles()
        {            
            for (var i = 0; i < NumParticles; i++)
            {
                Particles[i] = CreateParticle();
            }
        }

        float maxSideSpeed = 0.5f;
        FireParticle CreateParticle()
        {
            FireParticle particle = new FireParticle();
            particle.Position = new Vector3(-3.0f+6*(float)Rand.NextDouble(),
                                            0.1f*(float)Rand.NextDouble(),
                                            -6.0f + 6 * (float)Rand.NextDouble());
            particle.Velocity = new Vector3(maxSideSpeed * 2.0f * (float)(Rand.NextDouble() - 0.5), 
                                            1.2f + 0.5f * (float)Rand.NextDouble(), 
                                            maxSideSpeed * 2.0f * (float)(Rand.NextDouble() - 0.5));
            particle.Type = (float)Rand.Next(0, 3);
            particle.Age = 0.0f;
            particle.MaxAge = 0.1f + 0.8f * (float)Rand.NextDouble();
            particle.Size = 0.05f + 0.05f * (float)Rand.NextDouble();
            particle.Rotation = 2.0f * 3.14159f * (float)Rand.NextDouble();
            return particle;
        }

        public void Update(long deltaMillis)
        {
            UpdateParticles(deltaMillis);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[CurrentTFO]);
            GL.BufferData(BufferTarget.ArrayBuffer, Particles.Length * Stride * sizeof(float), Particles, BufferUsageHint.StreamDraw);
        }

        void UpdateParticles(long deltaMillis)
        {
            var gravity = 1.0f;

            for (var i = 0; i < NumParticles; i++)
            {
                if (Particles[i].Age > Particles[i].MaxAge)
                {
                    Particles[i] = CreateParticle();
                }
                else
                {
                    var particle = Particles[i];
                    particle.Position += deltaMillis / 1000.0f * particle.Velocity;

                    var dx = particle.Velocity.X;
                    dx = dx / (1 + (deltaMillis / 1000.0f));
                    particle.Velocity.X = dx;

                    particle.Velocity.Y += deltaMillis / 1000.0f * gravity; // Gravity 

                    var dz = particle.Velocity.Z;
                    dz = dz / (1 + (deltaMillis / 1000.0f));
                    particle.Velocity.Z = dz;

                    particle.Age += (float)deltaMillis / 1000.0f; // age                    

                    particle.Rotation += 1.0f * (float)deltaMillis / 1000.0f;

                    Particles[i] = particle;
                }
            }

        }

        public void Render()
        {
            RenderProgram.Use();

            var ModelMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            GL.UniformMatrix4(RenderProgram.Uniform("model"), false, ref ModelMatrix);
            GL.UniformMatrix4(RenderProgram.Uniform("view"), false, ref Camera.View);
            GL.UniformMatrix4(RenderProgram.Uniform("projection"), false, ref Camera.Projection);
            GL.Uniform1(RenderProgram.Uniform("opacity"), Opacity);

            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcColor);
            GL.Disable(EnableCap.DepthTest);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOs[CurrentTFO]);            
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Stride * sizeof(float), 0); // Position
            GL.VertexAttribPointer(1, 1, VertexAttribPointerType.Float, false, Stride * sizeof(float), 6 * sizeof(float)); // Sprite
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, Stride * sizeof(float), 7 * sizeof(float)); // Age
            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, Stride * sizeof(float), 8 * sizeof(float)); // MaxAge
            GL.VertexAttribPointer(4, 1, VertexAttribPointerType.Float, false, Stride * sizeof(float), 9 * sizeof(float)); // Size
            GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, Stride * sizeof(float), 10 * sizeof(float)); // Rotation
            GL.DrawArrays(PrimitiveType.Points, 0, NumParticles);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(4);

            //CurrentVBO = CurrentTFO;
            //CurrentTFO = (CurrentTFO + 1) & 0x1;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FireParticle
    {
        public Vector3 Position;
        public Vector3 Velocity;
        public float Type;
        public float Age;
        public float MaxAge;
        public float Size;
        public float Rotation;
        public float RotationSpeed;
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
