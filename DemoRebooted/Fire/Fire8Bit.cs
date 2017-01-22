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

        string VertexShaderFile = "DemoRebooted.Fire.Fire8Bit.vert";
        string FragmentShaderFile = "DemoRebooted.Fire.Fire8Bit.frag";

        int VertexShader;
        int FragmentShader;
        int ShaderProgram;

        int VBO;
        int VAO;
        int EBO;
        int FireTexture;
        int Palette8BitTexture;
        int Palette16BitTexture;

        int VertexAttribPosition;
        int VertexAttribTexCoord;
        int UniformBlend;

        Random RNG = new Random();


        float[] CanvasVertices = { -1.0f,  1.0f, 0.0f, 0.0f,
                                    1.0f,  1.0f, 1.0f, 0.0f,
                                    1.0f, -1.0f, 1.0f, 1.0f,
                                   -1.0f, -1.0f, 0.0f, 1.0f };

        int[] CanvasElements = { 0, 1, 2,
                                 0, 2, 3 };

        float[] Palette8BitData = { 0.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 1.0f, 0.0f,
                                    1.0f, 1.0f, 1.0f,};

        float[] Palette16BitData = { 0.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 1.0f, 0.0f,
                                    1.0f, 1.0f, 1.0f,};

        byte[] FireData;

        public Fire8Bit(int w, int h)
        {
            Width = w;
            Height = h;
            FireData = new byte[w * h];
        }

        public void Init()
        {
            InitShaderProgram();
            InitBuffers();
            InitVertexAttributes();
            InitTextures();
            InitUniforms();        
        }

        void InitUniforms()
        {
            UniformBlend = GL.GetUniformLocation(ShaderProgram, "blend");
        }

        byte fade = 2;
        void UpdateFireData()
        {
            for (var i = Width * (Height - 1); i < FireData.Length; i++)
            {
                if (RNG.Next(100) > 88)
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
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, FireTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, Width, Height, 0, PixelFormat.Luminance,
                          PixelType.UnsignedByte, FireData);           
        }

        void InitShaderProgram()
        {
            var vertShaderSource = ReadResourceFile(VertexShaderFile);
            var fragShaderSource = ReadResourceFile(FragmentShaderFile); 
           
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vertShaderSource);
            GL.CompileShader(VertexShader);
            CheckShaderCompilationStatus(VertexShader);
            
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragShaderSource);
            GL.CompileShader(FragmentShader);
            CheckShaderCompilationStatus(FragmentShader);

            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.BindFragDataLocation(ShaderProgram, 0, "outColor");
            GL.LinkProgram(ShaderProgram);        
            GL.UseProgram(ShaderProgram);
        }

        void InitBuffers()
        {
            VAO = CreateVAO();
            GL.BindVertexArray(VAO);
            VBO = CreateVBO(CanvasVertices);
            EBO = CreateEBO(CanvasElements);
        }

        int CreateVBO(float[] verts)
        {
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, sizeof(float)*verts.Length, verts, BufferUsageHint.StaticDraw );
            return vbo;
        }

        int CreateEBO(int[] elements)
        {
            var ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, sizeof(int) * elements.Length, elements, BufferUsageHint.StaticDraw);
            return ebo;
        }

        int CreateVAO()
        {
            return GL.GenVertexArray();
        }

        void InitVertexAttributes()
        {
            VertexAttribPosition = GL.GetAttribLocation(ShaderProgram, "position");
            GL.VertexAttribPointer(VertexAttribPosition, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(VertexAttribPosition);

            VertexAttribTexCoord = GL.GetAttribLocation(ShaderProgram, "texcoord");
            GL.VertexAttribPointer(VertexAttribTexCoord, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(VertexAttribTexCoord);

        }

        void InitTextures()
        {            
            // Fire texture
            FireTexture = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, FireTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Luminance, Width, Height, 0, PixelFormat.Luminance,
                          PixelType.UnsignedByte, FireData);
            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "texFire"), 0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D); // Why is this required?

            // 8-bit palette
            Palette8BitTexture = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture1D, Palette8BitTexture);
            GL.TexImage1D(TextureTarget.Texture1D, 0, PixelInternalFormat.Rgb, 
                          Palette8BitData.Length / 3, 0, PixelFormat.Rgb, PixelType.Float, Palette8BitData);
            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "texPalette8Bit"), 1);

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
            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "texPalette16Bit"), 2);

            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture1D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture1D); // Why is this required?


        }

        void CheckShaderCompilationStatus(int shaderPtr)
        {
            int status = 0;
            GL.GetShader(shaderPtr, ShaderParameter.CompileStatus, out status);

            var GL_FALSE = 0;
            if (status == GL_FALSE)
            {
                var infoLog = new StringBuilder();
                int infoLength = 0;
                GL.GetShaderInfoLog(shaderPtr, 512, out infoLength, infoLog);
                Console.WriteLine(infoLog.ToString());
            }
        }

        string ReadResourceFile(string fileName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public void Update()
        {
            UpdateFireData();
            UploadFireData();
        }

        public void Render()
        {          
            GL.UseProgram(ShaderProgram);
            GL.BindVertexArray(VAO);
            GL.Uniform1(UniformBlend, BitBlend);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }
    }
}
