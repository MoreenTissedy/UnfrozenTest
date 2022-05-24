using UnityEngine;
using System.Linq;

//This is one of my common scripts, it allows us to choose a sorting layer name from a list in the inspector
//when the string field is marked [SortingLayerProperty] 
public class SortingLayerProperty : PropertyAttribute
{
    public string[] layers;

    public SortingLayerProperty() {
        layers = SortingLayer.layers.Select(x => x.name).ToArray();
    }
}
