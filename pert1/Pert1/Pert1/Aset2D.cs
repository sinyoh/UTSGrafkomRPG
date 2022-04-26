using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pert1
{
    internal class Aset2D
    {
       float[] _vertices =
       {

       };

        uint[] _indices = //bikin kotak dari urutan segitiga
        {

        };

        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject; //bantu indice
        Shader _shader;
        int index;
        int[] _pascal = { };

        public Aset2D()
        {
            _vertices = new float[1080];
            index = 0;
        }

        public Aset2D(float[] vertices, uint[] indices)
        {
            _vertices = vertices;
            _indices = indices; 
        }

        public void load(string shadervert, string shaderfrag)
        {
            // inisiasi buffer
            _vertexBufferObject = GL.GenBuffer();

            //kasih tau dia target yang diinginkan targetnya array
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            //kasih tau lebih detail : 
            //vertice length di kali sizeof(float) supaya jumlahnya menjadi
            //byte yang ada di dalam 1 array
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length
                * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            //ngembalikin habis dari float ke array lagi
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            //aturan
            //size: ada berapa vertex di array
            //ukuran: size of float
            //stride: di 1 vertex ada berapa titik (x,y,z) * ukuran
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            //0 referensi dari pointer
            GL.EnableVertexAttribArray(0);

            // ada data yang disimpan di indice
            if(_indices.Length != 0)
            {
                //inisialisasi indice (kotak)
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);
            }


            //"C:/Winston/Kuliah/semester 4/Grafkom/pert1/Pert1/Pert1/Shaders/shader.vert",
            //    "C:/Winston/Kuliah/semester 4/Grafkom/pert1/Pert1/Pert1/Shaders/shader.frag");

            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();
        }

        public void render(int _lines)
        {

            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);

            if (_indices.Length != 0)
            {
                //kotak
                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if(_lines == 0)
                {
                    //segitiga
                    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                }else if (_lines == 1)
                { 
                    //circle
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, (_vertices.Length + 1) / 3);
                }else if (_lines == 2)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, index);
                }
                else if(_lines == 3)
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, (_vertices.Length + 1) / 3);
                }
            }
        }

        public void createCircle(float center_x, float center_y, float radius)
        {
            _vertices = new float[1080];
            for (int i = 0; i<360; i++)
            {
                double degInRad = i * Math.PI / 180;
                _vertices[i * 3] = radius * (float)Math.Cos(degInRad) + center_x;
                _vertices[i * 3 + 1] = radius * (float)Math.Sin(degInRad) + center_y;
                _vertices[i * 3 + 2] = 0;
            }
        }

        public void createEllipse(float center_x, float center_y, float radius_x, float radius_y)
        {
            _vertices = new float[1080];
            for (int i = 0; i < 360; i++)
            {
                double degInRad = i * Math.PI / 180;
                _vertices[i * 3] = radius_x * (float)Math.Cos(degInRad) + center_x;
                _vertices[i * 3 + 1] = radius_y  * (float)Math.Sin(degInRad) + center_y;
                _vertices[i * 3 + 2] = 0;
            }
        }

        public void updateMousePosition(float x, float y, float z)
        {
            _vertices[index * 3] = x;
            _vertices[index * 3 + 1] = y;
            _vertices[index * 3 + 2] = z;
            index++;

            GL.BufferData(BufferTarget.ArrayBuffer, index * 3 * sizeof(float),
                _vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
        public List<int> getRow(int rowIndex)
        {
            List<int> currow = new List<int>();
            //------
            currow.Add(1);
            if (rowIndex == 0)
            {
                return currow;
            }
            //-----
            List<int> prev = getRow(rowIndex - 1);
            for (int i = 1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            currow.Add(1);
            return currow;
        }
        public List<float> CreateCurveBezier()
        {
            List<float> _vertices_bezier = new List<float>();
            List<int> pascal = getRow(index - 1);
            _pascal = pascal.ToArray();
            for (float t = 0; t <= 1.0f; t += 0.01f)
            {
                Vector2 p = getP(index, t);
                _vertices_bezier.Add(p.X);
                _vertices_bezier.Add(p.Y);
                _vertices_bezier.Add(0);
            }
            return _vertices_bezier;
        }
        public Vector2 getP(int n, float t)
        {
            Vector2 p = new Vector2(0, 0);
            float[] k = new float[n];
            for (int i = 0; i < n; i++)
            {
                k[i] = (float)Math.Pow((1 - t), n - 1 - i)
                    * (float)Math.Pow(t, i) * _pascal[i];
                p.X += k[i] * _vertices[i * 3];
                p.Y += k[i] * _vertices[i * 3 + 1];
            }
            return p;
        }

        public void setVertices(float[] vertices)
        {
            _vertices = vertices;
        }

        public bool getVerticesLength()
        {
            if (_vertices[0] == 0)
            {
                return false;
            }
            if ((_vertices.Length + 1) / 3 > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void createClock(float _s, float _m, float _h)
        {
            int _hour = DateTime.Now.Hour;
            int _minute = DateTime.Now.Minute;
            int _second = DateTime.Now.Second;

            if (_hour >= 12)
                _hour -= 12;
            if (_minute >= 60)
                _minute -= 60;
            if (_second >= 60)
                _second -= 60;

            for (int i = 0; i < 18; i++)
            {
                _vertices[i] = 0;
            }

            _vertices[3] = _s * (float)Math.Sin(_second * 6 * Math.PI / 180);
            _vertices[4] = _s * (float)Math.Cos(_second * 6 * Math.PI / 180);

            _vertices[9] = _m * (float)Math.Sin((_minute) * 6 * Math.PI / 180);
            _vertices[10] = _m * (float)Math.Cos((_minute) * 6 * Math.PI / 180);

            _vertices[15] = _h * (float)Math.Sin((_hour) * 30 * Math.PI / 180);
            _vertices[16] = _h * (float)Math.Cos((_hour) * 30 * Math.PI / 180);

            index = 18;
            GL.BufferData(BufferTarget.ArrayBuffer, 18 * 3 * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
        }

    }
}
