
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Text;

namespace KCommon
{
    static class KCommon
    {
        public static char[] UnsuportedSpeechLetter = "دجحخهعغفقثصضشسيبلاتنمكطظزوةىىلارؤءئ".ToCharArray();
    }
    public sealed class IsolatedStorageHelper
    {
        private IsolatedStorageHelper()
        {
        }
        public static T GetObject<T>(string key)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values.ContainsKey(key))
            {
                string serializedObject = localSettings.Values[key].ToString();
                return Deserialize<T>(serializedObject);
            }

            return default(T);
        }

        public static void SaveObject<T>(string key, T objectToSave)
        {
            string serializedObject = Serialize(objectToSave);
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[key] = serializedObject;
        }

        public static void DeleteObject(string key)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove(key);
        }

        public static string Serialize(object objectToSerialize)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(objectToSerialize.GetType());
                serializer.WriteObject(ms, objectToSerialize);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static T Deserialize<T>(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
    }

}
