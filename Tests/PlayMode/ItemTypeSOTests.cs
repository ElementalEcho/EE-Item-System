using EE.ItemSystem.Impl;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace EE.ItemSystem.PlayMode {
    public class ItemTypeSOTests {
        internal class TestItemTypeSO : ItemTypeSO {
            public void SetValues(int maxInventorySize) {
                itemToDrop = new ItemInfo(maxInventorySize);
            }

            public static ItemTypeSO CreateItemTypeSO(int maxInventorySize) {
                var itemType1 = ScriptableObject.CreateInstance<TestItemTypeSO>();
                itemType1.SetValues(maxInventorySize);
                return itemType1;
            }
        }

        [UnityTest]
        public IEnumerator ItemTypeSO_Should_Init_Correctly() {
            var itemTypeSO = ScriptableObject.CreateInstance<ItemTypeSO>();
            Assert.AreEqual(5, itemTypeSO.MaxStack);
            Assert.IsNull(itemTypeSO.ItemToDrop);
            Assert.AreEqual(10, itemTypeSO.ManaToGive);
            Assert.AreEqual(0, itemTypeSO.StartItemUseEffects.Length);
            Assert.AreEqual(0, itemTypeSO.AttackItemUseEffects.Length);
            Assert.AreEqual(0, itemTypeSO.ThrownItemEffects.Length);
            yield return null;
        }
    }
}
