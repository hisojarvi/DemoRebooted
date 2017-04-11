using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Resources;
using System.IO;

namespace DemoRebooted.Fire
{
    class Fire8Bit
    {
        public int Width { get; }
        public int Height { get; }

        public float BitBlend = 0.0f;
        public double EmitProbability = 0.12;

        ShaderProgram Program;
        Mesh Canvas;
        Camera Camera;

        public Texture FireTexture;
        int Palette8BitTexture;
        int Palette16BitTexture;

        int UniformBlend;

        Random RNG = new Random();


        float[] CanvasVertices = { -1.0f,  1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f,
                                    1.0f,  1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, -1.0f,
                                    1.0f, -1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
                                   -1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, -1.0f };

        int[] CanvasElements = { 0, 2, 1,
                                 0, 3, 2 };

        float[] Palette8BitData = { 0.02f, 0.02f, 0.02f,
                                    0.531f, 0.000f, 0.000f,
                                    1.000f, 0.465f, 0.465f,
                                    0.863f, 0.531f, 0.330f,
                                    0.930f, 0.930f, 0.465f,
                                    1.0f, 1.0f, 1.0f,};

        float[] Palette16BitData = { 0.0f, 0.0f, 0.0f,
                                     0.3f, 0.0f, 0.0f,
                                     0.8f, 0.1f, 0.0f,
                                     0.95f, 0.4f, 0.22f,
                                     0.95f, 0.8f, 0.22f,
                                    1.0f, 1.0f, 1.0f,};


        /*        float[] Palette16BitData = { 0.0f, 0.0f, 0.0f,
                                            1.0f, 0.0f, 0.0f,
                                            1.0f, 1.0f, 0.0f,
                                            1.0f, 1.0f, 1.0f,};
        */
        byte[] FireData;

        public Fire8Bit(int w, int h)
        {
            Width = w;
            Height = h;
            FireData = new byte[w * h];
            Program = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.Fire.Fire8Bit.vert"),
                                        ResourceUtils.ReadResourceFile("DemoRebooted.Fire.Fire8Bit.frag"));
            Canvas = new Mesh(CanvasVertices, CanvasElements, Program);
            Camera = new Camera();
        }

        public void Init()
        {
            Program.Use();
            UniformBlend = Program.Uniform("blend");            
            InitTextures();   
        }

        byte fade = 2;
        void UpdateFireData()
        {
            for (var i = Width * (Height - 1); i < FireData.Length; i++)
            {
                if (RNG.NextDouble() < EmitProbability)
                {
                    FireData[i] = (byte)Math.Min(255, FireData[i] + RNG.Next(40));
                }
                else
                {
                    FireData[i] = (byte)Math.Max(0, FireData[i] - fade);
                }
            }
            for (var i = 0; i < Width*(Height-1); i++)
            {
                var below = FireData[i+Width];
                var belowLeft = FireData[i + Width - 1];
                var belowRight = (i + Width + 1) < FireData.Length ? FireData[i + Width + 1] : 0;
                var blended = (byte)(0.8 * below + 0.15 * belowLeft + 0.05 * belowRight);
                var newColor = (byte)Math.Max(0, blended - fade);
                FireData[i] = newColor;
            }
        }

        void UploadFireData()
        {
            GL.ActiveTexture(TextureUnit.Texture0 + FireTexture.TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, FireTexture.Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, Width, Height, 0, PixelFormat.Luminance,
                          PixelType.UnsignedByte, FireData);           
        }

       
        void InitTextures()
        {
            // Fire texture
            FireTexture = new Texture(0);
            FireTexture.Activate();
            GL.BindTexture(TextureTarget.Texture2D, FireTexture.Id);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, Width, Height, 0, PixelFormat.Luminance,
                          PixelType.UnsignedByte, FireData);
            GL.Uniform1(Program.Uniform("texFire"), 0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D); // Why is this required?

            // 8-bit palette
            Palette8BitTexture = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture1D, Palette8BitTexture);
            GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgb, 
                          Palette8BitData.Length / 3, 0, PixelFormat.Rgb, PixelType.Float, Palette8BitData);
            GL.Uniform1(Program.Uniform("texPalette8Bit"), 1);

            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture1D); // Why is this required?

            // 16-bit palette
            Palette16BitTexture = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture1D, Palette16BitTexture);
            GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgb,
                          Palette16BitData.Length / 3, 0, PixelFormat.Rgb, PixelType.Float, Palette16BitData);
            GL.Uniform1(Program.Uniform("texPalette16Bit"), 2);

            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture1D); // Why is this required?
        }
        
        public void Update(long deltaMillis)
        {
            UpdateFireData();
            UploadFireData();
        }

        public void Render()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            Program.Use();
            GL.Uniform1(UniformBlend, BitBlend);
            Canvas.Render(Camera);
        }
    }
}
