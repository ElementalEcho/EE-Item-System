using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.InventorySystem.Impl {
    [InlineEditor]
    [Serializable]
    [HideLabel]
    internal class InventoryDataSO : ScriptableObject, IHasInventoryData {
        [SerializeField]
        private int maxInventorySize = 5;
        [SerializeField]
        private List<InspectorItem> baseItems = new List<InspectorItem>();
        public IInventoryData InventoryData => new InventoryData(maxInventorySize, baseItems.GetItems());
    }


    [Serializable]
    internal class DefaultInventoryData : IInventoryData {
        public int MaxInventorySize => 1;
        public List<Item> BaseItems => new List<Item>();
    }
}

namespace EE.InventorySystem {
    public interface IHasInventoryData {
        IInventoryData InventoryData { get; }
    }


}