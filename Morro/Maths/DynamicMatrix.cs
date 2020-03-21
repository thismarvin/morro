using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Morro.Maths
{
    /// <summary>
    /// Represents a mathmatical matrix of any given size.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class DynamicMatrix<T> : ICloneable where T : struct, IConvertible
    {
        public int Rows { get; private set; }
        public int Columns { get; private set; }

        private T[] data;

        /// <summary>
        /// Creates an empty matrix of a given dimension.
        /// </summary>
        /// <param name="rows">The amount of rows the new matrix should have.</param>
        /// <param name="columns">The amount of columns the new matrix should have.</param>
        public DynamicMatrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            data = new T[rows * columns];
        }

        /// <summary>
        /// Creates a matrix from a given 2D array of data.
        /// </summary>
        /// <param name="rows">The amount of rows in your data.</param>
        /// <param name="columns">The amount of columns in your data.</param>
        /// <param name="data">The data that will be used to fill the new matrix.</param>
        public DynamicMatrix(int rows, int columns, T[] data)
        {
            Rows = rows;
            Columns = columns;

            SetData(data);
        }

        /// <summary>
        /// Returns a deep copy of the current matrix.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new DynamicMatrix<T>(Rows, Columns, data);
        }

        /// <summary>
        /// Returns the value at a given location of the matrix.
        /// </summary>
        /// <param name="x">The column of the matrix that the target value is within.</param>
        /// <param name="y">The row of the matrix that the target value is within.</param>
        /// <returns>The value at a given location of the matrix.</returns>
        public T Get(int x, int y)
        {
            return data[Columns * y + x];
        }

        /// <summary>
        /// Sets the value at a given location of the matrix.
        /// </summary>
        /// <param name="x">The column of the matrix that the target value is within.</param>
        /// <param name="y">The row of the matrix that the target value is within.</param>
        /// <param name="value">The new value to replace the existing value to.</param>
        public void Set(int x, int y, T value)
        {
            data[Columns * y + x] = value;
        }

        /// <summary>
        /// Replaces the current data of the matrix with the values of a given 2D array. (Note that the length of the 2D array must adhere to the current dimensions of the matrix).
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetData(T[] data)
        {
            if (data.Length != Rows * Columns)
                throw new ArgumentException("The data does not match the dimensions of the matrix.");

            this.data = data;
        }

        /// <summary>
        /// Flips the current matrix over its diagonal.
        /// </summary>
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

        public static DynamicMatrix<T> operator +(DynamicMatrix<T> a, T b)
        {
            DynamicMatrix<T> result = new DynamicMatrix<T>(a.Rows, a.Columns);
            for (int y = 0; y < a.Rows; y++)
            {
                for (int x = 0; x < a.Columns; x++)
                {
                    result.Set(x, y, Add(a.Get(x, y), b));
                }
            }
            return result;
        }

        public static DynamicMatrix<T> operator *(DynamicMatrix<T> a, T b)
        {
            DynamicMatrix<T> result = new DynamicMatrix<T>(a.Rows, a.Columns);
            for (int y = 0; y < a.Rows; y++)
            {
                for (int x = 0; x < a.Columns; x++)
                {
                    result.Set(x, y, Multiply(a.Get(x, y), b));
                }
            }
            return result;
        }

        public static DynamicMatrix<T> operator +(DynamicMatrix<T> a, DynamicMatrix<T> b)
        {
            if (a.Rows != b.Rows && a.Columns != b.Columns)
            {
                throw new ArgumentException("Both matricies must be the same size in order to add them.");
            }

            DynamicMatrix<T> result = new DynamicMatrix<T>(a.Rows, a.Columns);
            for (int y = 0; y < a.Rows; y++)
            {
                for (int x = 0; x < a.Columns; x++)
                {
                    result.Set(x, y, Add(a.Get(x, y), b.Get(x, y)));
                }
            }
            return result;
        }

        public static DynamicMatrix<T> operator *(DynamicMatrix<T> a, DynamicMatrix<T> b)
        {
            if (a.Columns != b.Rows)
            {
                throw new ArgumentException("Cannot multiply these matricies.");
            }

            DynamicMatrix<T> result = new DynamicMatrix<T>(a.Rows, b.Columns);
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

            string ExtraSpaces(int x, int y)
            {
                StringBuilder spaces = new StringBuilder();

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
                    spaces.Append(" ");
                }
                return spaces.ToString();
            }
        }
    }
}
