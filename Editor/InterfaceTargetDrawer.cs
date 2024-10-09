using UnityEngine;
using UnityEditor;

namespace DGP.InterfaceTargets.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceTarget<>))]
    public class InterfaceTargetDrawer : PropertyDrawer
    {
        private const float ErrorBoxHeight = 40f;
        private const float Padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetProp = property.FindPropertyRelative("target");
            var isValidProp = property.FindPropertyRelative("isValid");

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

                isValidProp.boolValue = isValid;

                if (!isValid)
                {
                    EditorGUI.HelpBox(
                        errorBoxRect,
                        $"Target must implement {fieldInfo.FieldType.GetGenericArguments()[0].Name}",
                        MessageType.Error
                    );
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var targetProp = property.FindPropertyRelative("target");
            var isValidProp = property.FindPropertyRelative("isValid");

            if (targetProp.objectReferenceValue != null && !isValidProp.boolValue)
            {
                return EditorGUIUtility.singleLineHeight + ErrorBoxHeight + Padding * 2;
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}