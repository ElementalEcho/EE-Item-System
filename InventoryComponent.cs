using EE.Core;
using EE.Core.PoolingSystem;
using EE.Core.ScriptableObjects;
using EE.SaveSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EE.InventorySystem {
    public interface IInventoryComponent {
        Item CurrentItem { get; }
        int ItemCount { get; }
        bool InventoryFull { get; }
        bool IsFullAndStacksAreFull { get; }
        Item[] Items { get; }
        bool CanAddItem(Item inventoryItem);
        bool ContainsItem(Item inventoryItem = null);
        void AddItem(Item inventoryItem);
        void RemoveAllItems(bool destroyItems = false);
        void RemoveItem(Item inventoryItem = null, bool destroyItems = false);
        void AddInventoryAlteredEvent(Action func);
        void AddItemAddedEvent(Action<IItemInfo, int> func);
        void AddRemovedAddedEvent(Action<IItemInfo, int,bool> func);
        void NextItem();
        void PreviousItem();
        void ChangeItem(int index);
        void InventoryOpened();

    }
}

namespace EE.InventorySystem.Impl {
    internal class InventoryComponent : EEMonobehavior, IInventoryComponent, ISaveble, IHasComponents {
        [Header("Inventory Component")]
        [SerializeField]
        private InventoryDataSO inventoryDataSO = null;
        private IFacingDirection itemDropOffPoint = null;
        [SerializeField]
        private ItemDataBaseSO itemDataBaseSO = null;
        [ReadOnly]
        private IInventory inventory = null;
        private IInventory Inventory {
            get {
                if (inventory == null) {
                    if (inventorySO != null) {
                        inventory = inventorySO;
                    }
                    else {
                        inventory = new Inventory(InventoryData);
                    }
                    inventory.AddRemovedAddedEvent(DropItem);
                }
                return inventory;
            }
        }

        [SerializeField]
        private InventorySO inventorySO;      
        public int ItemCount => Inventory.NumberOfFilledSlots;
        public Item[] Items => Inventory.Content;
        public bool InventoryFull => Inventory.IsFull;
        public bool IsFullAndStacksAreFull => Inventory.IsFullAndStacksAreFull;

        public Item CurrentItem => Inventory.CurrentItem;

        
        private IInventoryData InventoryData => inventoryDataSO != null ? inventoryDataSO.InventoryData : new DefaultInventoryData();

        [SerializeField]
        private GenericActionSO[] itemAddedActions = new GenericActionSO[0];

        public void Awake() {
            itemDropOffPoint = GetComponent<IFacingDirection>() ?? new DefaultFacingDirectionProvider();
            var genericActions = itemAddedActions.GetActions(this);
            foreach (var genericAction in genericActions) {

                void action(IItemInfo info, int count) { genericAction.Enter(); }
                AddItemAddedEvent(action);
            }
        }
        private void ItemAddedAction(IItemInfo itemInfo, int itemCount) { 
        
        }
        public void AddInventoryAlteredEvent(Action func) {
            Inventory.AddInventoryAlteredEvent(func);
        }
        public void AddItemAddedEvent(Action<IItemInfo,int> func) {
            Inventory.AddItemAddedEvent(func);

        }
        public void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func) {
            Inventory.AddRemovedAddedEvent(func);
        }
        public void AddItem(Item item) {
            //If inventory is full replace current item
            if (!CanAddItem(item)) {
                RemoveItem();               
            }
            Inventory.AddItem(item);

        }

        public void RemoveItem(Item inventoryItem = null, bool destroyItems = false) {
            if (inventoryItem == null) {
                Inventory.RemoveItem(destroyItems);
            }
            else {
                Inventory.RemoveItem(destroyItems,inventoryItem.ItemInfo, inventoryItem.NumberOfItems);
            }
        }
        public void RemoveAllItems(bool destroyItems = false) {
            ChangeItem(0);
            for (int i = Inventory.Content.Length - 1; i >= 0; i--) {
                RemoveItem(Inventory.Content[i], destroyItems);
                NextItem();
            }
        }

