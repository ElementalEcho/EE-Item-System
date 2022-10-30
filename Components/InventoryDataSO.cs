using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.ItemSystem.Impl {
    [InlineEditor]
    [Serializable]
    [HideLabel]
    public class InventoryDataSO : ScriptableObject, IInventoryData {
        [SerializeField]
        protected int maxInventorySize = 5;
        public int MaxInventorySize => maxInventorySize;

        [SerializeField]
        protected List<InspectorItem> baseItems = new List<InspectorItem>();
        public List<Item> BaseItems => baseItems.GetItems();
    }


    [Serializable]
    public class DefaultInventoryData : IInventoryData {
        public int MaxInventorySize => 1;
        public List<Item> BaseItems => new List<Item>();
    }
}
