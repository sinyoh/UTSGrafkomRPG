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
    internal class Aset3D
    {
        List<Vector3> _vertices = new List<Vector3>();
       
        List<uint> _indices = new List<uint>(); 
        int _vertexBufferObject;
        int _vertexArrayObject;
        int _elementBufferObject; //bantu indice
        Shader _shader;
        //Matrix4 _view; //camera
        //Matrix4 _projection; //settingan kamera
        Matrix4 _model; //ngrubah transformasi

        int[] _pascal = { };
        public Vector3 _centerPosition;
        public List<Vector3> _euler;
        public List<Aset3D> child;
        List<Vector3> _vertice_bezier;

        public Aset3D()
        {
            _vertices = new List<Vector3>();
            setDefault();
        }

        public void setDefault()
        {
            _euler = new List<Vector3>();

            _euler.Add(new Vector3(1, 0, 0));
            _euler.Add(new Vector3(0, 1, 0));
            _euler.Add(new Vector3(0, 0, 1));
            _model = Matrix4.Identity; //1 0 0   0 1 0   0 0 1

            _centerPosition = new Vector3(0 , 0, 0);

            child = new List<Aset3D>();

            _vertice_bezier = new List<Vector3>();

        }


        public Aset3D(List<Vector3> vertices, List<uint> indices)
        {
            _vertices = vertices;
            _indices = indices;
            setDefault();
        }

        public void load(string shadervert, string shaderfrag, float Size_x, float Size_y)
        {

            // inisiasi buffer
            _vertexBufferObject = GL.GenBuffer();

            //kasih tau dia target yang diinginkan targetnya array
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            //kasih tau lebih detail : 
            //vertice length di kali sizeof(float) supaya jumlahnya menjadi
            //byte yang ada di dalam 1 array
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count
                * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);

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
            if(_indices.Count != 0)
            {
                //inisialisasi indice (kotak)
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, 
                    _indices.Count * sizeof(uint), _indices.ToArray(), BufferUsageHint.StaticDraw);
            }


            //"C:/Winston/Kuliah/semester 4/Grafkom/pert1/Pert1/Pert1/Shaders/shader.vert",
            //    "C:/Winston/Kuliah/semester 4/Grafkom/pert1/Pert1/Pert1/Shaders/shader.frag");


            _shader = new Shader(shadervert, shaderfrag);
            _shader.Use();

            //_view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);
            //_projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size_x / (float)Size_y, 0.1f, 100.0f);

            foreach(var item in child)
            {
                item.load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }

        public void render(int _lines, Matrix4 temp, Matrix4 camera_view, Matrix4 camera_projection)
        {

            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);


            _shader.SetMatrix4("model", _model);
            _shader.SetMatrix4("view", camera_view);
            _shader.SetMatrix4("projection", camera_projection);

            if (_indices.Count != 0)
            {
                //kotak
                GL.DrawElements(PrimitiveType.Triangles, _indices.Count, DrawElementsType.UnsignedInt, 0);
            }
            else
            {
                if (_lines == 0) //gambar dengan isi DEFAULT
                {
                    GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
                }
                else if (_lines == 1) //gambar garis tpi gak sampai semua
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, _vertices.Count);
                }
                else if (_lines == 2) // gambar tanpa isi
                {
                    GL.DrawArrays(PrimitiveType.LineLoop, 0, _vertices.Count);
                }
                else if (_lines == 3)
                {
                    //lingkaran line
                    //GL.DrawArrays(PrimitiveType.LineLoop, 0, _vertices.Count);

                    //lingkaran berisi
                    GL.DrawArrays(PrimitiveType.TriangleFan, 0, _vertices.Count);

                }
            }

            foreach (var item in child)
            {
                item.render(_lines, temp, camera_view, camera_projection);
            }
        }
        public void createBoxVertices(float x, float y, float z, float length, float uk_x, float uk_y, float uk_z)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            //kalau x dibesarin jadi persegi panjang horizontal
            //kalau y dibesarin jadi persegi panjang vertical

            Vector3 temp_vector;

            float _x = uk_x;
            float _y = uk_y;
            float _z = uk_z;

            //TITIK 1
            temp_vector.X = x - length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x + length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x - length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x + length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x - length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {

                 //front
                0, 7, 3,
                0, 4, 7,
                //back
                1, 2, 6,
                6, 5, 1,
                //left
                0, 2, 1,
                0, 3, 2,
                //right
                4, 5, 6,
                6, 7, 4,
                //top
                2, 3, 6,
                6, 3, 7,
                //bottom
                0, 1, 5,
                0, 5, 4

            };

        }


        //linestrip
        public void createEllipsoid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {

            _centerPosition.X = _x;
            _centerPosition.Y = _y;
            _centerPosition.Z = _z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for(float u = -pi; u <= pi; u += pi / 700)
            {
                for(float v=-pi/2; v<=pi/2; v += pi / 700)
                {
                    temp_vector.X = _x + (float)Math.Cos(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)Math.Cos(v) * (float)Math.Sin(u) * radiusY;                  
                    temp_vector.Z = _z + (float)Math.Sin(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createHyperboloid1(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 300)
                {
                    temp_vector.X = _x + (float)(1 / Math.Cos(v)) * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + (float)(1 / Math.Cos(v)) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + (float)Math.Tan(v) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createHyperboloidDuaSisi(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float u = -pi / 2; u <= pi / 2; u += pi / 30)//u range di tabel
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 30) //v range di tabel, pembagi semakin besar, garis semakin banyak
                {
                    temp_vector.Z = _x + (float)Math.Tan(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.X = _y + (float)Math.Tan(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Y = _z + (1 / (float)Math.Cos(v)) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
            for (float u = pi / 2; u <= 3 * (pi / 2); u += pi / 30)//u range di tabel
            {
                for (float v = -pi / 2; v <= pi / 2; v += pi / 30) //v range di tabel, pembagi semakin besar, garis semakin banyak
                {
                    temp_vector.Z = _x + (float)Math.Tan(v) * (float)Math.Cos(u) * radiusX;
                    temp_vector.X = _y + (float)Math.Tan(v) * (float)Math.Sin(u) * radiusY;
                    temp_vector.Y = _z + (1 / (float)Math.Cos(v)) * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createEllipCone(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, float _v, float _range_v)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 600)
            {
                for (float v = _v; v <= _range_v; v += pi / 600)
                {
                    temp_vector.X = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + v * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }

        }
        public void createTorus(float center_x, float center_y, float center_z, float r1, float r2)
        {
            _centerPosition.X = center_x;
            _centerPosition.Y = center_y;
            _centerPosition.Z = center_z;

            float pi = (float)Math.PI;
            Vector3 temp_vector;

            for (float u = 0; u <= 2 * pi; u += pi / 700)
            {
                for (float v = 0; v <= 2 * pi; v += pi / 700)
                {
                    temp_vector.X = center_x + (r1 + r2 * (float)Math.Cos(v)) * (float)Math.Cos(u);
                    temp_vector.Y = center_y + (r1 + r2 * (float)Math.Cos(v)) * (float)Math.Sin(u);
                    temp_vector.Z = center_z + r2 * (float)Math.Sin(v);
                    _vertices.Add(temp_vector);
                }
            }
        }
        public void createEllipParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, float _v)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = _v; v >= 0; v -= pi / 300)
                {
                    temp_vector.X = _x + v * (float)Math.Cos(u) * radiusX;
                    temp_vector.Y = _y + v * (float)Math.Sin(u) * radiusY;
                    temp_vector.Z = _z + v * v * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        public void createHyperParaboloid(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, float _v)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            for (float u = -pi; u <= pi; u += pi / 300)
            {
                for (float v = _v; v >= 0; v -= pi / 300)
                {
                    temp_vector.X = _x + v * (float)Math.Tan(u) * radiusX;
                    temp_vector.Y = _y + v * (float)(1 / Math.Cos(u)) * radiusY;
                    temp_vector.Z = _z + v * v * radiusZ;
                    _vertices.Add(temp_vector);
                }
            }
        }

        //draw element
        public void createEllipsoid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radiusX * (float)Math.Cos(StackAngle);
                y = radiusY * (float)Math.Cos(StackAngle);
                z = radiusZ * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle);
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle);
                    temp_vector.Z = z;
                    _vertices.Add(temp_vector);
                }
            }

            uint k1, k2;
            for (int i = 0; i < stackCount; ++i)
            {
                k1 = (uint)(i * (sectorCount + 1));
                k2 = (uint)(k1 + sectorCount + 1);
                for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
                {
                    if (i != 0)
                    {
                        _indices.Add(k1);
                        _indices.Add(k2);
                        _indices.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        _indices.Add(k1 + 1);
                        _indices.Add(k2);
                        _indices.Add(k2 + 1);
                    }
                }
            }
        }

         public void createTabung(float _positionX ,float _positionY,float _positionZ,float _radius, float _height)
        {
            _centerPosition.X = _positionX; //jgn lupa selalu tambahkan ini
            _centerPosition.Y = _positionY;
            _centerPosition.Z = _positionZ;

            Vector3 temp_vector;
            float _pi = (float)Math.PI;


            for (float v = -_height / 2; v <= (_height / 2); v += 0.0001f)
            {
                Vector3 p = setBeizer((v + (_height / 2)) / _height);
                for (float u = -_pi; u <= _pi; u += (_pi / 30))
                {

                    temp_vector.X = p.X + _radius * (float)Math.Cos(u);
                    temp_vector.Y = p.Y + _radius * (float)Math.Sin(u);
                    temp_vector.Z = _positionZ + v;

                    _vertices.Add(temp_vector);

                }
            }



        }

        Vector3 setBeizer(float t)
        {
            //Console.WriteLine(t);
            Vector3 p = new Vector3(0f, 0f, 0f);
            float[] k = new float[3];

            k[0] = (float)Math.Pow((1 - t), 3 - 1 - 0) * (float)Math.Pow(t, 0) * 1;
            k[1] = (float)Math.Pow((1 - t), 3 - 1 - 1) * (float)Math.Pow(t, 1) * 2;
            k[2] = (float)Math.Pow((1 - t), 3 - 1 - 2) * (float)Math.Pow(t, 2) * 1;


            //titik 1
            p.X += k[0] * _centerPosition.X;
            p.Y += k[0] * _centerPosition.Y;

            //titik 2
            p.X += k[1] * (_centerPosition.X);
            p.Y += k[1] * _centerPosition.Y;

            //titik 3
            p.X += k[2] * _centerPosition.X;
            p.Y += k[2] * _centerPosition.Y;

            //Console.WriteLine(p.X + " "+ p.Y);

            return p;

        }

        public Vector3 getRotationResult(Vector3 pivot, Vector3 vector, float angle, Vector3 point, bool isEuler = false)
        {
            Vector3 temp, newPosition;

            if (isEuler)
            {
                temp = point;
            }
            else
            {
                temp = point - pivot;
            }

            newPosition.X =
                temp.X * (float)(Math.Cos(angle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Y * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) - vector.Z * Math.Sin(angle)) +
                temp.Z * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) + vector.Y * Math.Sin(angle));

            newPosition.Y =
                temp.X * (float)(vector.X * vector.Y * (1.0f - Math.Cos(angle)) + vector.Z * Math.Sin(angle)) +
                temp.Y * (float)(Math.Cos(angle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(angle))) +
                temp.Z * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) - vector.X * Math.Sin(angle));

            newPosition.Z =
                temp.X * (float)(vector.X * vector.Z * (1.0f - Math.Cos(angle)) - vector.Y * Math.Sin(angle)) +
                temp.Y * (float)(vector.Y * vector.Z * (1.0f - Math.Cos(angle)) + vector.X * Math.Sin(angle)) +
                temp.Z * (float)(Math.Cos(angle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(angle)));

            if (isEuler)
            {
                temp = newPosition;
            }
            else
            {
                temp = newPosition + pivot;
            }
            return temp;
        }

        public void rotate(Vector3 pivot, Vector3 vector, float angle)
        {
            var radAngle = MathHelper.DegreesToRadians(angle);

            var arbRotationMatrix = new Matrix4
                (
                new Vector4((float)(Math.Cos(radAngle) + Math.Pow(vector.X, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) + vector.Z * Math.Sin(radAngle)), (float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.Y * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Y * (1.0f - Math.Cos(radAngle)) - vector.Z * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Y, 2.0f) * (1.0f - Math.Cos(radAngle))), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.X * Math.Sin(radAngle)), 0),
                new Vector4((float)(vector.X * vector.Z * (1.0f - Math.Cos(radAngle)) + vector.Y * Math.Sin(radAngle)), (float)(vector.Y * vector.Z * (1.0f - Math.Cos(radAngle)) - vector.X * Math.Sin(radAngle)), (float)(Math.Cos(radAngle) + Math.Pow(vector.Z, 2.0f) * (1.0f - Math.Cos(radAngle))), 0),
                Vector4.UnitW
                );

            _model *= Matrix4.CreateTranslation(-pivot);
            _model *= arbRotationMatrix;
            _model *= Matrix4.CreateTranslation(pivot);

            for (int i = 0; i < 3; i++)
            {
                _euler[i] = Vector3.Normalize(getRotationResult(pivot, vector, radAngle, _euler[i], true));
            }

             //= getRotationResult(pivot, vector, radAngle, rotationCenter);
            _centerPosition = getRotationResult(pivot, vector, radAngle, _centerPosition);

            foreach (var i in child)
            {
                i.rotate(pivot, vector, angle);
            }
        }

        public void trans(float x, float y, float z)
        {
            _model = _model * Matrix4.CreateTranslation(x, y, z);

            foreach (var i in child)
            {
                i.trans(x,y,z);
            }
        }

        //untuk mengatur ukuran objek
        public void scale(float x)
        {

            _model = _model * Matrix4.CreateTranslation(-1 * (_centerPosition)) * Matrix4.CreateScale(x) * Matrix4.CreateTranslation((_centerPosition));
            foreach (var i in child)
            {
                i.scale(x);
            }
        }   

        public void addChildCone(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, float _v, float _range_v)
        {
            Aset3D newChild = new Aset3D();
            newChild.createEllipCone(radiusX, radiusY, radiusZ, _x, _y, _z, _v, _range_v);
            child.Add(newChild);
        }

        public void createBodyKnight(float x, float y, float z, float length, float uk_x, float uk_y, float uk_z)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            //kalau x dibesarin jadi persegi panjang horizontal
            //kalau y dibesarin jadi persegi panjang vertical

            Vector3 temp_vector;

            float _x = uk_x;
            float _y = uk_y;
            float _z = uk_z;

            //TITIK 1
            temp_vector.X = x - length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x + length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x - length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x + length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x - length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {

                 //front
                0, 7, 3,
                0, 4, 7,
                //back
                1, 2, 6,
                6, 5, 1,
                //left
                0, 2, 1,
                0, 3, 2,
                //right
                4, 5, 6,
                6, 7, 4,
                //top
                2, 3, 6,
                6, 3, 7,
                //bottom
                0, 1, 5,
                0, 5, 4

            };

        }

        public void addSlimBox(float x, float y, float z, float length, float uk_x, float uk_y, float uk_z)
        {
            Aset3D newChild = new Aset3D();
            newChild.createBoxVertices(x, y, z, length, uk_x, uk_y, uk_z);
            child.Add(newChild);
        }
        public void addEllipsoidChild(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            Aset3D newChild = new Aset3D();
            newChild.createEllipsoid(radiusX, radiusY, radiusZ, _x, _y, _z);
            child.Add(newChild);
        }

        public void addTabungChild(float _positionX, float _positionY, float _positionZ, float _radius, float _height)
        {
            Aset3D newChild = new Aset3D();
            newChild.createTabung(_positionX, _positionY, _positionZ, _radius, _height);
            child.Add(newChild);
        }


        public void addTorusChild(float center_x, float center_y, float center_z, float r1, float r2)
        {
            Aset3D newChild = new Aset3D();
            newChild.createTorus(center_x, center_y, center_z, r1, r2);
            child.Add(newChild);
        }

        public void addHyper2Child(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z)
        {
            Aset3D newChild = new Aset3D();
            newChild.createHyperboloidDuaSisi(radiusX, radiusY, radiusZ, _x, _y, _z);
            child.Add(newChild);
        }

        //berzier
        public List<int> getRow(int rowIndex)
        {
            List<int> currow = new List<int>();
            currow.Add(1);
            if (rowIndex == 0)
            {
                return currow;
            }
            List<int> prev = getRow(rowIndex - 1);
            for (int i = 1; i < prev.Count; i++)
            {
                int curr = prev[i - 1] + prev[i];
                currow.Add(curr);
            }
            currow.Add(1);
            return currow;
        }
        public void CreateCurveBezier(List<Vector3> titik_kontrol)
        {
            List<int> pascal = getRow(titik_kontrol.Count - 1);
            _pascal = pascal.ToArray();
            for (float t = 0; t <= 1.0f; t += 0.01f)
            {
                Vector3 p = getP(t, titik_kontrol);
                _vertices.Add(p);
            }
        }
        public Vector3 getP(float t, List<Vector3> titik_kontrol)
        {
            Vector3 p = Vector3.Zero;
            float k;
            for (int i = 0; i < titik_kontrol.Count; i++)
            {
                k = (float)Math.Pow((1 - t), (titik_kontrol.Count - 1 - i)) * (float)Math.Pow(t, i) * _pascal[i];
                p += titik_kontrol[i] * k;
            }
            return p;
        }

        public void createPersegi(float x, float y, float z, float length, float uk_x, float uk_y, float uk_z)
        {
            _centerPosition.X = x;
            _centerPosition.Y = y;
            _centerPosition.Z = z;

            //kalau x dibesarin jadi persegi panjang horizontal
            //kalau y dibesarin jadi persegi panjang vertical

            Vector3 temp_vector;

            float _x = uk_x;
            float _y = uk_y;
            float _z = uk_z;

            //TITIK 1
            temp_vector.X = x - length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 2
            temp_vector.X = x + length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 3
            temp_vector.X = x + length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 4
            temp_vector.X = x - length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z - length / _z;
            _vertices.Add(temp_vector);
            //TITIK 5
            temp_vector.X = x - length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 6
            temp_vector.X = x + length / _x;
            temp_vector.Y = y - length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 7
            temp_vector.X = x + length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);
            //TITIK 8
            temp_vector.X = x - length / _x;
            temp_vector.Y = y + length / _y;
            temp_vector.Z = z + length / _z;
            _vertices.Add(temp_vector);

            _indices = new List<uint>
            {
                //bottom
                0, 1, 5,
                0, 5, 4

            };

        }

    }
}   
