using UnityEngine;
using UnityEditor;

namespace DGP.InterfaceTargets.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceTargets<>))]
    public class InterfaceTargetsDrawer : PropertyDrawer
    {
        private const float ElementHeight = 20f;
        private const float Padding = 2f;
        private const float ButtonWidth = 20f;
        private const float ErrorBoxHeight = 40f;
        private const float AddButtonMaxWidth = 200f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            

            EditorGUI.EndProperty();
        }

        private bool ValidateElement(SerializedProperty elementProp)
        {
            if (elementProp.objectReferenceValue == null)
            {
                return false; // Treat null as invalid
            }

            var _component = elementProp.objectReferenceValue as Component;
            return _component != null && _component.TryGetComponent(fieldInfo.FieldType.GetGenericArguments()[0], out _);
        }

        private bool ValidateInterfaceTargets(SerializedProperty property)
        {
            var _targetComponentsProp = property.FindPropertyRelative("targetComponents");
            
            if (_targetComponentsProp.arraySize == 0)
            {
                return true; // Treat empty list as invalid
            }
            
            for (int i = 0; i < _targetComponentsProp.arraySize; i++)
            {
                if (!ValidateElement(_targetComponentsProp.GetArrayElementAtIndex(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var _targetComponentsProp = property.FindPropertyRelative("targetComponents");

            float height = EditorGUIUtility.singleLineHeight; // Foldout

            if (property.isExpanded)
            {
                height += Padding;
                height += (ElementHeight + Padding) * (_targetComponentsProp.arraySize + 1); // List elements + Add button
            }

            bool isValid = ValidateInterfaceTargets(property);

            if (!isValid)
            {
                height += ErrorBoxHeight + Padding; // Error message
            }

            return height;
        }
    }
}