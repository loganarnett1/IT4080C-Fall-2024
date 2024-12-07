using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct Message: INetworkSerializable, IEquatable<Message>
{
    public ulong clientId;
    public FixedString64Bytes message;

    public Message(ulong clientId, string message)
    {
        this.clientId = clientId;
        this.message = message;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out clientId);
            reader.ReadValueSafe(out message);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(clientId);
            writer.WriteValueSafe(message);
        }
    }

    public bool Equals(Message other) => clientId == other.clientId && message == other.message;

    public override bool Equals(object obj) => obj is Message other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(clientId, message);

    public override string ToString() => $"{clientId}:{message.Value.ToString()}";
    
    public static implicit operator string(Message message) => message.ToString();
    public static implicit operator Message(string s) => new Message { message = new FixedString64Bytes(s) };
}