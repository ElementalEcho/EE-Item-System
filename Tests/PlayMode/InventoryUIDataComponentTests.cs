using EE.Core;
using EE.Core.ScriptableObjects;
using EE.ItemSystem.Impl;
using EE.ItemSystem.UI;
using EE.Test.Util;
using EE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using static EE.ItemSystem.PlayMode.InventoryDataSOTests;
using static EE.ItemSystem.PlayMode.InventorySOTests;
using static EE.ItemSystem.PlayMode.ItemTypeSOTests;

namespace EE.ItemSystem.PlayMode {

    public class InventoryUIDataComponentTests {
        internal class TestInventoryUIDataComponent : InventoryUIDataComponent {

            public void SetValues(InventorySO inventorySO = null, ItemDataBaseSO itemDataBaseSO = null, ContentTypeSO contentTypeSO = null) {
                this.inventorySO = inventorySO;
                this.itemDataBaseSO = itemDataBaseSO;
                this.numberOfItemsContentType = contentTypeSO;
            }

            public InventorySO ExposeInventorySO => inventorySO;

            public ItemDataBaseSO ExposeItemDataBaseSO => itemDataBaseSO;

            public ContentTypeSO ExposeContentTypeSO => numberOfItemsContentType;

        }
        private TestInventoryUIDataComponent CreateTestInventoryUIDataComponent(InventorySO inventorySO = null, ItemDataBaseSO itemDataBaseSO = null, ContentTypeSO contentTypeSO = null) {
            GameObject gameObject = new GameObject();
            var inventoryUIDataComponent = gameObject.AddComponent<TestInventoryUIDataComponent>();
            inventoryUIDataComponent.SetValues(inventorySO, itemDataBaseSO, contentTypeSO);
            return inventoryUIDataComponent;
        }

        [UnityTest]
        public IEnumerator InventoryUIDataComponent_Should_Init_Correctly() {
            var inventoryComponent = CreateTestInventoryUIDataComponent();
            inventoryComponent.ExposeInventorySO.Should().BeNull();
            inventoryComponent.ExposeItemDataBaseSO.Should().BeNull();
            inventoryComponent.ExposeContentTypeSO.Should().BeNull();
            yield return null;
        }
        [UnityTest]
        public IEnumerator InventoryUIDataComponent_Should_Init_Correctly_WithData() {
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            var itemDataBaseSO = ScriptableObject.CreateInstance<ItemDataBaseSO>();
            var contentTypeSO = ScriptableObject.CreateInstance<ContentTypeSO>();

            var inventoryComponent = CreateTestInventoryUIDataComponent(inventorySO, itemDataBaseSO, contentTypeSO);
            inventoryComponent.ExposeInventorySO.Should().Be(inventorySO);
            inventoryComponent.ExposeItemDataBaseSO.Should().Be(itemDataBaseSO);
            inventoryComponent.ExposeContentTypeSO.Should().Be(contentTypeSO);
            yield return null;
        }

        [UnityTest]
        public IEnumerator GetObjects_Should_Return_ItemDisplayData() {
            var sprite1 = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
            var sprite2 = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);

            var itemType1 = ScriptableObject.CreateInstance<TestItemTypeSO>();
            itemType1.SetValues(5, sprite1);
            var itemType2 = ScriptableObject.CreateInstance<TestItemTypeSO>();
            itemType2.SetValues(5, sprite2);

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 3),
                new InspectorItem(itemType2, 3)
            };
            var itemDataBaseSO = ScriptableObject.CreateInstance<ItemDataBaseSO>();
            itemDataBaseSO.ItemList.Add(itemType1);
            itemDataBaseSO.ItemList.Add(itemType2);
            itemDataBaseSO.InitDictonary();

            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            inventoryDataSO.SetValues(5, baseItems);
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO); 
            var contentTypeSO = ScriptableObject.CreateInstance<ContentTypeSO>();

            var inventoryComponent = CreateTestInventoryUIDataComponent(inventorySO, itemDataBaseSO, contentTypeSO);
            inventoryComponent.ExposeInventorySO.Should().Be(inventorySO);
            inventoryComponent.ExposeItemDataBaseSO.Should().Be(itemDataBaseSO);
            inventoryComponent.ExposeContentTypeSO.Should().Be(contentTypeSO);

            var displayData = inventoryComponent.GetObjects();

            displayData.Count.Should().Be(5);

            var display = displayData[0];
            display.Color.Should().Be(Color.white);
            display.Icon.Should().Be(sprite1);
            display = displayData[1];
            display.Color.Should().Be(Color.white);
            display.Icon.Should().Be(sprite2); 
            display = displayData[2];
            display.Color.Should().Be(Color.white);
            display.Icon.Should().Be(null); 
            display = displayData[3];
            display.Color.Should().Be(Color.white);
            display.Icon.Should().Be(null);
            display = displayData[4];
            display.Color.Should().Be(Color.white);
            display.Icon.Should().Be(null);
            yield return null;
        }
    }
}