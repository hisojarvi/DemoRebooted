﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace DemoRebooted
{
    public static class ResourceUtils
    {

        public static string ReadResourceFile(string fileName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static Bitmap ReadResourceImage(string fileName)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames();
            using (Stream stream = assembly.GetManifestResourceStream(fileName))
            {
                return new Bitmap(stream);
            }
        }

    }
}
