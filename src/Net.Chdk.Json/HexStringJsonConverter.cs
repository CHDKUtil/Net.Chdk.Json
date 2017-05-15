using Newtonsoft.Json;
using System;

namespace Net.Chdk.Json
{
    public sealed class HexStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(uint).Equals(objectType) || typeof(ulong).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue($"0x{value:x}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var str = reader.Value as string;
            if (str == null)
                return null;
            try
            {
                var fromBase = GetBase(str);
                if (objectType == typeof(uint))
                    return Convert.ToUInt32(str, fromBase);
                return Convert.ToUInt64(str, fromBase);
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException("Error deserializing", ex);
            }
        }

        private static int GetBase(string str)
        {
            if (str.Length < 2 || str[0] != '0')
                return 10;

            switch (str[1])
            {
                case 'b':
                case 'B':
                    return 2;
                case 'x':
                case 'X':
                    return 16;
                default:
                    return 8;
            }
        }
    }
}
