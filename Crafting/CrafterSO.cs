using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;
using EE.InventorySystem.Impl;
using System.Linq;

namespace EE.InventorySystem.Crafting.Impl {
    public class CrafterSO : ScriptableObject, ICrafterComponent {

        [SerializeField]
        private CraftingDataSO craftingDataSO;
        [SerializeField]
        private InventorySO inventoryDataSO;

        [SerializeField]
        private int index = 0;
        private ICrafter crafter;

        public event Action CraftingMenuOpenedEvent;
        public void CraftingMenuOpened() {
            CraftingMenuOpenedEvent?.Invoke();
        }
        private ICrafter Crafter {
            get {
                if (crafter == null) {
                    crafter = new Crafter(craftingDataSO.CraftingData);
                }
                return crafter;
            }  
        }
        [Button]
        public void Craft() {
            if (Crafter.Craft(inventoryDataSO.Content.ToList(), index, out Item item)) {
                inventoryDataSO.AddItem(item);
                Debug.Log("Item crafted");
            }
            else {
                Debug.Log("Crafting failed");
            }
        }
        [Button]
        public void Craft(int recipeIndex) {
            if (crafter.Craft(inventoryDataSO.Content.ToList(), recipeIndex, out Item item)) {
                inventoryDataSO.AddItem(item);
                Debug.Log("Item crafted");
            }
            else {
                Debug.Log("Crafting failed");
            }
        }

        [Button]
        public Item Craft(List<Item> items) {
            foreach (var recipe in craftingDataSO.CraftingData.Recipes) {
                if (recipe.Craft(items, out Item craftedItem)) {                    
                    foreach (var item in recipe.Requirements) {
                        inventoryDataSO.RemoveItem(true, item.ItemInfo,item.NumberOfItems);
                    }
                    inventoryDataSO.AddItem(craftedItem);
                    return craftedItem;
                }
            }
            return null;

            //if (crafter.Craft(inventoryDataSO.Items, recipeIndex, out Item item)) {
            //    inventoryDataSO.AddItem(item.ItemInfo, item.NumberOfItems);
            //    Debug.Log("Item crafted");
            //}
            //else {
            //    Debug.Log("Crafting failed");
            //}
        }

        [Button]
        public bool CanCraftÍtem(List<Item> items, out IRecipe craftableRecipe) {
            foreach (var recipe in craftingDataSO.CraftingData.Recipes) {
                if (recipe.CanCraftItem(items)) {
                    craftableRecipe = recipe;
                    return true;
                }
            }
            craftableRecipe = null;
            return false;
        }
    }
}


