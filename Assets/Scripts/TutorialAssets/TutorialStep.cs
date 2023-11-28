using System;
using UnityEngine;

[System.Serializable]
public class TutorialStep : MonoBehaviour
{
    public float duration;
    public TargetObject[] targetObjects;
    public string explanationText;
    public enum StepAction {MoveRudder, MoveSails, MoveKeel, 
        ClickFrontSail, ClickMainSail, ClickBothSails, 
        NoAction};
    public StepAction stepAction;

    BoatController boat = BoatController.Instance;


   private void ExecuteStep()
    {
        doAction();
    }


    public void doAction() {
        switch (stepAction) {
            case StepAction.MoveRudder: boat.startSliderDemo(BoatController.DemoKind.STEERING); break;
            case StepAction.MoveSails: boat.startSliderDemo(BoatController.DemoKind.SAILS); break;
            case StepAction.MoveKeel: boat.startSliderDemo(BoatController.DemoKind.KEEL); break;
            case StepAction.ClickFrontSail: boat.SetFrontSail(); break;
            case StepAction.ClickMainSail: boat.SetMainSail(); break;
            case StepAction.ClickBothSails: boat.SetBothSails(); break;
        }
    }
    public void stopAction() {
        switch (stepAction) {
            case StepAction.MoveRudder: case StepAction.MoveSails: case StepAction.MoveKeel: boat.stopSliderDemo(); break;
        }
    }

    internal void clear()
    {
        stopAction();
    }
}