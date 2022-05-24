using UnityEngine;
using System.Linq;

public class SortingLayerProperty : PropertyAttribute
{
    public string[] layers;

    public SortingLayerProperty() {
        layers = SortingLayer.layers.Select(x => x.name).ToArray();
    }
}
