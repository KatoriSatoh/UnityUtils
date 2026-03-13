using System;
using UnityEngine;

namespace UnityUtils
{
    [Serializable]
    public struct SortingLayerPicker
    {
        public int id;

        public readonly string Name => SortingLayer.IDToName(id);

        public static implicit operator int(SortingLayerPicker layerPicker)
        {
            return layerPicker.id;
        }
    }
}