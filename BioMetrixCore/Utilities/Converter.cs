using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Utilities
{
    public class Converter
    {
        private static JsonSerializerSettings _serializerSetting = null;
        private static JsonSerializerSettings SerializerSetting
        {
            get
            {
                if (_serializerSetting == null)
                {
                    _serializerSetting = new JsonSerializerSettings()
                    {
                        //sNullValueHandling = NullValueHandling.Ignore,
                        Error = HandleDeserializationError,
                        Formatting = Formatting.Indented
                    };
                }
                return _serializerSetting;
            }
        }

        public static string JsonSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSetting);
        }

        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializerSetting);
        }

        public static void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            // Logger.LogError($"JsonDeserialize Error: {currentError}");
            errorArgs.ErrorContext.Handled = true;
        }

        /// <summary>
        /// Mã hóa base64
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            if (plainText == null)
            {
                return null;
            }
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Giải mã base64
        /// </summary>
        /// <param name="base64Text"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            if (base64EncodedData == null)
            {
                return null;
            }
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
