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
        public Camera Camera;
        Mesh Screen;
        Mesh TvBox;
        Mesh BlackBars;
        PointLight Light1;

        long ElapsedMillis = 0;

        Texture ContentTexture;

        Fire8Bit FireEffect;

        Animator MonitorScaleAnimation = new Animator(.2f, 1.65f, 5000, false);
        Animator MonitorRotXAnimation = new Animator(-0.5f, 0.0f, 5000, false);
        Animator MonitorRotYAnimation = new Animator(-0.2f, 0.0f, 5000, false);
        Animator MonitorRotZAnimation = new Animator(0.1f, 0.0f, 5000, false);
        Animator MonitorScaleAnimation2 = new Animator(1.65f, 1.7f, 4000, false);

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
            Light1 = new PointLight();
            Light1.Position = new Vector3(-0.5f, -0.5f, 3.5f);
            Light1.Color = new Vector3(1.0f, 1.0f, 0.8f);
        }

        public void Init()
        {
            FireEffect.Init();
            CRTMonitorProgram.Use();            
            GL.Uniform1(GL.GetUniformLocation(CRTMonitorProgram.Id, "texContents"), ContentTexture.TextureUnit);
        }
                             
        public void Update(long deltaMillis)
        {
            ElapsedMillis += deltaMillis;

            MonitorScaleAnimation.Update(deltaMillis);
            MonitorRotXAnimation.Update(deltaMillis);
            MonitorRotYAnimation.Update(deltaMillis);
            MonitorRotZAnimation.Update(deltaMillis);

            Screen.ModelMatrix = Matrix4.CreateScale(MonitorScaleAnimation.Value);
            Screen.ModelMatrix *= Matrix4.CreateRotationX(MonitorRotXAnimation.Value);
            Screen.ModelMatrix *= Matrix4.CreateRotationY(MonitorRotYAnimation.Value);
            Screen.ModelMatrix *= Matrix4.CreateRotationZ(MonitorRotZAnimation.Value);
            Screen.ModelMatrix *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f));
            TvBox.ModelMatrix = Screen.ModelMatrix;

            BlackBars.ModelMatrix = Matrix4.CreateScale(2.0f);
            BlackBars.ModelMatrix *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, -2.0f));

            if (ElapsedMillis > 7000)
            {
                var alpha = Math.Max(0.0f, 1.0f - (ElapsedMillis - 7000) / 3000.0f);
                TvBox.Color.W = alpha;
                Screen.Color.W = alpha;
                MonitorScaleAnimation2.Update(deltaMillis);
                Screen.ModelMatrix = Matrix4.CreateScale(MonitorScaleAnimation2.Value);
                Screen.ModelMatrix *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f));
                TvBox.ModelMatrix = Screen.ModelMatrix;
            }
            if (ElapsedMillis > 11000)
            {
                var scale = 2.0f + (ElapsedMillis - 11000) / 3000.0f;
                BlackBars.ModelMatrix = Matrix4.CreateScale(scale);
                BlackBars.ModelMatrix *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, -2.0f));
            }
        }

        public void Render()
        {                       
            CRTMonitorProgram.Use();
            GL.BindTexture(TextureTarget.Texture2D, ContentTexture.Id);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Uniform3(CRTMonitorProgram.Uniform("lightPosition"), Light1.Position);
            GL.Uniform3(CRTMonitorProgram.Uniform("lightColor"), Light1.Color);
            GL.Uniform1(CRTMonitorProgram.Uniform("lightDiffuseIntensity"), Light1.Intensity);

            TvProgram.Use();
            GL.Uniform3(TvProgram.Uniform("lightPosition"), Light1.Position);
            GL.Uniform3(TvProgram.Uniform("lightColor"), Light1.Color);
            GL.Uniform1(TvProgram.Uniform("lightDiffuseIntensity"), Light1.Intensity);

            TvBox.Render(Camera);
            Screen.Render(Camera);
            BlackBars.Render(Camera);
        }             
    }
}
