using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using DemoRebooted.Fire;

namespace DemoRebooted.CRTMonitor
{
    public class CRTMonitor
    {
        string VertexShaderFile = "DemoRebooted.CRTMonitor.CRTMonitor.vert";
        string FragmentShaderFile = "DemoRebooted.CRTMonitor.CRTMonitor.frag";

        int VertexShader;
        int FragmentShader;
        int ShaderProgram;

        int VBO;
        int VAO;
        int EBO;
        int ContentFBO;
        int ContentTexture;

        int VertexAttribPosition;
        int VertexAttribTexCoord;
        int UniformProjection;
        int UniformView;
        int UniformModel;


        float[] ScreenVertices = { -2.0f,  1.5f, 0.0f, 0.0f, 1.0f,
                                    2.0f,  1.5f, 0.0f, 1.0f, 1.0f,
                                    2.0f, -1.5f, 0.0f, 1.0f, 0.0f,
                                    -2.0f,-1.5f, 0.0f, 0.0f, 0.0f };

        int[] ScreenElements = { 0, 1, 2,
                                 0, 2, 3 };

        Matrix4 Model;
        Matrix4 View;
        Matrix4 Projection;

        Fire8Bit FireEffect;

        public CRTMonitor()
        {
            FireEffect = new Fire8Bit(320, 200);

        }

        public void Init()
        {
            FireEffect.Init();
            InitShaderProgram();
            GL.UseProgram(ShaderProgram);
            InitBuffers();
            InitVertexAttributes();
            GL.UseProgram(0);
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

        }

        void InitBuffers()
        {
            VAO = CreateVAO();
            GL.BindVertexArray(VAO);
            VBO = CreateVBO(ScreenVertices);
            EBO = CreateEBO(ScreenElements);
            ContentFBO = CreateContentFBO();
        }

        int CreateContentFBO()
        {
            var fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.ActiveTexture(TextureUnit.Texture2);
            ContentTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ContentTexture);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 
                          FireEffect.Width, FireEffect.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                                    TextureTarget.Texture2D, ContentTexture, 0);

            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "texContents"), 2);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);


            return fbo;
        }

        int CreateVBO(float[] verts)
        {
            var vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, sizeof(float) * verts.Length, verts, BufferUsageHint.StaticDraw);
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
            GL.VertexAttribPointer(VertexAttribPosition, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(VertexAttribPosition);

            VertexAttribTexCoord = GL.GetAttribLocation(ShaderProgram, "texcoord");
            GL.VertexAttribPointer(VertexAttribTexCoord, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(VertexAttribTexCoord);

            UniformModel = GL.GetUniformLocation(ShaderProgram, "model");
            UniformView = GL.GetUniformLocation(ShaderProgram, "view");
            UniformProjection = GL.GetUniformLocation(ShaderProgram, "projection");

        }

        void CheckShaderCompilationStatus(int shaderPtr)
        {
            int status = 0;
            GL.GetShader(shaderPtr, ShaderParameter.CompileStatus, out status);

            var GL_FALSE = 0;
            if (status == GL_FALSE)
            {
                var infoLog = new StringBuilder(512);
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
            Model = Matrix4.CreateRotationY(0.2f);
            View = Matrix4.LookAt(new Vector3(0.0f, 0.0f, 5.0f),
                                  new Vector3(0.0f, 0.0f, 0.0f),
                                  new Vector3(0.0f, 1.0f, 0.0f));
            Projection = Matrix4.CreatePerspectiveFieldOfView(45.0f/360.0f *(float)(2*Math.PI), 16.0f / 9.0f, 1.0f, 10.0f);
        }

        public void Render()
        {
            RenderContent();

            GL.UseProgram(ShaderProgram);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.BindVertexArray(VAO);
            GL.UniformMatrix4(UniformModel, false, ref Model);
            GL.UniformMatrix4(UniformView, false, ref View);
            GL.UniformMatrix4(UniformProjection, false, ref Projection);            
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        void RenderContent()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ContentFBO);
            FireEffect.Render();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);  
        }
    }
}
