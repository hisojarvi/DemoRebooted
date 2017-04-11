using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;
using System.Drawing.Imaging;

namespace DemoRebooted.Sky
{
    public class SkyBoxEffect
    {

        ShaderProgram SkyBoxProgram;
        Mesh SkyBox;
        public Camera Camera;
        Texture SkyBoxTexture;
        public float Brightness = 1.0f;

        public SkyBoxEffect(Camera camera)
        {
            SkyBoxProgram = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.Sky.SkyBoxEffect.vert"),
                                              ResourceUtils.ReadResourceFile("DemoRebooted.Sky.SkyBoxEffect.frag"));

            var parser = new ObjParser.ObjParser(ResourceUtils.ReadResourceFile("DemoRebooted.Resources.skybox.obj"));
            var o = parser.Object("Cube");
            SkyBox = new Mesh(o.GetVertexData(), o.GetElementData(), SkyBoxProgram);
            SkyBox.ModelMatrix = Matrix4.CreateScale(2.0f);
            Camera = camera;            
        }

        public void Init()
        {
            InitTextures();
        }

        void InitTextures()
        {
            GL.Enable(EnableCap.TextureCubeMap);

            SkyBoxTexture = new Texture(0);
            SkyBoxTexture.Activate();
            GL.BindTexture(TextureTarget.TextureCubeMap, SkyBoxTexture.Id);

            LoadCubeMapSide(TextureTarget.TextureCubeMapNegativeX, "DemoRebooted.Resources.StarSkyBoxX-.png");
            LoadCubeMapSide(TextureTarget.TextureCubeMapPositiveX, "DemoRebooted.Resources.StarSkyBoxX+.png");
            LoadCubeMapSide(TextureTarget.TextureCubeMapNegativeY, "DemoRebooted.Resources.StarSkyBoxY-.png");
            LoadCubeMapSide(TextureTarget.TextureCubeMapPositiveY, "DemoRebooted.Resources.StarSkyBoxY+.png");
            LoadCubeMapSide(TextureTarget.TextureCubeMapNegativeZ, "DemoRebooted.Resources.StarSkyBoxZ-.png");
            LoadCubeMapSide(TextureTarget.TextureCubeMapPositiveZ, "DemoRebooted.Resources.StarSkyBoxZ+.png");

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureParameterName.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);

            SkyBoxProgram.Use();
            GL.Uniform1(SkyBoxProgram.Uniform("skyboxTexture"), SkyBoxTexture.TextureUnit);
        }

        void LoadCubeMapSide(TextureTarget side, string resourceName)
        {
            Bitmap texBitmap = ResourceUtils.ReadResourceImage(resourceName);
            BitmapData data = texBitmap.LockBits(new System.Drawing.Rectangle(0, 0, texBitmap.Width, texBitmap.Height),
                                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(side, 0, PixelInternalFormat.Rgba, texBitmap.Width, texBitmap.Height, 0,
                          OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            texBitmap.UnlockBits(data);
            texBitmap.Dispose();
        }

        public void Update(long elapsedMillis)
        {

        }

        public void Render()
        {
            SkyBoxProgram.Use();
            var ModelMatrix = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
            GL.UniformMatrix4(SkyBoxProgram.Uniform("model"), false, ref ModelMatrix);
            GL.UniformMatrix4(SkyBoxProgram.Uniform("view"), false, ref Camera.View);
            GL.UniformMatrix4(SkyBoxProgram.Uniform("projection"), false, ref Camera.Projection);
            GL.Uniform1(SkyBoxProgram.Uniform("brightness"), Brightness);
            SkyBox.Render(Camera);
        }
    }
}
