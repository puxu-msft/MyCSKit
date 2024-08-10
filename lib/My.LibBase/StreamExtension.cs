using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace My
{
    public class FromHexTransform : System.Security.Cryptography.ICryptoTransform
    {
        public bool CanReuseTransform => true;
        public bool CanTransformMultipleBlocks => true;
        public int InputBlockSize => 2;
        public int OutputBlockSize => 1;

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (inputCount == 0)
                return 0;

            if (inputBuffer == null)
                throw new ArgumentNullException(nameof(inputBuffer));

            if (outputBuffer == null)
                throw new ArgumentNullException(nameof(outputBuffer));

            int inputBlocks = Math.DivRem(inputCount, InputBlockSize, out int inputRemainder);

            if (inputBlocks == 0)
                throw new ArgumentOutOfRangeException(nameof(inputCount));

            if (inputRemainder != 0)
                throw new ArgumentOutOfRangeException(nameof(inputCount));

            int requiredOutputLength = checked(inputBlocks * OutputBlockSize);
            if (requiredOutputLength > outputBuffer.Length - outputOffset)
                throw new ArgumentOutOfRangeException(nameof(outputBuffer));

            ReadOnlySpan<byte> input = inputBuffer.AsSpan(inputOffset, inputCount);
            Span<byte> output = outputBuffer.AsSpan(outputOffset, requiredOutputLength);

            for (int i = 0; i < inputBlocks; i++)
            {
                output[i] = FromHex(input[i * 2], input[i * 2 + 1]);
            }

            return requiredOutputLength;
        }

        private static byte HexCharToByte(byte c)
        {
            if (c >= '0' && c <= '9')
                c -= (byte)'0';
            else if (c >= 'A' && c <= 'F')
                c -= (byte)'A' - 10;
            else if (c >= 'a' && c <= 'f')
                c -= (byte)'a' - 10;
            else
                throw new ArgumentOutOfRangeException(nameof(c));
            return c;
        }

        private static byte FromHex(byte a, byte b)
        {
            return (byte)(HexCharToByte(a) << 4 | HexCharToByte(b));
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var outputBuffer = new byte[inputCount / 2];
            TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0);
            return outputBuffer;
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }

        ~FromHexTransform()
        {
            Dispose(false);
        }
    }

    public class ToHexTransform : System.Security.Cryptography.ICryptoTransform
    {
        public bool CanReuseTransform => true;
        public bool CanTransformMultipleBlocks => true;
        public int InputBlockSize => 1;
        public int OutputBlockSize => 2;

        // to cover common cases and benefit from JIT's optimizations.
        private const int StackAllocSize = 32;

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (inputCount == 0)
                return 0;

            if (inputBuffer == null)
                throw new ArgumentNullException(nameof(inputBuffer));

            if (outputBuffer == null)
                throw new ArgumentNullException(nameof(outputBuffer));

            int requiredOutputLength = inputCount * 2;
            if (requiredOutputLength > outputBuffer.Length - outputOffset)
                throw new ArgumentOutOfRangeException(nameof(outputBuffer));

            ReadOnlySpan<byte> input = inputBuffer.AsSpan(inputOffset, inputCount);
            Span<byte> output = outputBuffer.AsSpan(outputOffset);

            for (int i = 0; i < inputCount; i++)
            {
                output[i * 2] = ToHex(input[i] >> 4);
                output[i * 2 + 1] = ToHex(input[i] & 0x0F);
            }

            return requiredOutputLength;
        }

        private static byte ToHex(int b)
        {
            if (b < 10)
                return (byte)('0' + b);
            return (byte)('A' + b - 10);
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            var outputBuffer = new byte[inputCount * 2];
            TransformBlock(inputBuffer, inputOffset, inputCount, outputBuffer, 0);
            return outputBuffer;
        }

        public void Dispose()
        {
            Clear();
        }

        public void Clear()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) { }

        ~ToHexTransform()
        {
            Dispose(false);
        }
    }

    public static partial class StreamExtension
    {

        // https://stackoverflow.com/questions/2630359/convert-stream-to-ienumerable-if-possible-when-keeping-laziness
        public static IEnumerable<byte> ToEnumerable(this Stream stream)
        {
            while (true)
            {
                int b = stream.ReadByte();
                if (b == -1)
                    break;
                yield return (byte)b;
            }
        }

        public static IEnumerable<char> ToEnumerable(this TextReader reader)
        {
            while (true)
            {
                int c = reader.Read();
                if (c == -1)
                    yield break;
                yield return (char)c;
            }
        }

        // https://stackoverflow.com/questions/6335153/casting-a-byte-array-to-a-managed-structure
        public static IEnumerable<T> ToEnumerable<T>(this Stream stream)
            where T : unmanaged
        {
            int sizeofT = Marshal.SizeOf(typeof(T));
            var buffer = new byte[sizeofT];
            while (true)
            {
                var read = stream.Read(buffer, 0, sizeofT);
                if (read == 0)
                    break;
                if (read != sizeofT)
                    throw new InvalidOperationException("Stream read error");
                yield return MemoryMarshal.Cast<byte, T>(buffer)[0];
            }
        }

        public static Stream DecodeBase64(this Stream stream)
        {
            return new CryptoStream(stream, new FromBase64Transform(), CryptoStreamMode.Read);
        }

        public static Stream EncodeBase64(this Stream stream)
        {
            return new CryptoStream(stream, new ToBase64Transform(), CryptoStreamMode.Write);
        }

        public static Stream DecodeHex(this Stream stream)
        {
            return new CryptoStream(stream, new FromHexTransform(), CryptoStreamMode.Read);
        }

        public static Stream EncodeHex(this Stream stream)
        {
            return new CryptoStream(stream, new ToHexTransform(), CryptoStreamMode.Write);
        }

    }
}
