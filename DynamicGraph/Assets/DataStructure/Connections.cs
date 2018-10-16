using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Connections
{

    public float[,] Matrix { get; private set; }
    public int MatrixSize { get; private set; }

    public Connections(int matrixSize = 0)
    {
        this.MatrixSize = matrixSize;
        Matrix = new float[matrixSize, matrixSize];
    }
    public bool isLog = false;
    public Connections(float specialCall)
    {
        isLog = true;
        this.MatrixSize = 0;
    }
    public void Add(Connections c)
    {
        if (c.MatrixSize > this.MatrixSize)
            ExpandMatrix(c.MatrixSize);

        for (int i = 0; i < c.MatrixSize; i++)
            for (int j = 0; j < c.MatrixSize; j++)
                Matrix[i, j] += c.Matrix[i, j];
    }
    public void ExpandMatrix(int newsize)
    {
        if (newsize < MatrixSize)
            return;
        float[,] oldMatrix = Matrix;
        int oldMatrixSize = MatrixSize;

        Matrix = new float[newsize, newsize];
        MatrixSize = newsize;
        if (isLog)
            for (int i = 0; i < MatrixSize; i++)
                for (int j = 0; j < MatrixSize; j++)
                    Matrix[i, j] = 1;
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
    public void MultpCustom(Connection c)
    {
        if (c.a.code < c.b.code) //always a>=b
        {
            int temp = c.a.code;
            c.a.code = c.b.code;
            c.b.code = temp;
        }
        if (c.a.code + 1 > this.MatrixSize)
            ExpandMatrix(c.a.code + 1);
        Matrix[c.a.code, c.b.code] *= c.strength;
    }
    public void MultpCustom(Connections c)
    {
        if (c.MatrixSize > this.MatrixSize)
            ExpandMatrix(c.MatrixSize);

        for (int i = 0; i < c.MatrixSize; i++)
            for (int j = 0; j < c.MatrixSize; j++)
                if(c.Matrix[i, j] != 0)
                    Matrix[i, j] *= c.Matrix[i, j];
    }
        public Connections Copy()
    {
        var @return = new Connections(this.MatrixSize);
        @return.Add(this);
        @return.isLog = isLog;
        return @return;
    }
    public void Decay(int durationh)
    {
        for (int i = 0; i < MatrixSize; i++)
            for (int j = 0; j < i; j++)
            {
                if (Matrix[i, j] != 0)
                    Matrix[i, j] = Matrix[i, j] * Mathf.Pow(2 ,-(float)durationh / DataConsts.DecayHalfLife); //(int)Mathf.Pow(Matrix[i, j], -Mathf.Pow(2, (float)durationh / DataConsts.DecayRate));
                if (Matrix[i, j] < 0)
                    Matrix[i, j] = 0;
            }
            
        
    }

    public float GetConnectionBetween(HumanName a, HumanName b)
    {
        if(a.code >= MatrixSize || b.code >= MatrixSize)
            return 0;
        if (a.code < b.code) //always a>=b
        {
            var temp = a;
            a = b;
            b = temp;
        }
        return Matrix[a.code, b.code];
    }

    public float GetConnectionBetween(int a, int b)
    {
        if (a >= MatrixSize || b >= MatrixSize)
            return 0;
            if (a < b) //always a>=b
            {
                int temp = a;
                a = b;
                b = temp;
            }
            return Matrix[a, b];

    }


    public float GetScore(int a)
    {
        float score = 0;
        for (int i = 0; i < MatrixSize; i++)
            for (int j = 0; j < i; j++)
                if (i == a || j == a)
                    score += Matrix[i, j];//hack
        return score;
    }
    public float GetScore(HumanName a)
    {
        float score = 0;
        for (int i = 0; i < MatrixSize; i++)
            for (int j = 0; j < i; j++)
                if (i == a.code || j == a.code)
                    score += Matrix[i, j];//hack
        return score;
    }
}
