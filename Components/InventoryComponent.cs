using EE.Core;
using EE.Core.PoolingSystem;
using EE.Core.ScriptableObjects;
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
                        IInventoryData inventoryData;
                        if (inventoryDataSO != null) {
                            inventoryData = inventoryDataSO;
                        }
                        else {
                            inventoryData = new DefaultInventoryData();
                        }
                        var inventory = new Inventory(inventoryData); 
                        inventoryUser = new InventoryUser(inventory);
                    }
                }
                return inventoryUser;
            }
        }



        protected IFacingDirection itemDropOffPoint = null;


        public Item CurrentItem => InventoryUser.CurrentItem;
        public int CurrentIndex => InventoryUser.CurrentIndex;
        public bool IsFull => InventoryUser.IsFull;
        public int NumberOfFilledSlots => InventoryUser.NumberOfFilledSlots;


        [SerializeField]
        private EventActivatorSO eventActivatorSO;

        public void Awake() {
            itemDropOffPoint = GetComponent<IFacingDirection>() ?? new DefaultFacingDirectionProvider();
            var genericActions = itemAddedActions.GetActions(this);

            if (genericActions.Length > 0) {
                Debug.LogError("events no longer supported.", this);
            }
            if (inventorySO != null) {
                inventorySO.InventoryAlteredEvent.Add(eventActivatorSO.Invoke);
            }

        }

        public void IncreaseIndex() {
            InventoryUser.IncreaseIndex();
        }
        public void DecreaseIndex() {
            InventoryUser.DecreaseIndex();
        }

        public void ChangeIndex(int index) {
            InventoryUser.ChangeIndex(index);
        }

        public bool Add(Item item) {
            return InventoryUser.Add(item);

        }
        public void Remove(IItemInfo item, int numberOfItems) {
            InventoryUser.Remove(item, numberOfItems);
        }

        public void RemoveAll() {
            InventoryUser.RemoveAll();
        }

        public bool Contains(IItemInfo item = null, int NumberOfItems = 1) {
            return InventoryUser.Contains(item, NumberOfItems);
        }

        private void DropItem(IItemInfo itemInfo, int numberOfItems) {

            ItemDropInfo itemDropInfo = itemDropInfoContainer.GetDropPosition(itemDropOffPoint);
            var dropPosition = (Vector2)transform.position + itemDropInfo.dropPosition;
            var itemType = itemDataBaseSO.GetItemType(itemInfo.ID);
            for (int i = 0; i < numberOfItems; i++) {
                IPoolable droppedItem = PoolManager.SpawnObject(itemType.ItemToDrop, transform.position, dropPosition, itemDropInfo.arcHight, itemDropInfo.dropDuration, itemDropInfo.dropRotationSpeed, itemDropInfo.rotationAngle);
            }
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
                //Commented since itemdatabase currently doesnt support guid, only the Iitemtype
                itemSave.itemGuid = item.ItemInfo.ID;
                itemSave.numberOfItems = item.NumberOfItems;
                itemSaveDatas.Add(itemSave);
            }
            

            var intentorySaveData = new InventorySaveData {
                ItemIndex = InventoryUser.CurrentIndex,
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

                    if (intentorySaveData.Items != null) {
                        foreach (var inventoryItem in intentorySaveData.Items) {
                            //Commented since itemdatabase currently doesnt support guid, only the Iitemtype
                            var itemtype = itemDataBaseSO.GetItemType(inventoryItem.itemGuid);
                            var item = new Item(itemtype.ItemType, inventoryItem.numberOfItems);
                            InventoryUser.Add(item);
                        }
                    }
                    inventoryUser.ChangeIndex(intentorySaveData.ItemIndex);
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
