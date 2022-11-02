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
    public class InventoryComponent : EEMonobehavior, ISaveble, IInventoryUser {
        [Header("Inventory Component")]
        [SerializeField]
        protected InventoryDataSO inventoryDataSO = null;

        [SerializeField]
        protected InventorySO inventorySO = null;
        [SerializeField]
        protected EventActivatorSO eventActivatorSO;

        [SerializeField]
        protected ItemDataBaseSO itemDataBaseSO = null;

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

        public Item CurrentItem => InventoryUser.CurrentItem;
        public int CurrentIndex => InventoryUser.CurrentIndex;
        public bool IsFull => InventoryUser.IsFull;
        public int NumberOfFilledSlots => InventoryUser.NumberOfFilledSlots;


        public void Awake() {

            if (inventorySO != null && eventActivatorSO != null) {
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

        [Button]
        public void PrintNumberOfItems() {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var item in InventoryUser.GetItems()) {
                if (item != null) {
                    stringBuilder.AppendLine($"Item: {item.ItemInfo}. Amount: {item.NumberOfItems}.");
                }
            }
            print(stringBuilder.ToString());
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
                            var item = new Item(itemtype, inventoryItem.numberOfItems);
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
}
