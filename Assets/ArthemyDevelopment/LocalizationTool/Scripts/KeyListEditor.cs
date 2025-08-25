using System.Collections.Generic;
using ArthemyDevelopment.Localization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArthemyDevelopment.Localization
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class KeyListEditor
    {
        private static LocalizationData currentLoadedData;
        public static List<string> currentKeys;


#if UNITY_EDITOR
        static KeyListEditor()
        {
            if (PlayerPrefs.HasKey("KEYLISTEDITOR_LOADEDDATA"))
            {
                string temp = PlayerPrefs.GetString("KEYLISTEDITOR_LOADEDDATA");
                SetKeyList(JsonUtility.FromJson<LocalizationData>(temp));
            }
        }
#endif

        public static LocalizationData GetCurrentData()
        {
            return currentLoadedData;
        }

        public static void SetKeyList(LocalizationData data)
        {
            currentLoadedData = data;

            if (currentKeys != null)
            {
                currentKeys.Clear();

            }
            else
            {
                currentKeys = new List<string>();
            }

            if (currentLoadedData.LI_Items.Length != 0)
            {
                for (int i = 0; i < currentLoadedData.LI_Items.Length; i++)
                {

                    currentKeys.Add(currentLoadedData.LI_Items[i].key);
                }

            }

            string temp = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("KEYLISTEDITOR_LOADEDDATA", temp);


        }


    }
}
