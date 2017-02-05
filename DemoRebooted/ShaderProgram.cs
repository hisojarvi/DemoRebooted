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
        string GeometryShaderSource;
        string FragmentShaderSource;
        int VertexShader;
        int GeometryShader;
        int FragmentShader;
        string[] Varyings;

        public ShaderProgram(string vertShaderSource, string fragShaderSource)
        {
            VertexShaderSource = vertShaderSource;
            FragmentShaderSource = fragShaderSource;            
            Init(false);
        }

        public ShaderProgram(string vertShaderSource, string geomShaderSource, string fragShaderSource, string[] varyings)
        {
            VertexShaderSource = vertShaderSource;
            GeometryShaderSource = geomShaderSource;
            FragmentShaderSource = fragShaderSource;
            Varyings = varyings;
            Init(true);
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

        void Init(bool hasGeometryShader)
        {
            _id = GL.CreateProgram();

            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);
            CheckShaderCompilationStatus(VertexShader);
            GL.AttachShader(Id, VertexShader);

            if (hasGeometryShader)
            {
                GeometryShader = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(GeometryShader, GeometryShaderSource);
                GL.CompileShader(GeometryShader);
                CheckShaderCompilationStatus(GeometryShader);
                GL.AttachShader(Id, GeometryShader);
            }

            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);
            CheckShaderCompilationStatus(FragmentShader);
            GL.AttachShader(Id, FragmentShader);
            GL.BindFragDataLocation(Id, 0, "outColor");


            if (Varyings != null)
            {
                GL.TransformFeedbackVaryings(Id, Varyings.Length, Varyings, TransformFeedbackMode.InterleavedAttribs);
            }
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

        public void DebugPrintAttributes()
        {
            Console.WriteLine("Attributes:");
            int count = 0;
            GL.GetProgram(Id, GetProgramParameterName.ActiveAttributes, out count);
            for (var i = 0; i < count; i++)
            {
                int size;
                ActiveAttribType type;
                Console.WriteLine(GL.GetActiveAttrib(Id, i, out size, out type));
            }
        }
    }
}
