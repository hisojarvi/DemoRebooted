using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRebooted.ObjParser
{


    class LineProvider
    {
        string[] Lines;
        int Index = 0;

        public LineProvider(string objFile)
        {
            Lines = objFile.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
        }

        public string NextLine()
        {
            string value = null;
            if (HasNextLine())
            {
                value = Lines[Index];
                Index++;
            }
            return value;
        }

        public string PeekNextLine()
        {
            string value = null;
            if (HasNextLine())
            {
                value = Lines[Index];
            }
            return value;
        }

        public bool HasNextLine()
        {
            return Index < Lines.Length;
        }
    }

    public class ObjObject
    {
        public string Name;
        public string Material;

        List<float> Vertices;
        List<float> VertexNormals;
        List<float> TexCoords;
        public List<int> VertexIndices = new List<int>();
        public List<int> TextureIndices = new List<int>();
        public List<int> NormalIndices = new List<int>();

        public ObjObject(List<float> verts, List<float> normals, List<float> texcoords)
        {
            Vertices = verts;
            VertexNormals = normals;
            TexCoords = texcoords;
        }

        public float[] GetVertexData()
        {
            List<float> dataList = new List<float>();
            for (var i = 0; i < VertexIndices.Count; i++)
            {
                var vertex_i = 3 * (VertexIndices[i]-1);
                var texture_i = 2 * (TextureIndices[i]-1);
                var normal_i = 3 * (NormalIndices[i]-1);
                dataList.Add(Vertices[vertex_i]); dataList.Add(Vertices[vertex_i + 1]); dataList.Add(Vertices[vertex_i + 2]);
                dataList.Add(TexCoords[texture_i]); dataList.Add(TexCoords[texture_i + 1]);
                dataList.Add(VertexNormals[normal_i]); dataList.Add(VertexNormals[normal_i + 1]); dataList.Add(VertexNormals[normal_i + 2]);
            }
            return dataList.ToArray();
        }

        public int[] GetElementData()
        {
            int[] data = new int[VertexIndices.Count];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = i;
            }
            return data;
        }
    }

    public class ObjParser
    {
        LineProvider Lines;
        public List<float> Vertices = new List<float>();
        public List<float> VertexNormals = new List<float>();
        public List<float> TexCoords = new List<float>();
        public List<ObjObject> Objects = new List<ObjObject>();

        public ObjParser(string objFile)
        {
            Lines = new LineProvider(objFile);
            Parse();
        }

        void Parse()
        {
            while (Lines.HasNextLine())
            {
                ParseLine(Lines.NextLine());
            }
        }

        public ObjObject Object(string name)
        {
            return Objects.Find(x => x.Name == name);
        }

        void ParseLine(string line)
        {
            line = line.TrimStart(null);
            if (line.StartsWith("#"))
            {
                //Comment, do nothing
            }
            else if (line.StartsWith("mtllib"))
            {
                // Materials library, ignore
            }
            else if (line.StartsWith("o "))
            {
                Objects.Add(ParseObject(line));
            }
            else
            {
                throw new System.ArgumentException("Can't parse line: " + line);
            }
        }

        ObjObject ParseObject(string headerLine)
        {
            var o = new ObjObject(Vertices, VertexNormals, TexCoords);
            o.Name = TokenizeLine(headerLine)[1];
            while (Lines.HasNextLine() && !Lines.PeekNextLine().StartsWith("o "))
            {
                ParseObjectLine(o, Lines.NextLine());
            }
            return o;
        }

        void ParseObjectLine(ObjObject o, string line)
        {
            var tokens = TokenizeLine(line);
            if (tokens[0] == "v")
            {
                for (var i = 1; i < tokens.Length; i++)
                {                    
                    Vertices.Add(float.Parse(tokens[i], CultureInfo.InvariantCulture));
                }
            }
            else if (tokens[0] == "vt")
            {
                for (var i = 1; i < tokens.Length; i++)
                {
                    TexCoords.Add(float.Parse(tokens[i], CultureInfo.InvariantCulture));
                }
            }
            else if (tokens[0] == "vn")
            {
                for (var i = 1; i < tokens.Length; i++)
                {
                    VertexNormals.Add(float.Parse(tokens[i], CultureInfo.InvariantCulture));
                }
            }
            else if (tokens[0] == "f")
            {
                for (var i = 1; i < tokens.Length; i++)
                {
                    ParseFaceElement(o, tokens[i]);
                }
            }
        }

        void ParseFaceElement(ObjObject o, string faceElement)
        {
            var components = faceElement.Split('/');
            o.VertexIndices.Add(int.Parse(components[0]));
            o.TextureIndices.Add(int.Parse(components[1]));
            o.NormalIndices.Add(int.Parse(components[2]));
        }

        string[] TokenizeLine(string line)
        {
            return line.Split(' ');
        }
    }
}
