﻿using EE.Core;
using EE.InventorySystem.Impl;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.InventorySystem.Decisions {
    public class InventoryHasSpecificItemsDecisionSO : GenericActionSO<InventoryHasSpecificItemsDecision> {

        [SerializeField]
        private Item[] requiredItems = new Item[0];
        public List<Item> RequiredItems => requiredItems.Select(item => new Item(item.ItemInfo, item.NumberOfItems)).ToList();

        [SerializeField]
        private bool checkCurrentItem = false;
        public bool CheckCurrentItem => checkCurrentItem;


    }
    public class InventoryHasSpecificItemsDecision : GenericAction {
        private InventoryHasSpecificItemsDecisionSO OriginSO => (InventoryHasSpecificItemsDecisionSO)_originSO;

        IInventoryComponent inventory;
        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryComponent>();
        }

        protected override bool Decide() {
            if (OriginSO.CheckCurrentItem && OriginSO.RequiredItems.Count > 0) {
                var firstItem = OriginSO.RequiredItems[0];
                return inventory.CurrentItem != null && inventory.CurrentItem.ItemInfo != firstItem.ItemInfo && inventory.CurrentItem.NumberOfItems >= firstItem.NumberOfItems;               
            }
            else {
                foreach (var inventoryItem in OriginSO.RequiredItems) {
                    if (!inventory.ContainsItem(inventoryItem)) {
                        return false;
                    }
                }
            }
            return true;

        }

    }
}