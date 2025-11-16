using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationProtocol.Extensions
{
    public static class ProtocolExtension
    {

        private static readonly int[] _hexLookup = new int[256];

        static ProtocolExtension()
        {
            HexLookupInitializer();
        }

        private static void HexLookupInitializer()
        {
            for (int i = 0; i < _hexLookup.Length; i++)
                _hexLookup[i] = -1;

            for (int i = 0; i < 10; i++)
                _hexLookup['0' + i] = i;
            for (int i = 0; i < 6; i++)
            {
                _hexLookup['A' + i] = 10 + i;
                _hexLookup['a' + i] = 10 + i;
            }
        }

        public static string ToHexString(this byte[] bytes, char? separator = null)
        {

            return ((ReadOnlySpan<byte>)bytes).ToHexString(separator);
        }

        public static string ToHexString(this ReadOnlyMemory<byte> bytes, char? separator = null)
        {

            return bytes.Span.ToHexString(separator);
        }

        public static string ToHexString(this ReadOnlySpan<byte> bytes, char? separator = null)
        {
            if (bytes.Length == 0)
                return string.Empty;

            const string hexChars = "0123456789ABCDEF";

            var length = separator.HasValue ? bytes.Length * 3 - 1 : bytes.Length * 2;

            Span<char> chars = stackalloc char[length];

            for (int i = 0; i < bytes.Length; i++)
            {
                var charIndex = separator.HasValue ? i * 3 : i * 2;
                chars[charIndex] = hexChars[bytes[i] >> 4];
                chars[charIndex + 1] = hexChars[bytes[i] & 0x0F];

                if (separator.HasValue && i < bytes.Length - 1)
                {
                    chars[charIndex + 2] = separator.Value;
                }
            }

            return new string(chars);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ToHexChar(this byte value)
        {
            return (byte)(value < 10 ? value + '0' : value - 10 + 'A');
        }

        public static bool TryParseAsciiHex(this ReadOnlyMemory<byte> asciiHex, Span<byte> output)
        {
            return asciiHex.Span.TryParseAsciiHex(output);
        }

        public static bool TryParseAsciiHex(this ReadOnlySpan<byte> asciiHex, Span<byte> output)
        {

            if (asciiHex.Length % 2 != 0 || output.Length < asciiHex.Length / 2)
                return false;

            for (int i = 0; i < output.Length; i++)
            {
                int high = asciiHex[i * 2].ParseHexChar();
                int low = asciiHex[i * 2 + 1].ParseHexChar();

                if (high < 0 || low < 0) return false;

                output[i] = (byte)((high << 4) | low);
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseHexChar(this byte value)
        {
            if (value >= '0' && value <= '9') return value - '0';
            if (value >= 'A' && value <= 'F') return value - 'A' + 10;
            if (value >= 'a' && value <= 'f') return value - 'a' + 10;
            return -1;
        }

        public static T ToUnmanagedType<T>(this ReadOnlyMemory<byte> memoryBytes) where T : unmanaged
        {

            return memoryBytes.Span.ToBigEndianUnmanagedType<T>();
        }

        public static T ToBigEndianUnmanagedType<T>(this ReadOnlyMemory<byte> memoryBytes, int elementSize) where T : unmanaged
        {
            return memoryBytes.Span.ToBigEndianUnmanagedType<T>(elementSize);
        }

        public static T ToBigEndianUnmanagedType<T>(this ReadOnlySpan<byte> values) where T : unmanaged
        {
            var elementSize = Unsafe.SizeOf<T>();

            return values.ToBigEndianUnmanagedType<T>(elementSize);
        }

        public static T ToBigEndianUnmanagedType<T>(this ReadOnlySpan<byte> values, int elementSize) where T : unmanaged
        {
            return values.ToUnmanagedType<T>(elementSize);
        }

        public static T ToLittleEndianUnmanagedType<T>(this ReadOnlyMemory<byte> memoryBytes) where T : unmanaged
        {
            var elementSize = Unsafe.SizeOf<T>();
            return memoryBytes.Span.ToLittleEndianUnmanagedType<T>(elementSize);
        }

        public static T ToLittleEndianUnmanagedType<T>(this ReadOnlyMemory<byte> memoryBytes, int elementSize) where T : unmanaged
        {
            return memoryBytes.Span.ToLittleEndianUnmanagedType<T>(elementSize);
        }

        public static T ToLittleEndianUnmanagedType<T>(this ReadOnlySpan<byte> values, int elementSize) where T : unmanaged
        {
            return values.ToUnmanagedType<T>(elementSize, false);
        }

        public static T ToUnmanagedType<T>(this ReadOnlySpan<byte> values, int elementSize, bool IsReverse = true) where T : unmanaged
        {
            if (values.Length != elementSize)
                throw new ArgumentException($"Byte array length {values.Length} related to type size {elementSize} do not match", nameof(values));

            if (IsReverse && elementSize > 1)
            {
                Span<byte> reversedBytes = stackalloc byte[elementSize];
                values.CopyTo(reversedBytes);
                reversedBytes.Reverse();
                return MemoryMarshal.Read<T>(reversedBytes);
            }

            return MemoryMarshal.Read<T>(values);

        }

        public static ReadOnlyMemory<byte> ToLittleEndianBytes<T>(this T value) where T : unmanaged
        {
            return value.ToBytes(false);
        }

        public static ReadOnlyMemory<byte> ToBigEndiannBytes<T>(this T value) where T : unmanaged
        {
            return value.ToBytes();
        }

        public static ReadOnlyMemory<byte> ToBytes<T>(this T value, bool IsReverse = true) where T : unmanaged
        {
            var elementSize = Unsafe.SizeOf<T>();

            Span<byte> spanBytes = stackalloc byte[elementSize];

            MemoryMarshal.Write(spanBytes, ref value);

            if (IsReverse && spanBytes.Length > 1)
            {
                spanBytes.Reverse();
            }

            return spanBytes.ToArray();
        }

        public static async Task<ReadOnlyMemory<byte>> ReadExactAsync(this NetworkStream networkStream, int length, ushort receiveTimeout)
        {
            return await networkStream.ReadExactAsync((uint)length, receiveTimeout);
        }

        public static async Task<ReadOnlyMemory<byte>> ReadExactAsync(this NetworkStream networkStream, uint length, ushort receiveTimeout)
        {

            using var cts = new CancellationTokenSource(receiveTimeout);

            var offset = 0;
            Memory<byte> buffer = new byte[length];
            while (offset < buffer.Length)
            {
                int bytesRead = await networkStream.ReadAsync(buffer[offset..], cts.Token);

                if (bytesRead == 0)
                    throw new IOException("The TCP connection has been disconnected.");

                offset += bytesRead;
            }

            return buffer;

        }

        public static ReadOnlyMemory<byte> GetZeroPaddingBytes(this string value)
        {
            var valueBytesCount = Encoding.UTF8.GetByteCount(value);

            var Length = valueBytesCount % 2;

            Span<byte> zeroPaddingBytes = new byte[valueBytesCount + Length];

            Encoding.UTF8.GetBytes(value, zeroPaddingBytes);
            if (Length == 1)
            {
                zeroPaddingBytes[^1] = 0x00;
            }

            return zeroPaddingBytes.ToArray();
        }

        public static ushort GetDivideTwoCount(this int value)
        {
            return (ushort)(value > 1 ? value / 2 : 1);
        }

        public static ushort GetRemainderPaddingCount(this byte value)
        {
            return (ushort)(value / 2 + value % 2);
        }
    }
}
