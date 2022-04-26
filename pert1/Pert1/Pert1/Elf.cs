using LearnOpenTK.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Pert1
{
    internal class Elf
    {

        List<Vector3> vertikal = new List<Vector3>();
        List<uint> indikasi = new List<uint>();
        int ebo;
        int vertex_buffer;
        int vertex_array;
        Shader shader;
        int indexu = 0;
        int[] _pascal = { };
        public Matrix4 _model;
        Vector3 colour;
        Matrix4 _view;
        Matrix4 _projection;
        public List<Vector3> _euler = new List<Vector3>();
        public Vector3 objectCenter = Vector3.Zero;

        public List<Elf> child = new List<Elf>();
        public List<Vector3> _vertice_bezier;

        public Elf(float[] _vertikal, uint[] _indikasi)
        {

        }

        public Elf(Vector3 colour)
        {
            setDefault();
            this.colour = colour;
        }
        public void setDefault()
        {
            _model = Matrix4.Identity;
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }
        public void Load(string shadervert, string shaderfrag, float Size_x, float Size_y)
        {
            vertex_buffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertex_buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, vertikal.Count * Vector3.SizeInBytes, vertikal.ToArray(), BufferUsageHint.StaticDraw);
            vertex_array = GL.GenVertexArray();
            GL.BindVertexArray(vertex_array);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            if (indikasi.Count != 0)
            {
                ebo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indikasi.Count * sizeof(uint), indikasi.ToArray(), BufferUsageHint.StaticDraw);
            }
            shader = new Shader(shadervert, shaderfrag);
            shader.Use();

            //_view = Matrix4.CreateTranslation(0.0f,0.0f,-3.0f);
            //_projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size_x / (float)Size_y, 0.1f, 100.0f);
            foreach (var i in child)
            {
                i.Load(shadervert, shaderfrag, Size_x, Size_y);
            }
        }

        public void render(int _lines, Matrix4 temp, Matrix4 camera_v, Matrix4 camera_pr)
        {
            shader.Use();
            GL.BindVertexArray(vertex_array);
            shader.SetVector3("ourcolor", colour);
            shader.SetMatrix4("model", _model);
            shader.SetMatrix4("view", camera_v);
            shader.SetMatrix4("projection", camera_pr);
            if (indikasi.Count != 0)
            {
                GL.DrawElements(PrimitiveType.Triangles, indikasi.Count, DrawElementsType.UnsignedInt,0);
            }
            else
            {
                if (_lines == 0) //gambar dengan isi DEFAULT
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, vertikal.Count);
                }
                else if (_lines == 1) //gambar garis tpi gak sampai semua
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, vertikal.Count);
                }
                else if (_lines == 2) // gambar tanpa isi
                {
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, vertikal.Count);
                }
                else if (_lines == 3)
                {
                    //lingkaran line
                    //GL.DrawArrays(PrimitiveType.LineLoop, 0, _vertices.Count);

                    //lingkaran berisi
                    GL.DrawArrays(PrimitiveType.LineStrip, 0, vertikal.Count);

                }
            }
            foreach (var i in child)
            {
                i.render(_lines, temp, camera_v, camera_pr);
            }
        }
        public void boxvertice(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //titik1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik5
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik6
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik7
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik8
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            indikasi = new List<uint>
            {
                ////SEGITIGA DEPAN 1
                //1,3,7,
                ////SEGITIGA DEPAN 2
                //1,3,7,
                ////SEGITIGA ATAS 1
                //1,3,7,
                ////SEGITIGA ATAS 2
                //1,3,7,
                ////SEGITIGA KANAN 1
                //1,3,7,
                ////SEGITIGA KANAN 2
                //1,3,7,
                ////SEGITIGA KIRI 1
                //1,3,7,
                ////SEGITIGA KIRI 2
                //1,3,7,
                ////SEGITIGA BELAKANG 1
                //1,3,7,
                ////SEGITIGA BELAKANG 2
                //1,3,7,
                ////SEGITIGA BAWAH 1
                //1,3,8,
                ////SEGITIGA BAWAH 2
                //1,3,8,

                                //SEGITIGA DEPAN 1
                //1,3,8,
                ////SEGITIGA DEPAN 2
                //1,3,8,
                ////SEGITIGA ATAS 1
                //1,3,8,
                ////SEGITIGA ATAS 2
                //1,3,8,
                ////SEGITIGA KANAN 1
                //1,3,8,
                ////SEGITIGA KANAN 2
                //1,3,8,
                ////SEGITIGA KIRI 1
                //1,3,8,
                ////SEGITIGA KIRI 2
                //1,3,8,
                ////SEGITIGA BELAKANG 1
                //1,3,8,
                ////SEGITIGA BELAKANG 2
                //1,3,8,
                ////SEGITIGA BAWAH 1
                //1,3,8,
                ////SEGITIGA BAWAH 2
                //1,3,8,


                1,3,7,
                1,3,7,

                1,3,7,
                1,3,7,

                1,3,7,
                1,3,7,

                1,3,7,
                1,3,7,

                1,3,7,
                1,3,7,

                1,3,7,
                1,3,7,
            };

            ////////FRONT FACE
            ////////SEGITIGA FRONT 1
            //vertikal.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, -0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, 0.5f, -0.5f));
            ////////SEGITIGA FRONT 2
            //vertikal.Add(new Vector3(0.5f, 0.5f, -0.5f));
            //vertikal.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            //vertikal.Add(new Vector3(-0.5f, -0.5f, -0.5f));

            ////////BACK FACE
            ////////SEGITIGA BACK 1
            //vertikal.Add(new Vector3(-0.5f, -0.5f, 0.5f));
            //vertikal.Add(new Vector3(0.5f, -0.5f, 0.5f));
            //vertikal.Add(new Vector3(0.5f, 0.5f, 0.5f));
            ////////SEGITIGA BACK 2
            //vertikal.Add(new Vector3(0.5f, 0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, -0.5f, 0.5f));

            ////////LEFT FACE
            ////////SEGITIGA LEFT 1
            //vertikal.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            //vertikal.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            ////////SEGITIGA LEFT 2
            //vertikal.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            //vertikal.Add(new Vector3(-0.5f, -0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, 0.5f, 0.5f));

            ////////RIGHT FACE
            ////////SEGITIGA RIGHT 1
            //vertikal.Add(new Vector3(0.5f, 0.5f, 0.5f));
            //vertikal.Add(new Vector3(0.5f, 0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, -0.5f, -0.5f));
            ////////SEGITIGA LEFT 2
            //vertikal.Add(new Vector3(0.5f, -0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, -0.5f, 0.5f));
            //vertikal.Add(new Vector3(0.5f, 0.5f, 0.5f));

            ////////BOTTOM FACE
            ////////SEGITIGA BOTTOM 1
            //vertikal.Add(new Vector3(-0.5f, -0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, -0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, -0.5f, 0.5f));
            ////////SEGITIGA BOTTOM 2
            //vertikal.Add(new Vector3(0.5f, -0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, -0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, -0.5f, -0.5f));

            ////////FRONT FACE
            ////////SEGITIGA BOTTOM 1
            //vertikal.Add(new Vector3(-0.5f, 0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, 0.5f, -0.5f));
            //vertikal.Add(new Vector3(0.5f, 0.5f, 0.5f));
            ////////SEGITIGA BOTTOM 2
            //vertikal.Add(new Vector3(0.5f, 0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, 0.5f, 0.5f));
            //vertikal.Add(new Vector3(-0.5f, 0.5f, -0.5f));
        }

        public void boxvertice2(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //titik1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            indikasi = new List<uint>
            {
                //SEGITIGA DEPAN 1
                1,3,6,
                //SEGITIGA DEPAN 2
                1,3,6,
                //SEGITIGA ATAS 1
                1,3,6,
                //SEGITIGA ATAS 2
                1,3,6,
                //SEGITIGA KANAN 1
                1,3,6,
                //SEGITIGA KANAN 2
                1,3,6,
                //SEGITIGA KIRI 1
                1,3,6,
                //SEGITIGA KIRI 2
                1,3,6,
                //SEGITIGA BELAKANG 1
                1,3,6,
                //SEGITIGA BELAKANG 2
                1,3,6,
                //SEGITIGA BAWAH 1
                1,3,6,
                //SEGITIGA BAWAH 2
                1,3,6,

            };
        }

        public void boxvertice3(float x, float y, float z, float length)
        {
            Vector3 temp_vector;

            //titik1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z - length / 2.0f;
            vertikal.Add(temp_vector);
            //titik1
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik2
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y + length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik3
            temp_vector.X = x - length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            //titik4
            temp_vector.X = x + length / 2.0f;
            temp_vector.Y = y - length / 2.0f;
            temp_vector.Z = z + length / 2.0f;
            vertikal.Add(temp_vector);
            indikasi = new List<uint>
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
                    vertikal.Add(temp_vector);
                }
            }

        }
        public void createEllipsoid2(float radiusX, float radiusY, float radiusZ, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            objectCenter.X = _x;
            objectCenter.Y = _y;
            objectCenter.Z = _z;
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

                    temp_vector.X = x * (float)Math.Cos(sectorAngle) + _x;
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle) + _y;
                    temp_vector.Z = z + _z;
                    vertikal.Add(temp_vector);
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
                        indikasi.Add(k1);
                        indikasi.Add(k2);
                        indikasi.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        indikasi.Add(k1 + 1);
                        indikasi.Add(k2);
                        indikasi.Add(k2 + 1);
                    }
                }
            }
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
                vertikal.Add(p);
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

        Vector3 setBeizer(float t)
        {
            //Console.WriteLine(t);
            Vector3 p = new Vector3(0f, 0f, 0f);
            float[] k = new float[3];

            k[0] = (float)Math.Pow((1 - t), 3 - 1 - 0) * (float)Math.Pow(t, 0) * 1;
            k[1] = (float)Math.Pow((1 - t), 3 - 1 - 1) * (float)Math.Pow(t, 1) * 2;
            k[2] = (float)Math.Pow((1 - t), 3 - 1 - 2) * (float)Math.Pow(t, 2) * 1;


            //titik 1
            p.X += k[0] * objectCenter.X;
            p.Y += k[0] * objectCenter.Y;

            //titik 2
            p.X += k[1] * (objectCenter.X);
            p.Y += k[1] * objectCenter.Y;

            //titik 3
            p.X += k[2] * objectCenter.X;
            p.Y += k[2] * objectCenter.Y;

            //Console.WriteLine(p.X + " "+ p.Y);

            return p;

        }

        public void createEllipsoid(float radius, float _x, float _y, float _z, int sectorCount, int stackCount)
        {
            objectCenter.X = _x;
            objectCenter.Y = _y;
            objectCenter.Z = _z;
            float pi = (float)Math.PI;
            Vector3 temp_vector;
            float sectorStep = 2 * (float)Math.PI / sectorCount;
            float stackStep = (float)Math.PI / stackCount;
            float sectorAngle, StackAngle, x, y, z;

            for (int i = 0; i <= stackCount; ++i)
            {
                StackAngle = pi / 2 - i * stackStep;
                x = radius * (float)Math.Cos(StackAngle);
                y = radius * (float)Math.Cos(StackAngle);
                z = radius * (float)Math.Sin(StackAngle);

                for (int j = 0; j <= sectorCount; ++j)
                {
                    sectorAngle = j * sectorStep;

                    temp_vector.X = x * (float)Math.Cos(sectorAngle) + _x;
                    temp_vector.Y = y * (float)Math.Sin(sectorAngle) + _y;
                    temp_vector.Z = z + _z;
                    vertikal.Add(temp_vector);
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
                        indikasi.Add(k1);
                        indikasi.Add(k2);
                        indikasi.Add(k1 + 1);
                    }
                    if (i != (stackCount - 1))
                    {
                        indikasi.Add(k1 + 1);
                        indikasi.Add(k2);
                        indikasi.Add(k2 + 1);
                    }
                }
            }
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

            objectCenter = getRotationResult(pivot, vector, radAngle, objectCenter);

            foreach (var i in child)
            {
                i.rotate(pivot, vector, angle);
            }
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

        //transformasi
        public void trans(float x, float y, float z)
        {

            _model = _model * Matrix4.CreateTranslation(x, y, z);
            foreach (var i in child)
            {
                i.trans(x, y, z);
            }
        }

        public void scale(float x)
        {

            _model = _model * Matrix4.CreateTranslation(-1 * (objectCenter)) * Matrix4.CreateScale(x) * Matrix4.CreateTranslation((objectCenter));
            foreach (var i in child)
            {
                i.scale(x);
            }
        }

        //berzier
        public void AddCoordinates(float x1, float y1, float z1)
        {
            _vertice_bezier.Add(new Vector3(x1, y1, z1));

        }
        public Vector3 getSegment(float Time)
        {
            Time = Math.Clamp(Time, 0, 1);
            float time = 1 - Time;
            Vector3 result =
            ((float)Math.Pow(time, 3) * _vertice_bezier[0])
            + (3 * time * time * Time * _vertice_bezier[1])
            + (3 * time * Time * Time * _vertice_bezier[2])
            + (Time * Time * Time * _vertice_bezier[3]);
            return result;
        }
        public void Bezier3d()
        {
            List<Vector3> segments = new List<Vector3>();
            float time;

            for (float i = 0; i < 1.0f; i += 0.01f)
            {

                time = i;
                segments.Add(getSegment(time));
            }

            setVertices(segments);

        }
        public void setVertices(List<Vector3> vertices)
        {
            vertikal = vertices;
        }

        public void resetEuler()
        {
            _euler.Clear();
            _euler.Add(Vector3.UnitX);
            _euler.Add(Vector3.UnitY);
            _euler.Add(Vector3.UnitZ);
        }
        //public List<int> getRow(int rowIndex)
        //{
        //    List<int> currow = new List<int>();
        //    currow.Add(1);
        //    if (rowIndex == 0)
        //    {
        //        return currow;
        //    }
        //    Console.WriteLine(rowIndex);
        //    List<int> prev = getRow(rowIndex - 1);
        //    for (int i = 1; i < prev.Count; i++)
        //    {
        //        int curr = prev[i - 1] + prev[i];
        //        currow.Add(curr);
        //    }
        //    currow.Add(1);
        //    return currow;
        //}
        //public List<float> CreateCurveBezier()
        //{
        //    List<float> vertic_bezier = new List<float>();
        //    List<int> pascal = getRow(indexu - 1);
        //    _pascal = pascal.ToArray();
        //    for (float t = 0; t <= 1.0f; t += 0.01f)
        //    {
        //        Vector2 p = getP(indexu, t);
        //        vertic_bezier.Add(p.X);
        //        vertic_bezier.Add(p.Y);
        //        vertic_bezier.Add(0);
        //    }
        //    return vertic_bezier;
        //}
        //public Vector2 getP(int n, float t)
        //{
        //    Vector2 p = new Vector2(0, 0);
        //    float[] k = new float[n];
        //    for (int i = 0; i < n; i++)
        //    {
        //        k[i] = (float)Math.Pow((1 - t), (n - 1 - i)) * (float)Math.Pow(t, i) * _pascal[i];
        //        p.X += k[i] * vertikal[i * 3];
        //        p.Y += k[i] * vertikal[i * 3 + 1];
        //    }
        //    return p;
        //}
    }
}

