using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EE.InventorySystem {
    public interface IInventory {
        Item CurrentItem { get; }
        int NumberOfFilledSlots { get; }
        bool IsFull { get; }
        bool IsFullAndStacksAreFull { get; }
        Item[] Content { get; }
        int CurrentItemIndex { get; }
        bool ContainsItem(IItemInfo item = null, int NumberOfItems = 1);
        bool AddItem(Item item);
        void RemoveAllItems();
        void RemoveItem(bool destroyItems = false, IItemInfo item = null, int NumberOfItems = 1);
        void AddInventoryAlteredEvent(Action func);
        void AddItemAddedEvent(Action<IItemInfo, int> func);
        void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func);
        void NextItem();
        void PreviousItem();
        void ChangeItem(int index);
        bool ItemHasFreeSlot(Item item);
        void InventoryOpened();
        void LoadData(InventorySaveData inventorySaveData, ItemDataBaseSO itemDataBaseSO);
    }
    [System.Serializable]
    public struct InventorySaveData {
        public int ItemIndex;
        public List<ItemSaveData> Items;
    }
    [System.Serializable]
    public struct ItemSaveData {
        public string itemGuid;
        public int numberOfItems;
    }
}
namespace EE.InventorySystem.Impl {
    [Serializable]
    internal class Inventory : IInventory {
        readonly IInventoryData inventoryData;
        public Inventory(IInventoryData inventoryData) {
            this.inventoryData = inventoryData;
            content = new Item[inventoryData.MaxInventorySize];
            foreach (var inventoryItem in inventoryData.BaseItems) {
                AddItem(inventoryItem);
            }
        }
        public void LoadData(InventorySaveData inventorySaveData, ItemDataBaseSO itemDataBaseSO) {
            _currentItemIndex = inventorySaveData.ItemIndex;
            content = new Item[inventoryData.MaxInventorySize];
            if (inventorySaveData.Items != null) {
                foreach (var inventoryItem in inventorySaveData.Items) {
                    var itemtype = itemDataBaseSO.GetItemType(inventoryItem.itemGuid);
                    var item = new Item(itemtype.ItemType, inventoryItem.numberOfItems);
                    AddItem(item);
                }
            }

        }

        private Item[] content;
        public Item[] Content => content;

        public int NumberOfFilledSlots { get {
                int numberOfItems = 0;
                for (int i = 0; i < content.Length; i++) {
                    if (!Item.IsNull(content[i])) {
                        numberOfItems++;
                    }
                }
                return numberOfItems;
            } 
        }
        public bool IsFull {
            get {
                for (int i = 0; i < content.Length; i++) {
                    if (Item.IsNull(content[i])) {
                        return false;
                    }
                }
                return true;
            }
        }
        public bool IsFullAndStacksAreFull {
            get {
                for (int i = 0; i < content.Length; i++) {
                    if (Item.IsNull(content[i]) || content[i].NumberOfItems < content[i].ItemInfo.MaxItemStack) {
                        return false;
                    }
                }
                return true;
            }
        }
        private event Action InventoryAlteredEvent;
        private event Action<IItemInfo, int> ItemAddedEvent;
        private event Action<IItemInfo, int, bool> ItemRemovedEvent;

        public Item CurrentItem => content.Length > _currentItemIndex && content[_currentItemIndex] != null? content[_currentItemIndex] :null;
        private int _currentItemIndex = 0;
        public int CurrentItemIndex => _currentItemIndex;

