using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ArthemyDevelopment.Localization
{

    public class KeysPopUp : PropertyAttribute
    {
        public Type myType = typeof(KeyListEditor);
        public string propertyName = "currentKeys";

    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(KeysPopUp))]
    public class KeysPopUpDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            KeysPopUp keys = attribute as KeysPopUp;
            List<string> stringList = null;

            if (keys.myType.GetField(keys.propertyName) != null)
                stringList = keys.myType.GetField(keys.propertyName).GetValue(keys.myType) as List<string>;

            if (stringList != null && stringList.Count != 0)
            {
                int selectedIndex = Mathf.Max(stringList.IndexOf(property.stringValue), 0);
                selectedIndex = EditorGUI.Popup(position, property.name, selectedIndex, stringList.ToArray());
                property.stringValue = stringList[selectedIndex];
            }
            else EditorGUI.PropertyField(position, property, label);

        }
    }

#endif

}