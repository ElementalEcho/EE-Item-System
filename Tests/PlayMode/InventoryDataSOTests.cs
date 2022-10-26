using EE.ItemSystem.Impl;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace EE.ItemSystem.PlayMode {
    public class InventoryDataSOTests {
        internal class TestInventoryDataSO : InventoryDataSO {
            public void SetValues(int maxInventorySize, List<InspectorItem> baseItems) {
                this.maxInventorySize = maxInventorySize;
                this.baseItems = baseItems;
            }
        }

        [UnityTest]
        public IEnumerator InventoryDataSO_Should_Have_DefaultValues() {
            var inventoryDataSO = ScriptableObject.CreateInstance<InventoryDataSO>();
            var inventoryData = inventoryDataSO.InventoryData;
            Assert.AreEqual(5, inventoryData.MaxInventorySize);
            Assert.AreEqual(0, inventoryData.BaseItems.Count);
            yield return null;
        }
        [UnityTest]
        public IEnumerator InventoryDataSO_Should_Have_Correct_Values() {
            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            var baseItems = new List<InspectorItem>();

            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();
            var item = new InspectorItem(itemType,3);
            baseItems.Add(item);

            inventoryDataSO.SetValues(15, baseItems);


            var inventoryData = inventoryDataSO.InventoryData;
            Assert.AreEqual(15, inventoryData.MaxInventorySize);
            Assert.AreEqual(1, inventoryData.BaseItems.Count);
            var baseItem = inventoryData.BaseItems[0];
            Assert.AreEqual(3, baseItem.NumberOfItems);
            Assert.AreEqual(itemType.ItemType, baseItem.ItemInfo);
            yield return null;
        }
    }
}
