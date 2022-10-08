namespace EE.Core.Actions {
    public class FullGrownDecisionSO : GenericActionSO<FullGrownDecision> {

    }
    public class FullGrownDecision : GenericAction {
        private FullGrownDecisionSO OriginSO => (FullGrownDecisionSO)_originSO;

        GrowingComponent growingComponent;
        public override void Init(IHasComponents controller) {
            growingComponent = controller.GetComponent<GrowingComponent>();
        }

        protected override bool Decide() {
            return growingComponent.FullyGrownFlag;
        }
    }
}