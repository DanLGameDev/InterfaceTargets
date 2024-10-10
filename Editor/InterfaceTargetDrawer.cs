using System;
using UnityEngine;
using UnityEditor;

namespace DGP.InterfaceTargets.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceTarget<>))]
    //[CustomPropertyDrawer(typeof(RequireTargetAttribute))]
    public class InterfaceTargetDrawer : PropertyDrawer
    {
        private const float ErrorBoxHeight = 40f;
        private const float Padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetProp = property.FindPropertyRelative("target");

            var info = fieldInfo.GetCustomAttributes(typeof(RequireTargetAttribute), false);
            bool isRequired = info.Length > 0 && ((RequireTargetAttribute)info[0]).IsRequired;
            
            
            // Calculate rects
            Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect errorBoxRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Padding, position.width, ErrorBoxHeight);

            // Draw the object field
            EditorGUI.PropertyField(objectFieldRect, targetProp, label);
            

            // Validate the input
            if (targetProp.objectReferenceValue != null)
            {
                var interfaceTargetObject = fieldInfo.GetValue(property.serializedObject.targetObject);
                var validateMethod = interfaceTargetObject.GetType().GetMethod("ValidateInput");
                bool isValid = (bool)validateMethod.Invoke(interfaceTargetObject, null);
                
                if (!isValid)
                {
                    EditorGUI.HelpBox(
                        errorBoxRect,
                        $"Target must implement {fieldInfo.FieldType.GetGenericArguments()[0].Name}",
                        MessageType.Error
                    );
                }
            } else if (isRequired)
            {
                EditorGUI.HelpBox(
                    errorBoxRect,
                    "This field is required",
                    MessageType.Error
                );
            }

            EditorGUI.EndProperty();
        }
        
        private bool HasError(SerializedProperty property)
        {
            var info = fieldInfo.GetCustomAttributes(typeof(RequireTargetAttribute), false);
            bool isRequired = info.Length > 0 && ((RequireTargetAttribute)info[0]).IsRequired;
            
            var targetProp = property.FindPropertyRelative("target");
            
            var interfaceTargetObject = fieldInfo.GetValue(property.serializedObject.targetObject);
            var validateMethod = interfaceTargetObject.GetType().GetMethod("ValidateInput");
            
            bool hasValidValue = (bool)validateMethod.Invoke(interfaceTargetObject, null);
            
            if (isRequired && targetProp.objectReferenceValue == null)
                return true;
            
            if (!isRequired && targetProp.objectReferenceValue == null)
                return false;
            
            return !hasValidValue;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (HasError(property))
                return EditorGUIUtility.singleLineHeight + ErrorBoxHeight + Padding * 2;

            return EditorGUIUtility.singleLineHeight;
        }
    }
}