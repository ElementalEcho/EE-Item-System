using EE.Core;
using EE.Core.PoolingSystem;
using EE.SaveSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EE.ItemSystem.Impl {
    public class InventoryComponent : EEMonobehavior, ISaveble, IHasComponents, IInventoryUser {
        [Header("Inventory Component")]
        [SerializeField]
        protected InventoryDataSO inventoryDataSO = null;

        [SerializeField]
        protected InventorySO inventorySO = null;

        [SerializeField]
        protected ItemDataBaseSO itemDataBaseSO = null;

        [SerializeField]
        protected ItemDropInfoContainer itemDropInfoContainer = new ItemDropInfoContainer();
        [SerializeField]
        protected GenericActionSO[] itemAddedActions = new GenericActionSO[0];

        [ReadOnly]
        private IInventoryUser inventoryUser = null;
        private IInventoryUser InventoryUser {
            get {
                if (inventoryUser == null) {
                    if (inventorySO != null) {
                        inventoryUser = new InventoryUser(inventorySO);
                    }
                    else {
                        var inventoryData = inventoryDataSO != null ? inventoryDataSO.InventoryData : new DefaultInventoryData();
                        var inventory = new Inventory(inventoryData); 
                        inventoryUser = new InventoryUser(inventory);
                    }
                    inventoryUser.AddRemovedAddedEvent(DropItem);
                }
                return inventoryUser;
            }
        }



        protected IFacingDirection itemDropOffPoint = null;


        public Item CurrentItem => InventoryUser.CurrentItem;
        public int CurrentItemIndex => InventoryUser.CurrentItemIndex;
        public bool IsFull => InventoryUser.IsFull;
        public int NumberOfFilledSlots => InventoryUser.NumberOfFilledSlots;



        public void Awake() {
            itemDropOffPoint = GetComponent<IFacingDirection>() ?? new DefaultFacingDirectionProvider();
            var genericActions = itemAddedActions.GetActions(this);
            foreach (var genericAction in genericActions) {

                void action(IItemInfo itemInfo, int numberOfItems) { genericAction.Enter(); }
                AddItemAddedEvent(action);
            }
        }

        public void NextItem() {
            InventoryUser.NextItem();
        }
        public void PreviousItem() {
            InventoryUser.PreviousItem();
        }

        public void ChangeItem(int index) {
            InventoryUser.ChangeItem(index);
        }

        public bool AddItem(Item item) {
            //If inventory is full replace current item
            if (!InventoryUser.AddItem(item)) {
                RemoveItem();               
            }
            return InventoryUser.AddItem(item);

        }
        public void RemoveItem(bool destroyItems = false, IItemInfo item = null, int NumberOfItems = 1) {
            InventoryUser.RemoveItem(destroyItems, item, NumberOfItems);
        }

        public void RemoveAllItems() {
            InventoryUser.RemoveAllItems();
        }

        public bool ContainsItem(IItemInfo item = null, int NumberOfItems = 1) {
            return InventoryUser.ContainsItem(item, NumberOfItems);
        }

        private void DropItem(IItemInfo itemInfo, int numberOfItems, bool destroye) {
            List<IPoolable> droppedItems = new List<IPoolable>();
            if (destroye) {
                return;
            }
            ItemDropInfo itemDropInfo = itemDropInfoContainer.GetDropPosition(itemDropOffPoint);
            var dropPosition = (Vector2)transform.position + itemDropInfo.dropPosition;
            var itemType = itemDataBaseSO.GetItemType(itemInfo.PrefabGuid);
            for (int i = 0; i < numberOfItems; i++) {
                IPoolable droppedItem = PoolManager.SpawnObject(itemType.ItemToDrop, transform.position, dropPosition, itemDropInfo.arcHight, itemDropInfo.dropDuration, itemDropInfo.dropRotationSpeed, itemDropInfo.rotationAngle);
            }
        }

        public void LoadData(InventorySaveData inventorySaveData, List<Item> items) {
            InventoryUser.LoadData(inventorySaveData, items);
        }

        public void AddInventoryAlteredEvent(ItemDelegate.EEDelegate func) {
            InventoryUser.AddInventoryAlteredEvent(func);
        }
        public void AddItemAddedEvent(AddItemDelegate.EEDelegate func) {
            InventoryUser.AddItemAddedEvent(func);
        }
        public void AddRemovedAddedEvent(RemoveItemDelegate.EEDelegate func) {
            InventoryUser.AddRemovedAddedEvent(func);
        }

        public void InventoryOpened() {
            InventoryUser.InventoryOpened();
        }

        [Button]
        public void PrintNumberOfItems() {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var item in InventoryUser.GetItems()) {
                if (!Item.IsNull(item)) {
                    stringBuilder.AppendLine($"Item: {item.ItemInfo}. Amount: {item.NumberOfItems}.");
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
        public List<Item> GetItems() {
            return InventoryUser.GetItems();
        }
        public SaveComponentData GetSaveData() {
            var saveComponentData = new SaveComponentData();

            var itemSaveDatas = new List<ItemSaveData>();
            foreach (var item in InventoryUser.GetItems()) {
                if (item == null ||item.ItemInfo == null) {
                    continue;
                }
                var itemSave = new ItemSaveData();
                itemSave.itemGuid = item.ItemInfo.PrefabGuid;
                itemSave.numberOfItems = item.NumberOfItems;
                itemSaveDatas.Add(itemSave);
            }
            

            var intentorySaveData = new InventorySaveData {
                ItemIndex = InventoryUser.CurrentItemIndex,
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

                    InventoryUser.LoadData(intentorySaveData, itemList);
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
