using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.ItemSystem.Impl {
    [InlineEditor]
    [Serializable]
    [HideLabel]
    public class InventoryDataSO : ScriptableObject, IHasInventoryData {
        [SerializeField]
        protected int maxInventorySize = 5;
        [SerializeField]
        protected List<InspectorItem> baseItems = new List<InspectorItem>();
        public IInventoryData InventoryData => new InventoryData(maxInventorySize, baseItems.GetItems());
    }


    [Serializable]
    public class DefaultInventoryData : IInventoryData {
        public int MaxInventorySize => 1;
        public List<Item> BaseItems => new List<Item>();
    }
}

namespace EE.ItemSystem {
    public interface IHasInventoryData {
        IInventoryData InventoryData { get; }
    }


}