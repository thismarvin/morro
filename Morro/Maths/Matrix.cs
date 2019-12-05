using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Maths
{
    class Matrix<T>
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        private T[] data;

        public Matrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            data = new T[rows * columns];
        }

        public Matrix(int rows, int columns, T[] data)
        {
            Rows = rows;
            Columns = columns;

            SetData(data);
        }

        public static Matrix<float> ConvertMonogameMatrix(Matrix matrix)
        {
            float[] data = new float[]
            {
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44,
            };
            return new Matrix<float>(4, 4, data);
        }

        public static Vector2 ApplyTransformation(float x, float y, Matrix matrix)
        {
            Matrix<float> transform = ConvertMonogameMatrix(matrix);
            Matrix<float> position = new Matrix<float>(1, 4, new float[] { x, y, 0, 1 });
            Matrix<float> result = position * transform;
            return new Vector2(result.Get(0, 0), result.Get(1, 0));
        }

        public Matrix<T> Clone()
        {
            return new Matrix<T>(Rows, Columns, data);
        }

        public T Get(int x, int y)
        {
            return data[Columns * y + x];
        }

        public void Set(int x, int y, T value)
        {
            data[Columns * y + x] = value;
        }

        public void SetData(T[] data)
        {
            if (data.Length == Rows * Columns)
            {
                this.data = data;
            }
            else
            {
                throw new ArgumentException("The data does not match the dimensions of the Matrix.");
            }
        }

        public void Transpose()
        {
            T[] newData = new T[Rows * Columns];
            int newColumns = Rows;
            int newRows = Columns;

            for (int y = 0; y < newRows; y++)
            {
                for (int x = 0; x < newColumns; x++)
                {
                    newData[newColumns * y + x] = data[Columns * x + y];
                }
            }

            data = newData;
            Rows = newRows;
            Columns = newColumns;
        }

        public static Matrix<T> operator +(Matrix<T> a, T b)
        {
            Matrix<T> result = new Matrix<T>(a.Rows, a.Columns);
            for (int y = 0; y < a.Rows; y++)
            {
                for (int x = 0; x < a.Columns; x++)
                {
                    result.Set(x, y, Add(a.Get(x, y), b));
                }
            }
            return result;
        }

        public static Matrix<T> operator *(Matrix<T> a, T b)
        {
            Matrix<T> result = new Matrix<T>(a.Rows, a.Columns);
            for (int y = 0; y < a.Rows; y++)
            {
                for (int x = 0; x < a.Columns; x++)
                {
                    result.Set(x, y, Multiply(a.Get(x, y), b));
                }
            }
            return result;
        }

        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
        {
            if (a.Rows != b.Rows && a.Columns != b.Columns)
            {
                throw new ArgumentException("Both matricies must be the same size in order to add them.");
            }

            Matrix<T> result = new Matrix<T>(a.Rows, a.Columns);
            for (int y = 0; y < a.Rows; y++)
            {
                for (int x = 0; x < a.Columns; x++)
                {
                    result.Set(x, y, Add(a.Get(x, y), b.Get(x, y)));
                }
            }
            return result;
        }

        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
        {
            if (a.Columns != b.Rows)
            {
                throw new ArgumentException("Cannot multiply these matricies.");
            }

            Matrix<T> result = new Matrix<T>(a.Rows, b.Columns);
            for (int aY = 0; aY < a.Rows; aY++)
            {
                for (int aX = 0; aX < a.Columns; aX++)
                {
                    for (int bX = 0; bX < b.Columns; bX++)
                    {
                        result.Set(bX, aY, Add(result.Get(bX, aY), Multiply(a.Get(aX, aY), b.Get(bX, aX))));
                    }
                }
            }
            return result;
        }

        private static T Add(T a, T b)
        {
            dynamic c = a;
            dynamic d = b;
            return c + d;
        }

        private static T Multiply(T a, T b)
        {
            dynamic c = a;
            dynamic d = b;
            return c * d;
        }

        private string ExtraSpaces(int x, int y)
        {
            StringBuilder stringBuilder = new StringBuilder();

            int mostCharacters = Get(x, 0).ToString().Length;
            int value;
            for (int i = 0; i < Rows; i++)
            {
                value = Get(x, i).ToString().Length;
                mostCharacters = value > mostCharacters ? value : mostCharacters;
            }

            int total = mostCharacters - Get(x, y).ToString().Length;
            for (int i = 0; i < total; i++)
            {
                stringBuilder.Append(" ");
            }
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0}x{1}\n", Rows, Columns));
            for (int y = 0; y < Rows; y++)
            {
                stringBuilder.Append("| ");
                for (int x = 0; x < Columns; x++)
                {
                    if (x != Columns - 1)
                    {
                        stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0}{1}  ", ExtraSpaces(x, y), Get(x, y)));
                    }
                    else
                    {
                        stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0}{1}", ExtraSpaces(x, y), Get(x, y)));
                    }
                }
                stringBuilder.Append(" |\n");
            }
            return stringBuilder.ToString();
        }
    }
}
