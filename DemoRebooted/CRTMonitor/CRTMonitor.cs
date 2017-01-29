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
        public int Width { get; }
        public int Height { get; }

        ShaderProgram CRTMonitorProgram;
        ShaderProgram TvProgram;
        Camera Camera;
        Mesh Screen;
        Mesh TvBox;
        Mesh BlackBars;

        Texture ContentTexture;

        Fire8Bit FireEffect;

        Animator MonitorScaleAnimation = new Animator(.2f, 1.65f, 5000, false);

        public CRTMonitor(int w, int h, Texture contentTex)
        {
            Width = w;
            Height = h;
            ContentTexture = contentTex;
            FireEffect = new Fire8Bit(320, 200);
            CRTMonitorProgram = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.CRTMonitor.3dProjection.vert"),
                                        ResourceUtils.ReadResourceFile("DemoRebooted.CRTMonitor.CRTMonitor.frag"));
            TvProgram = new ShaderProgram(ResourceUtils.ReadResourceFile("DemoRebooted.CRTMonitor.3dProjection.vert"),
                                        ResourceUtils.ReadResourceFile("DemoRebooted.CRTMonitor.LightModel.frag"));

            Camera = new DemoRebooted.Camera();
            
            var parser = new ObjParser.ObjParser(ResourceUtils.ReadResourceFile("DemoRebooted.Resources.oldtv.obj"));
            var o = parser.Object("Grid_Screen");
            Screen = new Mesh(o.GetVertexData(), o.GetElementData(), CRTMonitorProgram);
            o = parser.Object("TV_TvBox");
            TvBox = new Mesh(o.GetVertexData(), o.GetElementData(), TvProgram);
            TvBox.Color = new Vector4(.2f, .2f, .2f, 1.0f);
            o = parser.Object("BlackBars");
            BlackBars = new Mesh(o.GetVertexData(), o.GetElementData(), TvProgram);
            BlackBars.Color = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
        }

        public void Init()
        {
            FireEffect.Init();
            CRTMonitorProgram.Use();
            
            GL.Uniform1(GL.GetUniformLocation(CRTMonitorProgram.Id, "texContents"), ContentTexture.TextureUnit);
        }
                             
        float rot = 0.0f;
        public void Update(long deltaMillis)
        {
            rot += 0.0f;
            MonitorScaleAnimation.Update(deltaMillis);
            Screen.ModelMatrix = Matrix4.CreateScale(MonitorScaleAnimation.Value);
            Screen.ModelMatrix *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f));
            Screen.ModelMatrix *= Matrix4.CreateRotationY(rot);
            TvBox.ModelMatrix = Screen.ModelMatrix;

            BlackBars.ModelMatrix = Matrix4.CreateScale(2.0f);
            BlackBars.ModelMatrix *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, -2.0f));
        }

        public void Render()
        {                       
            CRTMonitorProgram.Use();
            GL.BindTexture(TextureTarget.Texture2D, ContentTexture.Id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            TvBox.Render(Camera);
            Screen.Render(Camera);
            BlackBars.Render(Camera);
        }             
    }
}
