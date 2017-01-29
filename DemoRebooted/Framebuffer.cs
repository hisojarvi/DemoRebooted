using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class Framebuffer
    {

        public int FBO;
        public Texture Texture;
        public int Width;
        public int Height;
        int[] ScreenViewport;
        

        public Framebuffer(int w, int h, int texUnit)
        {
            Width = w;
            Height = h;
            Texture = new Texture(texUnit);
            ScreenViewport = new int[4];
            Init();
        }

        void Init()
        {
            FBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            GL.ActiveTexture(TextureUnit.Texture0 + Texture.TextureUnit);
            GL.BindTexture(TextureTarget.Texture2D, Texture.Id);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                          Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                                    TextureTarget.Texture2D, Texture.Id, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind()
        {
            GL.GetInteger(GetPName.Viewport, ScreenViewport);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            GL.Viewport(0, 0, Width, Height);
        }

        public void Unbind()
        {            
            GL.Viewport(ScreenViewport[0], ScreenViewport[1], ScreenViewport[2], ScreenViewport[3]);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
    }
}