        private void DropItem(IItemInfo item, int numberOfItems, bool destroyItems = false) {
            if (destroyItems) {
                return;
            }
            InventoryData inventoryData = inventoryDataSO != null ? (InventoryData)inventoryDataSO.InventoryData : null;
            ItemDropInfo itemDropInfo = inventoryData.ItemDropInfoContainer.GetDropPosition(itemDropOffPoint);
            var dropPosition = (Vector2)transform.position + itemDropInfo.dropPosition;
            for (int i = 0; i < numberOfItems; i++) {
                IPoolable droppedItem = PoolManager.SpawnObject(item.ItemToDrop, transform.position, dropPosition, itemDropInfo.arcHight, itemDropInfo.dropDuration, itemDropInfo.dropRotationSpeed, itemDropInfo.rotationAngle);
            }
        }

        public bool ContainsItem(Item inventoryItem = null) {
            return inventoryItem == null ? 
                Inventory.ContainsItem() :
                Inventory.ContainsItem(inventoryItem.ItemInfo, inventoryItem.NumberOfItems);
        }

        public void NextItem() {
            Inventory.NextItem();
        }
        public void PreviousItem() {
            Inventory.PreviousItem();
        }

        public void ChangeItem(int index) {
            Inventory.ChangeItem(index);
        }
        public void InventoryOpened() {
            Inventory.InventoryOpened();
        }
        public bool CanAddItem(Item inventoryItem) {
            return !InventoryFull || Inventory.ItemHasFreeSlot(inventoryItem);
        }

        [Button]
        public void PrintNumberOfItems() {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var item in Inventory.Content) {
                if (!Item.IsNull(item)) {
                    stringBuilder.AppendLine($"Item: { item.ItemInfo.ItemToDrop.name}. Amount: {item.NumberOfItems}.");
                }
            }          
            print(stringBuilder.ToString());
        }

        private class DefaultFacingDirectionProvider : IFacingDirection {
            public Vector2 FacingDirection => UnityEngine.Random.insideUnitCircle;

            public void SetFacingDirection(Vector2 vector2) {
                throw new NotImplementedException();
            }
        }
        public SaveComponentData GetSaveData() {
            var saveComponentData = new SaveComponentData();

            var itemSaveDatas = new List<ItemSaveData>();
            foreach (var item in Inventory.Content) {
                if (item == null ||item.ItemInfo == null) {
                    continue;
                }
                var itemSave = new ItemSaveData();
                itemSave.itemGuid = item.ItemInfo.PrefabGuid;
                itemSave.numberOfItems = item.NumberOfItems;
                itemSaveDatas.Add(itemSave);
            }
            

            var intentorySaveData = new InventorySaveData {
                ItemIndex = Inventory.CurrentItemIndex,
                Items = itemSaveDatas
            };
            saveComponentData.componentName = typeof(InventoryComponent).FullName;
            saveComponentData.jsonData = JsonUtility.ToJson(intentorySaveData);
            return saveComponentData;
        }

        public void Load(List<SaveComponentData> saveComponentDatas) {
            foreach (var saveComponentData in saveComponentDatas) {
                if (saveComponentData.componentName == typeof(InventoryComponent).FullName) {
                    var intentorySaveData = JsonUtility.FromJson<InventorySaveData>(saveComponentData.jsonData); ;
                    Inventory.LoadData(intentorySaveData, itemDataBaseSO);
                }
            }
        }



#if UNITY_EDITOR
        private void OnValidate() {

        }
        private static string inventoryDataPathStart = "Assets/Resources/ScriptableObjects/Items/InventoryDataSOs/";
        private static string inventoryDataPathEnding = "_InventoryData.asset";
        public static string GetPhysicsDataPath(string gameobjectName) {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(inventoryDataPathStart);
            stringBuilder.Append(gameobjectName);
            stringBuilder.Append(inventoryDataPathEnding);
            return stringBuilder.ToString();
        }

        [ShowIf("@this.inventoryDataSO == null")]
        [Button, PropertyOrder(-10)]
        private void ButtonCreateInventoryDataSO() {
            var path = GetPhysicsDataPath(gameObject.name);
            InventoryDataSO inventoryDataSO = ScriptableObject.CreateInstance<InventoryDataSO>();

            path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path);
            UnityEditor.AssetDatabase.CreateAsset(inventoryDataSO, path);
            UnityEditor.AssetDatabase.SaveAssets();

            this.inventoryDataSO = inventoryDataSO;
        }
#endif

    }

}
