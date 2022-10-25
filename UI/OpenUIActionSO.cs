using EE.Core;
using EE.ItemSystem;
using EE.ItemSystem.Crafting.Impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.UI.Actions {
    public class OpenUIActionSO : GenericActionSO<OpenUIAction> {
        [SerializeField]
        private CrafterSO craftingController = null;
        public CrafterSO CraftingController => craftingController;

    }

    public class OpenUIAction : GenericAction {
        private OpenUIActionSO OriginSO => (OpenUIActionSO)base._originSO;
        IInventoryUser inventoryComponent;
        public override void Init(IHasComponents hasComponents) {
            inventoryComponent = hasComponents.GetComponent<IInventoryUser>();

        }
        public override void Enter() {
            inventoryComponent.InventoryOpened();
            //OriginSO.CraftingController.CraftingMenuOpened();
        }
    }
}



