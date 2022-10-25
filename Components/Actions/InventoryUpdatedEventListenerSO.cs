﻿using EE.Core;

namespace EE.ItemSystem.EventListeners {
    public class InventoryUpdatedEventListenerSO : EventListenerSO<InventoryUpdatedEventListener> {
        private GenericActionSO[] genericActions;
        public GenericActionSO[] GenericActions => genericActions;

    }
    public class InventoryUpdatedEventListener : EventListener {
        private InventoryUpdatedEventListenerSO OriginSO => (InventoryUpdatedEventListenerSO)_originSO;

        IInventoryUser inventory;
        GenericAction[] actions;
        public void GetActions(IHasComponents controller) {
            actions = new GenericAction[OriginSO.GenericActions.Length];
            for (int i = 0; i < OriginSO.GenericActions.Length; i++) {
                actions[i] = OriginSO.GenericActions[i].GetAction(controller);
            }
        }

        public override void Init(IHasComponents controller) {
            inventory = controller.GetComponent<IInventoryUser>();
            GetActions(controller);
            inventory.AddInventoryAlteredEvent(DoActions);
        }

        private void DoActions() {
            foreach (var genericAction in actions) {
                genericAction.Enter();
            }
        }
    }
}