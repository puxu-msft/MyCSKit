using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace My;

public class MyPackedKV
{
    public UInt16 KeySize { get; set; }
    public UInt32? ValSize { get; set; }
    public byte[]? KeyData { get; set; }
    public byte[]? ValData { get; set; }

    public MyPackedKV()
    {
    }

    public MyPackedKV(UInt16 keySize, UInt32? valSize = null, byte[]? keyData = null, byte[]? valData = null)
    {
        KeySize = keySize;
        ValSize = valSize;
        KeyData = keyData;
        ValData = valData;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(KeySize);

        if (ValSize.HasValue)
        {
            writer.Write(ValSize.Value);
        }

        if (KeyData != null && KeyData.Length > 0)
        {
            writer.Write(KeyData);
        }

        if (ValSize.HasValue && ValData != null && ValData.Length > 0)
        {
            writer.Write(ValData);
        }
    }

    public static IEnumerable<MyPackedKV> Read(BinaryReader reader, bool hasValSize, bool hasKeyData, bool hasValData)
    {
        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            var pack = new MyPackedKV();

            pack.KeySize = reader.ReadUInt16();

            if (hasValSize)
            {
                pack.ValSize = reader.ReadUInt32();
            }
            else if (hasValData)
            {
                throw new InvalidOperationException("Cannot have ValData without ValSize.");
            }

            if (hasKeyData && pack.KeySize > 0)
            {
                pack.KeyData = reader.ReadBytes(pack.KeySize);
            }

            if (hasValData && pack.ValSize.HasValue && pack.ValSize.Value > 0)
            {
                pack.ValData = reader.ReadBytes((int)pack.ValSize.Value);
            }

            yield return pack;
        }
    }

    public byte[] ToBytes()
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms))
        {
            Write(writer);
            return ms.ToArray();
        }
    }

    public static IEnumerable<MyPackedKV> FromBytes(byte[] data, bool hasValSize, bool hasKeyData, bool hasValData)
    {
        using (var ms = new MemoryStream(data))
        using (var reader = new BinaryReader(ms))
        {
            foreach (var pack in Read(reader, hasValSize, hasKeyData, hasValData))
            {
                yield return pack;
            }
        }
    }
}
