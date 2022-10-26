using EE.ItemSystem.Impl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace EE.ItemSystem.PlayMode {
    public class InventoryComponentTests
    {
        internal class TestInventoryComponent : InventoryComponent {
            //public Transform MyTransform { get => myTransform; set => myTransform = value; }
            public void ExposeAwake() {
                Awake();
            }
        }

        private TestInventoryComponent CreateTestInventoryComponent() {
            GameObject gameObject = new GameObject();
            var physicsComponent = gameObject.AddComponent<TestInventoryComponent>();
            physicsComponent.ExposeAwake();
            return physicsComponent;
        }
        [UnityTest]
        public IEnumerator FacingDirection_Should_Be_RightDown_When_Target_Is_Right() {
            var physic2DComponent = CreateTestInventoryComponent();
            yield return null;
        }
    }
}
