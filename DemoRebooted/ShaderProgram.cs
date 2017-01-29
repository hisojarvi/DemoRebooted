using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class ShaderProgram
    {
        int _id;
        public int Id { get { return _id; } }

        string VertexShaderSource;
        string FragmentShaderSource;
        int VertexShader;
        int FragmentShader;

        public ShaderProgram(string vertShaderSource, string fragShaderSource)
        {
            VertexShaderSource = vertShaderSource;
            FragmentShaderSource = fragShaderSource;            
            Init();
        }

        public void Use()
        {
            GL.UseProgram(Id);
        }

        public int Attrib(string name)
        {
            return GL.GetAttribLocation(Id, name);
        }

        public int Uniform(string name)
        {
            return GL.GetUniformLocation(Id, name);
        }

        void Init()
        {
            _id = GL.CreateProgram();
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);
            CheckShaderCompilationStatus(VertexShader);

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);
            CheckShaderCompilationStatus(FragmentShader);
            
            GL.AttachShader(Id, VertexShader);
            GL.AttachShader(Id, FragmentShader);
            GL.BindFragDataLocation(Id, 0, "outColor");
            GL.LinkProgram(Id);
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
    }
}
