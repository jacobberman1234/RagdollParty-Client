using Unity.Netcode;
using UnityEngine;

public struct NetworkPlayerData : INetworkSerializable
{
    private Vector3 pos;
    private short yRot;

    internal Vector3 Pos
    {
        get => pos;
        set
        {
            pos = value;
        }
    }
    internal Vector3 Rot
    {
        get => new Vector3(0, yRot, 0);
        set => yRot = (short)value.y;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref pos);
        serializer.SerializeValue(ref yRot);
    }
}

public struct NetworkAnimData : INetworkSerializable
{
    private float x, z;

    internal Vector2 Move
    {
        get => new Vector2(x, z);
        set
        {
            x = value.x;
            z = value.y;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref z);
    }
}