        public bool AddItem(Item itemToAdd) {
            bool retval = true;
            List<int> itemPositions = ContainsItem(itemToAdd.ItemInfo);
            int numberOfItems = itemToAdd.NumberOfItems;
            for (int i = 0; i < itemPositions.Count; i++) {
                if (numberOfItems <= 0) {
                    break;
                }
                Item itemSlot = content[itemPositions[i]];
                int itemsInSlot = itemToAdd.ItemInfo.MaxItemStack - itemSlot.NumberOfItems;
                if (numberOfItems > 0) {
                    int numberofItemsToAdd = Mathf.Min(itemsInSlot, numberOfItems);
                    content[itemPositions[i]].AddItems(numberofItemsToAdd);
                    numberOfItems -= numberofItemsToAdd;
                }
            }

            if (!IsFull) {
                AddItemToArray(itemToAdd.ItemInfo, numberOfItems);
            }
            else {
                retval = false;
            }

            ItemAddedEvent?.Invoke(itemToAdd.ItemInfo, itemToAdd.NumberOfItems);
            InventoryAlteredEvent?.Invoke();
            return retval;

        }
        public bool ItemHasFreeSlot(Item item) {
            List<int> itemPositions = ContainsItem(item.ItemInfo);
            for (int i = 0; i < itemPositions.Count; i++) {
                Item itemSlot = content[itemPositions[i]];
                int itemsInSlot = item.ItemInfo.MaxItemStack - itemSlot.NumberOfItems;
                if (itemsInSlot > 0) {
                    return true;
                }
            }
            return false;
        }
        protected bool AddItemToArray(IItemInfo itemInfo, int numberToAdd) {
            int i = 0;
            while (i < content.Length) {
                if (Item.IsNull(content[i])) {
                    content[i] = new Item(itemInfo, numberToAdd);
                    return true;
                }
                i++;
            }
            return false;
        }

        public void RemoveAllItems() {
            for (int i = 0; i < content.Length; i++) {
                content[i] = null;
            }
        }

        private List<int> ContainsItem(IItemInfo item) {
            List<int> list = new List<int>();

            for (int i = 0; i < content.Length; i++) {
                if (content[i] != null && content[i].ItemInfo == item) {
                    list.Add(i);
                }
            }
            return list;

        }

        public bool ContainsItem(IItemInfo item = null, int NumberOfItems = 1) {
            if (item == null) {
                return CurrentItem != null && CurrentItem.ItemInfo != null;
            }
            else {
                List<int> itemPositions = ContainsItem(item);
                if (itemPositions.Count <= 0) {
                    return false;
                }
                int numberOfItemsInInventory = 0;
                foreach (var index in itemPositions) {
                    numberOfItemsInInventory += content[index].NumberOfItems;
                    if (numberOfItemsInInventory >= NumberOfItems) {
                        return true;
                    }
                }

            }

            return false;
        }

        public void RemoveItem(bool destroyItems = false, IItemInfo item = null, int NumberOfItems = 1) {
            List<int> indexes;
            //Remove current item by default
            if (item == null && content.Length > 0) {
                Item inventoryItem = content[_currentItemIndex];
                item = inventoryItem.ItemInfo;
                NumberOfItems = inventoryItem.NumberOfItems;
                inventoryItem.RemoveItems(NumberOfItems);
                if (inventoryItem.NumberOfItems <= 0) {
                    content[_currentItemIndex] = null;
                }
            }
            else if ((indexes = ContainsItem(item)).Count > 0) {
                var numberOfItemsToRemove = NumberOfItems;
                for (int i = 0; i < indexes.Count; i++) {
                    content[indexes[i]].RemoveItems(numberOfItemsToRemove);
                    numberOfItemsToRemove -= content[indexes[i]].NumberOfItems;

                    if (content[indexes[i]].NumberOfItems <= 0) {
                        content[indexes[i]] = null;
                    }
                    if (numberOfItemsToRemove <= 0) {
                        break;
                    }
                }
            }
            ItemRemovedEvent?.Invoke(item, NumberOfItems, destroyItems);
            InventoryAlteredEvent?.Invoke();
        }


        public void NextItem() {
            _currentItemIndex++;
            if (_currentItemIndex >= content.Length) {
                _currentItemIndex = 0;
            }
            InventoryAlteredEvent?.Invoke();

        }
        public void PreviousItem() {
            _currentItemIndex--;
            if (_currentItemIndex <0 ) {
                _currentItemIndex = content.Length - 1;
            }
            InventoryAlteredEvent?.Invoke();
        }

        public void ChangeItem(int index) {
            _currentItemIndex = index;
            InventoryAlteredEvent?.Invoke();
        }

        public void AddInventoryAlteredEvent(Action func) {
            InventoryAlteredEvent += func;
        }
        public void AddItemAddedEvent(Action<IItemInfo, int> func) {
            ItemAddedEvent += func;
        }
        public void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func) {
            ItemRemovedEvent += func;
        }
        public void InventoryOpened() {
            Debug.LogWarning("Normal inventory does not implement Open inventoryEvent");
        }

    }

}
