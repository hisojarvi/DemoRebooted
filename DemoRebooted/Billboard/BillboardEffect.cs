using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted.Billboard
{
    public class BillboardEffect
    {
        int Width;
        int Height;
        Texture Texture;
        ShaderProgram Program;
        Mesh Canvas;
        Camera Camera;

        float[] CanvasVertices = { -1.0f,  1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, -1.0f,
                                    1.0f,  1.0f, 0.0f, 1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
                                    1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, -1.0f,
                                   -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f };

        int[] CanvasElements = { 0, 2, 1,
                                 0, 3, 2 };

        public BillboardEffect(int w, int h, Texture texture)
        {
            Width = w;
            Height = h;
            Texture = texture;
            Program = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.Billboard.Billboard.vert"),
                                        ResourceUtils.ReadResourceFile("DemoRebooted.Billboard.Billboard.frag"));
            Canvas = new Mesh(CanvasVertices, CanvasElements, Program);
            Camera = new Camera();
            Init();
        }

        void Init()
        {
            Program.Use();
            GL.Uniform1(Program.Uniform("billboardTex"), Texture.TextureUnit);
        }

        public void Render()
        {
            Texture.Activate();
            GL.BindTexture(TextureTarget.Texture2D, Texture.Id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            Canvas.Render(Camera);
        }
    }
}
