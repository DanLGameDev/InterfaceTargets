using System;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace DGP.InterfaceTargets.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceTarget<>))]
    public class InterfaceTargetDrawer : PropertyDrawer
    {
        private const float ErrorBoxHeight = 40f;
        private const float Padding = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DropAreaGUI(position, property);
            EditorGUI.BeginProperty(position, label, property);

            var targetProp = property.FindPropertyRelative("target");
            var isRequired = Attribute.IsDefined(fieldInfo, typeof(RequireTargetAttribute));
            
            // Calculate rects
            Rect objectFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect errorBoxRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Padding, position.width, ErrorBoxHeight);

            // Draw the object field and handle custom drag and drop
            //DrawCustomObjectField(objectFieldRect, targetProp, label, property);
            EditorGUI.PropertyField(objectFieldRect, targetProp, label);
            
            // Show error messages
            if (targetProp.objectReferenceValue == null)
            {
                if (isRequired)
                {
                    EditorGUI.HelpBox(
                        errorBoxRect,
                        "This field is required",
                        MessageType.Error
                    );
                }
            }
            else if (!IsValidTarget(property, targetProp.objectReferenceValue, out _))
            {
                EditorGUI.HelpBox(
                    errorBoxRect,
                    $"Target must implement {fieldInfo.FieldType.GetGenericArguments()[0].Name}",
                    MessageType.Error
                );
            }

            EditorGUI.EndProperty();
        }
        
        public void DropAreaGUI(Rect dropArea, SerializedProperty property)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition) || DragAndDrop.objectReferences.Length > 1)
                        return;

                    Object draggedObject = DragAndDrop.objectReferences[0];

                    var isValidTarget = IsValidTarget(property, draggedObject, out var validObject);
                    if (!isValidTarget)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        return;
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    break;
            }
        }
        
        private bool IsValidTarget(SerializedProperty property, Object targetValue, out Object validObject)
        {
            var interfaceType = fieldInfo.FieldType.GetGenericArguments()[0];
            
            if (interfaceType.IsInstanceOfType(targetValue)) {
                validObject = targetValue;
                return true;
            }

            if (targetValue is Component component) {
                validObject = component.GetComponent(interfaceType);
                return validObject != null;
            }

            if (targetValue is GameObject gameObject) {
                validObject = gameObject.GetComponent(interfaceType);
                return validObject != null;
            }

            validObject = null;
            return false;
        }

        private bool HasError(SerializedProperty property)
        {
            var isRequired = Attribute.IsDefined(fieldInfo, typeof(RequireTargetAttribute));
            var targetProp = property.FindPropertyRelative("target");
            
            if (targetProp.objectReferenceValue == null)
                return isRequired;
            
            return !IsValidTarget(property, targetProp.objectReferenceValue, out _);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return HasError(property) 
                ? EditorGUIUtility.singleLineHeight + ErrorBoxHeight + Padding * 2
                : EditorGUIUtility.singleLineHeight;
        }
    }
}