using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SortingLayerProperty))]
public class SortingLayerDrawer : PropertyDrawer
{
    string[] layers;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var layerList = attribute as SortingLayerProperty;
        layers = layerList.layers;
        int selectedID = IDFromLayerName(property.stringValue);
        selectedID = EditorGUI.Popup(position, label.text, selectedID, layers);
        property.stringValue = LayerFromID(selectedID);
    }

    string LayerFromID(int id) {
        if (id < 0)
            return "None";
        return layers[id];
    }

    int IDFromLayerName(string name) {
        for (int i = 0; i<layers.Length; i++)
            if (layers[i] == name)
                return i;
        return -1;
    }
}
