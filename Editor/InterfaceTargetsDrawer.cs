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

            var _targetComponentsProp = property.FindPropertyRelative("targetComponents");

            Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                float yOffset = EditorGUIUtility.singleLineHeight + Padding;

                // Draw list elements
                for (int i = 0; i < _targetComponentsProp.arraySize; i++)
                {
                    Rect elementRect = EditorGUI.IndentedRect(new Rect(position.x, position.y + yOffset, position.width - ButtonWidth - Padding, ElementHeight));
                    Rect buttonRect = new Rect(elementRect.xMax + Padding, elementRect.y, ButtonWidth, ElementHeight);

                    var _elementProp = _targetComponentsProp.GetArrayElementAtIndex(i);
                    bool isValidElement = ValidateElement(_elementProp);

                    // Change GUI color to red for invalid elements (including null)
                    if (!isValidElement)
                    {
                        GUI.color = new Color(1, 0.5f, 0.5f); // Light red
                    }

                    EditorGUI.PropertyField(elementRect, _elementProp, GUIContent.none);

                    // Reset GUI color
                    GUI.color = Color.white;

                    if (GUI.Button(buttonRect, "-"))
                    {
                        _targetComponentsProp.DeleteArrayElementAtIndex(i);
                    }

                    yOffset += ElementHeight + Padding;
                }

                // Add button with max width
                Rect addButtonRect = EditorGUI.IndentedRect(new Rect(position.x, position.y + yOffset, position.width, ElementHeight));
                float addButtonWidth = Mathf.Min(addButtonRect.width, AddButtonMaxWidth);
                addButtonRect.width = addButtonWidth;
                addButtonRect.x += (position.width - addButtonWidth) / 2; // Center the button

                if (GUI.Button(addButtonRect, "Add Element"))
                {
                    _targetComponentsProp.InsertArrayElementAtIndex(_targetComponentsProp.arraySize);
                    _targetComponentsProp.GetArrayElementAtIndex(_targetComponentsProp.arraySize - 1).objectReferenceValue = null;
                }

                EditorGUI.indentLevel--;
            }

            // Validate
            bool isValid = ValidateInterfaceTargets(property);
            
            // Show error if invalid (including empty list)
            if (!isValid)
            {
                Rect errorRect = new Rect(position.x, position.yMax - ErrorBoxHeight, position.width, ErrorBoxHeight);
                EditorGUI.HelpBox(errorRect, $"All elements must be non-null and implement {fieldInfo.FieldType.GetGenericArguments()[0].Name}", MessageType.Error);
            }

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