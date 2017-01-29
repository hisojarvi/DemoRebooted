using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace DemoRebooted
{
    public class Texture
    {
        public int Id;
        public int TextureUnit;

        public Texture(int textureUnit)
        {
            Id = GL.GenTexture();
            TextureUnit = textureUnit;
        }
    }
}
