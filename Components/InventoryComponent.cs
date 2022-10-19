using EE.Core;
using EE.Core.PoolingSystem;
using EE.SaveSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EE.ItemSystem {
    public interface IInventoryComponent {
        Item CurrentItem { get; }
        int ItemCount { get; }
        bool InventoryFull { get; }
        bool ContainsItem(Item inventoryItem = null);
        void AddItem(Item inventoryItem);
        void RemoveAllItems(bool destroyItems = false);
        void RemoveItem(Item inventoryItem = null, bool destroyItems = false);
        void AddInventoryAlteredEvent(ItemDelegate.EEDelegate func);
        void AddItemAddedEvent(AddItemDelegate.EEDelegate func);
        void AddRemovedAddedEvent(RemoveItemDelegate.EEDelegate func);
        void NextItem();
        void PreviousItem();
        void ChangeItem(int index);
        void InventoryOpened();
        List<Item> GetItems();

    }
}

namespace EE.ItemSystem.Impl {
    internal class InventoryComponent : EEMonobehavior, IInventoryComponent, ISaveble, IHasComponents {
        [Header("Inventory Component")]
        [SerializeField]
        private InventoryDataSO inventoryDataSO = null;
        private IFacingDirection itemDropOffPoint = null;
        [SerializeField]
        private ItemDataBaseSO itemDataBaseSO = null;
        [ReadOnly]
        private IInventoryUser inventoryUser = null;
        private IInventoryUser Inventory {
            get {
                if (inventoryUser == null) {
                    if (inventorySO != null) {
                        inventoryUser = new InventoryUser(inventorySO);
                    }
                    else {
                        var inventory = new Inventory(InventoryData); 
                        inventoryUser = new InventoryUser(inventory);
                    }
                    inventoryUser.AddRemovedAddedEvent(DropItem);
                }
                return inventoryUser;
            }
        }

        [SerializeField]
        private InventorySO inventorySO;      
        public int ItemCount => Inventory.NumberOfFilledSlots;
        public List<Item> Items => Inventory.GetItems();
        public bool InventoryFull => Inventory.IsFull;

        public Item CurrentItem => Inventory.CurrentItem;


        [SerializeField]
        private ItemDropInfoContainer itemDropInfoContainer = new ItemDropInfoContainer();
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
        public void AddInventoryAlteredEvent(ItemDelegate.EEDelegate func) {
            Inventory.AddInventoryAlteredEvent(func);
        }
        public void AddItemAddedEvent(AddItemDelegate.EEDelegate func) {
            Inventory.AddItemAddedEvent(func);

        }
        public void AddRemovedAddedEvent(RemoveItemDelegate.EEDelegate func) {
            Inventory.AddRemovedAddedEvent(func);
        }
        public void AddItem(Item item) {
            //If inventory is full replace current item
            if (!Inventory.AddItem(item)) {
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
            Inventory.RemoveAllItems();
        }

        private void DropItem(IItemInfo item, int numberOfItems, bool destroyItems = false) {
            if (destroyItems) {
                return;
            }
            ItemDropInfo itemDropInfo = itemDropInfoContainer.GetDropPosition(itemDropOffPoint);
            var dropPosition = (Vector2)transform.position + itemDropInfo.dropPosition;
            var itemType = itemDataBaseSO.GetItemType(item.PrefabGuid);
            for (int i = 0; i < numberOfItems; i++) {
                IPoolable droppedItem = PoolManager.SpawnObject(itemType.ItemToDrop, transform.position, dropPosition, itemDropInfo.arcHight, itemDropInfo.dropDuration, itemDropInfo.dropRotationSpeed, itemDropInfo.rotationAngle);
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

        [Button]
        public void PrintNumberOfItems() {
            //System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            //foreach (var item in Inventory.Content) {
            //    if (!Item.IsNull(item)) {
            //        stringBuilder.AppendLine($"Item: { item.ItemInfo.ItemToDrop.name}. Amount: {item.NumberOfItems}.");
            //    }
            //}          
            //print(stringBuilder.ToString());
        }

        private class DefaultFacingDirectionProvider : IFacingDirection {
            public Vector2 FacingDirection => UnityEngine.Random.insideUnitCircle;

            public void SetFacingDirection(Vector2 vector2) {
                throw new NotImplementedException();
            }
        }
        public List<Item> GetItems() {
            return Inventory.GetItems();
        }
        public SaveComponentData GetSaveData() {
            var saveComponentData = new SaveComponentData();

            var itemSaveDatas = new List<ItemSaveData>();
            foreach (var item in Inventory.GetItems()) {
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
                    var intentorySaveData = JsonUtility.FromJson<InventorySaveData>(saveComponentData.jsonData);

                    var itemList = new List<Item>();
                    if (intentorySaveData.Items != null) {
                        foreach (var inventoryItem in intentorySaveData.Items) {
                            var itemtype = itemDataBaseSO.GetItemType(inventoryItem.itemGuid);
                            var item = new Item(itemtype.ItemType, inventoryItem.numberOfItems);
                            itemList.Add(item);
                        }
                    }

                    Inventory.LoadData(intentorySaveData, itemList);
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
    public class ItemDropInfoContainer {
        public float dropForce = 10;
        public float dropRange = 3;
        public float dropRotationSpeed = 3;
        public float dropRotationAngle = 90;
        public float arcHight = 90;

        public ItemDropInfo GetDropPosition(EE.Core.IFacingDirection facingDirection) {
            ItemDropInfo itemDropInfo = new ItemDropInfo();
            var direction = Mathf.Sign(facingDirection.FacingDirection.x);

            float pointX = Random.Range(dropRange / 2, dropRange);
            float pointY = Random.Range(0, dropRange / 2);
            itemDropInfo.dropPosition = new Vector2(pointX * direction, pointY / 2);

            itemDropInfo.rotationAngle = Random.Range(0, dropRotationAngle);

            itemDropInfo.dropDuration = dropRange / dropForce;
            itemDropInfo.arcHight = arcHight;
            itemDropInfo.dropRotationSpeed = dropRotationSpeed * direction;

            return itemDropInfo;
        }
    }
    public struct ItemDropInfo {
        public Vector2 dropPosition;
        public float arcHight;
        public float dropDuration;
        public float dropRotationSpeed;
        public float rotationAngle;
    }
}
