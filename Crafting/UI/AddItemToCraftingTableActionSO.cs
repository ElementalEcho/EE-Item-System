using EE.Core;
using EE.ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.UI.Actions {
    public class AddItemToCraftingTableActionSO : GenericActionSO<AddItemToCraftingTableAction> {

        public ContentTypeSO ItemContentType = null;

    }
    public class AddItemToCraftingTableAction : GenericAction {
        private AddItemToCraftingTableActionSO OriginSO => (AddItemToCraftingTableActionSO)_originSO;
        GridElement gridElement;

        public override void Init(IHasComponents controller) {
            gridElement = controller.GetComponent<GridElement>();
        }

        public override void Enter() {
            GameObject hasItems = gridElement.GetContent(OriginSO.ItemContentType);
            if (hasItems == null) {
                return;
            }
            foreach (var window in CraftingUI.Instance.additionalItemsWindows) {
                if (!window.HasContent(OriginSO.ItemContentType)) {
                    window.ReplaceOrAddContent(OriginSO.ItemContentType, hasItems);
                    gridElement.RemoveContent(OriginSO.ItemContentType);
                    break;
                }
            }
            CraftingUI.Instance.ItemChanged();
        }
    }

}