using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FontStashSharp;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Shimakaze.Framework.Brushes;
using Shimakaze.LogTracer;

namespace Shimakaze.Framework
{
    public static class StringConverter
    {
        public static bool ToBoolean(string s)
        {
            if (!bool.TryParse(s, out bool result))
            {
                if (int.TryParse(s, out int i))
                {
                    result = i == 0;
                }
                else
                {
                    var ch = s[0];
                    if (ch > 112)
                        ch -= ' ';
                    result = ch switch
                    {
                        'T' => true,
                        'Y' => true,
                        _ => false,
                    };
                }
            }
            return result;
        }

        public static Vector4 ToVector4(string s)
        {
            float[] args = s.Split(',', ' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(float.Parse).ToArray();
            return args.Length switch
            {
                1 => new Vector4(args[0]),
                2 => new Vector4(args[0], args[1], args[0], args[1]),
                4 => new Vector4(args[0], args[1], args[2], args[3]),
                _ => s.ThrowHelper<Vector4>(),
            };
        }

        internal static TEnum ToEnum<TEnum>(string s) where TEnum : struct => Enum.TryParse(s, true, out TEnum result) ? result : s.ThrowHelper<TEnum>();

        internal static float ToSingle(string s) => float.TryParse(s, out float i) ? i : s.ThrowHelper<float>();
        internal static double ToDouble(string s) => double.TryParse(s, out double i) ? i : s.ThrowHelper<double>();

        internal static Brush ToBrush(string s) => ToTexture2D(s).ToBrush();
        internal static Texture2D ToTexture2D(string s)
        {
            if (s.StartsWith("#"))
                return Application.Current!.Graphics!.CreateTexture2D(ToColor(s), 1, 1);

            if (File.Exists(s))
                return Application.Current!.Content.Load<Texture2D>(s);

            return s.ThrowHelper<Texture2D>();
        }

        internal static Vector2 ToVector2(string s)
        {
            float[] args = s.Split(',', ' ').Where(s => !string.IsNullOrWhiteSpace(s)).Select(float.Parse).ToArray();
            return args.Length switch
            {
                1 => new Vector2(args[0]),
                2 => new Vector2(args[0], args[1]),
                _ => s.ThrowHelper<Vector2>(),
            };
        }

        internal static sbyte ToInt8(string s) => sbyte.TryParse(s, out sbyte i) ? i : s.ThrowHelper<sbyte>();
        internal static byte ToUInt8(string s) => byte.TryParse(s, out byte i) ? i : s.ThrowHelper<byte>();
        internal static short ToInt16(string s) => short.TryParse(s, out short i) ? i : s.ThrowHelper<short>();
        internal static ushort ToUInt16(string s) => ushort.TryParse(s, out ushort i) ? i : s.ThrowHelper<ushort>();
        internal static int ToInt32(string s) => int.TryParse(s, out int i) ? i : s.ThrowHelper<int>();
        internal static uint ToUInt32(string s) => uint.TryParse(s, out uint i) ? i : s.ThrowHelper<uint>();
        internal static long ToInt64(string s) => long.TryParse(s, out long i) ? i : s.ThrowHelper<long>();
        internal static ulong ToUInt64(string s) => ulong.TryParse(s, out ulong i) ? i : s.ThrowHelper<ulong>();

        private static T ThrowHelper<T>(this string s) => throw new NotSupportedException($"Cannot Convert {s} to {typeof(T).FullName}");

        internal static FontSystem ToFontSystem(string s)
        {
            FontSystem result = FontSystemFactory.Create(Application.Current!.GraphicsDevice);
            foreach (var i in s.Split(','))
                result.AddFont(i);
            return result;
        }

        internal static Color ToColor(string value)
        {
            if (value.StartsWith("#"))
            {
                value = value.Substring(1);
                var color = Convert.ToUInt32(value, 16);
                return value.Length == 8
                    ? new Color(color & 0xFF0000, color & 0xFF00, color & 0xFF, color & 0xFF000000)
                    : new Color(color & 0xFF0000, color & 0xFF00, color & 0xFF);
            }

            return (Color)typeof(Color).GetProperty(value)!.GetValue(null)!;
        }
    }
}
