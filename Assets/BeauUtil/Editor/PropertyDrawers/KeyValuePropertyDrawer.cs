using System;
using UnityEditor;
using UnityEngine;

namespace BeauUtil.Editor
{
    [CustomPropertyDrawer(typeof(KeyValuePairAttribute))]
    internal sealed class KeyValuePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            KeyValuePairAttribute attr = (KeyValuePairAttribute) attribute;

            SerializedProperty keyProp = property.FindPropertyRelative(attr.KeyPropertyName);
            SerializedProperty valProp = property.FindPropertyRelative(attr.ValuePropertyName);

            GUIContent newLabel = new GUIContent(label);

            newLabel.text = keyProp.stringValue;
            if (string.IsNullOrEmpty(newLabel.text))
            {
                newLabel.text = label.text;
            }

            newLabel = EditorGUI.BeginProperty(position, newLabel, property);
            position = EditorGUI.PrefixLabel(position, newLabel);

            using(GUIScopes.IndentLevelScope.SetIndent(0))
            {
                Rect keyPos = position;
                keyPos.width = (keyPos.width * 0.5f) - 2;

                EditorGUI.PropertyField(keyPos, keyProp, GUIContent.none, false);

                Rect valPos = keyPos;
                valPos.x += valPos.width + 4;

                EditorGUI.PropertyField(valPos, valProp, GUIContent.none, false);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}