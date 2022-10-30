using EE.Core;
using EE.ItemSystem.Impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.UI.Actions {
    public class RemoveItemFromCraftingTableActionSO : GenericActionSO<RemoveItemFromCraftingTableAction> {

        public ContentTypeSO ItemContentType = null;
    }
    public class RemoveItemFromCraftingTableAction : GenericAction {
        private RemoveItemFromCraftingTableActionSO OriginSO => (RemoveItemFromCraftingTableActionSO)_originSO;
        GridElement gridElement;

        public override void Init(IHasComponents controller) {
            gridElement = controller.GetComponent<GridElement>();
        }

        public override void Enter() {
            GameObject hasItems = gridElement.GetContent(OriginSO.ItemContentType);
            if (hasItems == null) {
                return;
            }
            gridElement.RemoveContent(OriginSO.ItemContentType);

            CraftingUI.Instance.ReturnItemToInventory(gridElement,hasItems);
        }
    }

}