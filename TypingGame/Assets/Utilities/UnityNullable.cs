using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Based on https://stackoverflow.com/questions/52854129/unity-doesnt-serialize-int-field
[Serializable]
public class UnityNullableInt
{
    public int Value;
    public bool HasValue;
}

[Serializable]
public class UnityNullableFloat
{
    public float Value;
    public bool HasValue;
}

[Serializable]
public class UnityNullableEnum<T>
    where T : Enum
{
    public T Value;
    public bool HasValue;
}