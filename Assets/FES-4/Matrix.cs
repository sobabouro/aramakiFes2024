using System;
using Unity.Mathematics;
using UnityEngine;

public struct Matrix
{
    public int Rows { get; }
    public int Cols { get; }
    public double[,] Data { get; }

    //
    // �T�v:
    //     The inverse of this matrix. (Read Only)
    public Matrix inverse => Inverse(this);

    //
    // �T�v:
    //     Returns the transpose of this matrix (Read Only).
    public Matrix transpose => Transpose(this);

    public double this[int row, int column]
    {
        get
        {
            return Data[row, column];
        }
        set
        {
            Data[row, column] = value;
        }
    }

    //
    // �T�v:
    //     Returns the identity matrix (Read Only).
    public static Matrix Identity(int n)
    {
        Matrix identityMatrix = new Matrix(n, n);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                identityMatrix[i, j] = (i == j) ? 1 : 0;
            }
        }
        return identityMatrix;
    }

    //
    // �T�v:
    //     Returns a matrix with all elements set to zero (Read Only).
    public static Matrix Zero(int n)
    {
        Matrix zeroMatrix = new Matrix(n, n);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                zeroMatrix[i, j] = 0;
            }
        }
        return zeroMatrix;
    }

    // �t�s������߂郁�\�b�h�i�K�E�X�E�W�����_���@�j
    public static Matrix Inverse(Matrix m)
    {
        if (m.Rows != m.Cols)
        {
            throw new InvalidOperationException("�t�s��͐����s��ɑ΂��Ă̂݌v�Z�ł��܂��B");
        }

        int n = m.Rows;
        Matrix augmented = new Matrix(n, 2 * n);

        // �g��s��i���̍s��ƒP�ʍs�����ׂ��s��j���쐬
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                augmented[i, j] = m[i, j];
            }
            augmented[i, i + n] = 1.0;
        }

        // �K�E�X�E�W�����_���@��p���āA�g��s���ό`
        for (int i = 0; i < n; i++)
        {
            // �s�{�b�g��0�̏ꍇ�A�s����������
            if (augmented[i, i] == 0)
            {
                bool swapped = false;
                for (int j = i + 1; j < n; j++)
                {
                    if (augmented[j, i] != 0)
                    {
                        for (int k = 0; k < augmented.Cols; k++)
                        {
                            double temp = augmented[i, k];
                            augmented[i, k] = augmented[j, k];
                            augmented[j, k] = temp;
                        }
                        swapped = true;
                        break;
                    }
                }
                if (!swapped)
                {
                    throw new InvalidOperationException("�s��͋t�s��������܂���B");
                }
            }

            // �Ίp������1�ɂ���
            double pivot = augmented[i, i];
            for (int j = 0; j < 2 * n; j++)
            {
                augmented[i, j] /= pivot;
            }

            // ���̍s�̃s�{�b�g���0�ɂ���
            for (int j = 0; j < n; j++)
            {
                if (i != j)
                {
                    double factor = augmented[j, i];
                    for (int k = 0; k < 2 * n; k++)
                    {
                        augmented[j, k] -= factor * augmented[i, k];
                    }
                }
            }
        }

        // �E�������t�s��
        Matrix inverse = new Matrix(n, n);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                inverse[i, j] = augmented[i, j + n];
            }
        }

        return inverse;
    }

    public static Matrix Transpose(Matrix m)
    {
        Matrix result = new Matrix(m.Cols, m.Rows);
        for (int i = 0; i < m.Cols; i++)
        {
            for (int j = 0; j < m.Rows; j++)
            {
                result[i, j] = m[j, i];
            }
        }
        return result;
    }


    public Matrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        Data = new double[rows, cols];
    }

    public Matrix(double[,] values)
    {
        Rows = values.GetLength(0);
        Cols = values.GetLength(1);
        Data = new double[Rows, Cols];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Cols; j++)
            {
                Data[i, j] = values[i, j];
            }
        }
    }

    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    public override bool Equals(object other)
    {
        if (!(other is Matrix))
        {
            return false;
        }

        return Equals((Matrix)other);
    }

    public bool Equals(Matrix other)
    {
        return Data.Equals(other.Data);
    }

    public static Matrix operator *(Matrix lhs, Matrix rhs)
    {
        if (lhs.Cols != rhs.Rows)
        {
            throw new ArgumentException("�s��A�̗񐔂ƍs��B�̍s������v���Ă��܂���B��Z�ł��܂���B");
        }
        Matrix result = new Matrix(lhs.Rows, rhs.Cols);
        for (int i = 0; i < lhs.Rows; i++)
        {
            for (int j = 0; j < rhs.Cols; j++)
            {
                double sum = 0;
                for (int k = 0; k < lhs.Cols; k++)
                {
                    sum += lhs[i, k] * rhs[k, j];
                }
                result[i, j] = sum;
            }
        }

        return result;
    }

    public static double[] operator *(Matrix lhs, double[] rhs)
    {
        if (lhs.Cols != rhs.Length)
        {
            throw new ArgumentException("�s��A�̗񐔂ƍs��B�̍s������v���Ă��܂���B��Z�ł��܂���B");
        }
        double[] result = new double[lhs.Rows];
        for (int i = 0; i < lhs.Rows; i++)
        {
            double sum = 0;
            for (int k = 0; k < lhs.Cols; k++)
            {
                sum += lhs[i, k] * rhs[k];
            }
            result[i] = sum;
        }

        return result;
    }

    public static Matrix operator +(Matrix lhs, Matrix rhs)
    {
        if (lhs.Rows != rhs.Rows || lhs.Cols != rhs.Cols)
        {
            throw new ArgumentException("�s��A�ƍs��B�̎�������v���Ă��܂���B���Z�ł��܂���B");
        }

        Matrix result = new Matrix(lhs.Rows, lhs.Cols);
        for (int i = 0; i < lhs.Rows; i++)
        {
            for (int j = 0; j < lhs.Cols; j++)
            {
                result[i, j] = lhs[i, j] + rhs[i, j];
            }
        }
        return result;
    }
    public static Matrix operator -(Matrix lhs, Matrix rhs)
    {
        if (lhs.Rows != rhs.Rows || lhs.Cols != rhs.Cols)
        {
            throw new ArgumentException("�s��A�ƍs��B�̎�������v���Ă��܂���B���Z�ł��܂���B");
        }

        Matrix result = new Matrix(lhs.Rows, lhs.Cols);
        for (int i = 0; i < lhs.Rows; i++)
        {
            for (int j = 0; j < lhs.Cols; j++)
            {
                result[i, j] = lhs[i, j] - rhs[i, j];
            }
        }
        return result;
    }

    public static bool operator ==(Matrix lhs, Matrix rhs)
    {
        if (lhs.Rows != rhs.Rows || lhs.Cols != rhs.Cols)
        {
            return false;
        }

        for (int i = 0; i < lhs.Rows; i++)
        {
            for (int j = 0; j < lhs.Cols; j++)
            {
                if(lhs[i, j] != rhs[i, j]) return false;
            }
        }
        return true;
    }

    public static bool operator !=(Matrix lhs, Matrix rhs)
    {
        return !(lhs == rhs);
    }


    private static double[] SumDoubleAry(double[] a, double[] b)
    {
        if (a.Length != b.Length)
        {
            throw new InvalidOperationException("���Z�͓��������̔z��ɑ΂��Ă̂݌v�Z�ł��܂��B");
        }
        double[] result = new double[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = a[i] + b[i];
        }
        return result;
    }
}