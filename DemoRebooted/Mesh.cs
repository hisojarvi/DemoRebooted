using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class Mesh
    {
        public float[] Vertices;
        public int[] Elements;

        int VBO;
        int VAO;
        int EBO;
        ShaderProgram Program;

        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Vector4 Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        public Mesh(float[] verts, int[] elements, ShaderProgram program)
        {
            Vertices = verts;
            Elements = elements;
            Program = program;
            Init();
        }

        void Init()
        {
            VAO = CreateVAO();
            GL.BindVertexArray(VAO);
            VBO = CreateVBO(Vertices);
            EBO = CreateEBO(Elements);
            InitVertexAttributes(Program);
            GL.BindVertexArray(0);
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

        public void InitVertexAttributes(ShaderProgram program)
        {
            GL.EnableVertexAttribArray(program.Attrib("position"));
            GL.VertexAttribPointer(program.Attrib("position"), 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            GL.EnableVertexAttribArray(program.Attrib("texcoord"));
            GL.VertexAttribPointer(program.Attrib("texcoord"), 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(program.Attrib("normal"));
            GL.VertexAttribPointer(program.Attrib("normal"), 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));
        }


        public void Render(Camera camera)
        {
            Program.Use();
            GL.UniformMatrix4(Program.Uniform("model"), false, ref ModelMatrix);
            GL.UniformMatrix4(Program.Uniform("view"), false, ref camera.View);
            GL.UniformMatrix4(Program.Uniform("projection"), false, ref camera.Projection);
            GL.Uniform4(Program.Uniform("color"), Color);
            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, Elements.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

    }
}
