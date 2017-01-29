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

        int ContentFBO;
        int ContentTexture;

        Fire8Bit FireEffect;

        Animator MonitorScaleAnimation = new Animator(.2f, 1.3f, 2000, false);

        public CRTMonitor(int w, int h)
        {
            Width = w;
            Height = h;
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
            TvBox.Color = new Vector4(.3f, .3f, .3f, .7f);
        }

        public void Init()
        {
            FireEffect.Init();
            CRTMonitorProgram.Use();            
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

            GL.Uniform1(GL.GetUniformLocation(CRTMonitorProgram.Id, "texContents"), 2);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return fbo;
        }
        
        float rot = -0.2f;
        public void Update(long deltaMillis)
        {
            rot += 0.0f;
            MonitorScaleAnimation.Update(deltaMillis);
            Screen.ModelMatrix = Matrix4.CreateScale(MonitorScaleAnimation.Value);
            Screen.ModelMatrix *= Matrix4.CreateTranslation(new Vector3(0.0f, 0.0f, 0.0f));
            Screen.ModelMatrix *= Matrix4.CreateRotationY(rot);
            TvBox.ModelMatrix = Screen.ModelMatrix;

        }

        public void Render()
        {             
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            GL.Viewport(0, 0, FireEffect.Width, FireEffect.Height);
            RenderContent();
            GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);
            
            CRTMonitorProgram.Use();
            GL.Enable(EnableCap.Blend);            
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            TvBox.Render(Camera);
            Screen.Render(Camera);
            
        }
        
        void RenderContent()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, ContentFBO);
            FireEffect.Render();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);  
        }
        
    }
}
