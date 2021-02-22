using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableVector3
{
    private float _x;
    private float _y;
    private float _z;

    public SerializableVector3(Vector3 vector)
    {
        _x = vector.x;
        _y = vector.y;
        _z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(_x, _y, _z);
    }
}
