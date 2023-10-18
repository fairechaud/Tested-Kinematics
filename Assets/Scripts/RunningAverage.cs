using System.Collections.Generic;
using UnityEngine;
 
public abstract class RunningAverage<T>
{
    protected Queue<T> m_History = new Queue<T>();
    protected T m_TotalSum;
    protected int m_Count;
    protected abstract void AddValue(T aVal);
    protected abstract void SubtractValue(T aVal);
    public abstract T Current { get; }
    public int Count
    {
        get { return m_Count; }
        set
        {
            if (value < 1)
                value = 1;
            if (value < m_Count)
            {
                while (m_History.Count > value)
                    SubtractValue(m_History.Dequeue());
            }
            m_Count = value;
        }
    }
 
    public RunningAverage(int aCount)
    {
        m_Count = aCount;
        if (m_Count < 1)
            m_Count = 1;
    }
    public void Add(T aValue)
    {
        if (m_History.Count >= m_Count)
            SubtractValue(m_History.Dequeue());
        m_History.Enqueue(aValue);
        AddValue(aValue);
    }
    public void Clear()
    {
        m_History.Clear();
        m_TotalSum = default(T);
    }
}
 
public class RunningDoubleAverage : RunningAverage<double>
{
    public override double Current
    {
        get
        {
            if (m_History.Count > 0)
                return m_TotalSum / m_History.Count;
            return 0;
        }
    }
    public RunningDoubleAverage(int aCount) : base(aCount) { }
 
    protected override void AddValue(double aVal)
    {
        m_TotalSum += aVal;
    }
 
    protected override void SubtractValue(double aVal)
    {
        m_TotalSum -= aVal;
    }
}
 
public class RunningVector4Average : RunningAverage<Vector4>
{
    public override Vector4 Current
    {
        get
        {
            if (m_History.Count > 0)
                return m_TotalSum / m_History.Count;
            return Vector4.zero;
        }
    }
    public RunningVector4Average(int aCount) : base(aCount) { }
 
    protected override void AddValue(Vector4 aVal)
    {
        m_TotalSum += aVal;
    }
 
    protected override void SubtractValue(Vector4 aVal)
    {
        m_TotalSum -= aVal;
    }
}