using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;

public struct PlayerInfoData : INetworkSerializable, IEquatable<PlayerInfoData>
{
    public ulong _clientId;
    public FixedString64Bytes _name;
    public bool _isPlayerReady;
    public Color _color;

    public PlayerInfoData(ulong id)
    {
        _clientId = id;
        _name = "";
        _isPlayerReady = false;
        _color = Color.black;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out _clientId);
            reader.ReadValueSafe(out _name);
            reader.ReadValueSafe(out _isPlayerReady);
            reader.ReadValueSafe(out _color);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(_clientId);
            writer.WriteValueSafe(_name);
            writer.WriteValueSafe(_isPlayerReady);
            writer.WriteValueSafe(_color);
        }
    }


    public bool Equals(PlayerInfoData other)
    {
        return (
            _clientId == other._clientId &&
            _name == other._name &&
            _isPlayerReady == other._isPlayerReady &&
            _color == other._color
        );
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerInfoData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_clientId, _name, _isPlayerReady, _color);
    }

    public override string ToString() => _name.Value.ToString();

    public static implicit operator string(PlayerInfoData name) => name.ToString();
    public static implicit operator PlayerInfoData(string s) => new PlayerInfoData { _name = new FixedString64Bytes(s) };
}
