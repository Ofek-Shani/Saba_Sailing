using System;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class TutorialStep : MonoBehaviour
{
    public float duration;
    public TargetObject[] targetObjects;
    public string explanationText;
    public float factor = 1f;
    public float velocity = 3f; // what is the simulated velocity of the boat
    public enum StepAction {MoveRudder, MoveSails, MoveKeel, 
        ClickFrontSail, ClickMainSail, ClickBothSails, MoveRudderBack, 
        NoAction};
    public StepAction stepAction;

    BoatController boat;
    
    void Start() {
        boat = BoatController.Instance;
    }
   private void ExecuteStep()
    {
        doAction();
    }
    public void doAction() {
        switch (stepAction) {
            case StepAction.MoveRudder: boat.startSliderDemo(BoatController.DemoKind.STEERING, _factor:factor,  _velocity: velocity); break;
            case StepAction.MoveSails: boat.startSliderDemo(BoatController.DemoKind.SAILS, _factor:factor,  _velocity: velocity); break;
            case StepAction.MoveKeel: boat.startSliderDemo(BoatController.DemoKind.KEEL, _factor:factor,  _velocity: velocity); break;
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