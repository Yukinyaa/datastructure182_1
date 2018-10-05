using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Connections
{

    public float[,] Matrix { get; private set; }
    public int MatrixSize { get; private set; }

    public Connections(int matrixSize = 2)
    {
        Matrix = new float[matrixSize, matrixSize];
    }
    public void Add(Connections c)
    {
        if (c.MatrixSize > this.MatrixSize)
            ExpandMatrix(c.MatrixSize);

        for (int i = 0; i < c.MatrixSize; i++)
            for (int j = 0; j < c.MatrixSize; j++)
                Matrix[i, j] += c.Matrix[i, j];
    }
    void ExpandMatrix(int newsize)
    {
        if (newsize < MatrixSize)
            return;
        float[,] oldMatrix = Matrix;
        int oldMatrixSize = MatrixSize;

        Matrix = new float[newsize, newsize];
        MatrixSize = newsize;
        for (int i = 0; i < oldMatrixSize; i++)
            for (int j = 0; j < oldMatrixSize; j++)
                Matrix[i, j] = oldMatrix[i, j];
}

    public void Add(Connection c)
    {
        if (c.a.code < c.b.code) //always a>=b
        {
            int temp = c.a.code;
            c.a.code = c.b.code;
            c.b.code = temp;
        }
        if (c.a.code + 1 > this.MatrixSize)
            ExpandMatrix(c.a.code + 1);
        Matrix[c.a.code, c.b.code] += c.strength;
    }
    public Connections Copy()
    {
        var @return = new Connections(this.MatrixSize);
        @return.Add(this);
        return @return;
    }
    public void Decay(int durationh)
    {
        for (int i = 0; i < MatrixSize; i++)
            for (int j = 0; j < MatrixSize; j++)
                Matrix[i, j] = (int)Mathf.Pow(Matrix[i, j], -Mathf.Pow(2, (float)durationh / DataConsts.DecayRate));
        
    }

    public float GetConnectionBetween(HumanName a, HumanName b)
    {
        if (a.code < b.code) //always a>=b
        {
            int temp = a.code;
            a.code = b.code;
            b.code = temp;
        }
        return Matrix[a.code,b.code];
    }
}
