using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using UnityEditor.Build.Content;

public class HelpManager : MonoBehaviour
{
        Button showKeyboardB, showLayoutB, tutorialB, stopB;
        GameObject keyboard, layout, tutorial;
        // [SerializeField] TutorialManager tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        GameObject buttonsP = transform.GetChild(0).gameObject;
        showKeyboardB = buttonsP.transform.GetChild(0).GetComponent<Button>();
        // Debug.Log("showKeyboardB : " + showKeyboardB);
        showLayoutB = buttonsP.transform.GetChild(1).GetComponent<Button>();
        tutorialB = buttonsP.transform.GetChild(2).GetComponent<Button>();
        stopB = buttonsP.transform.GetChild(3).GetComponent<Button>();
        showKeyboardB.onClick.AddListener(ShowKeyboard);
        showLayoutB.onClick.AddListener(ShowLayout);
        tutorialB.onClick.AddListener(ShowTutorial);
        stopB.onClick.AddListener(Stop);
        keyboard = transform.GetChild(1).gameObject;
        // Debug.Log("keyboard : " + keyboard);
        layout = transform.GetChild(2).gameObject;
        tutorial = transform.GetChild(3).gameObject;
        infoPColor = GetComponent<Image>().color;
        showKeyboardB.onClick.Invoke();
   }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject onDisplay;
    private Button onButton;
    private float onColorA;

    float setAlphaInButton(Button button, float a) {
        ColorBlock cb = button.colors;
        Color c = cb.normalColor;
        float ra = c.a;
        c.a = a; //c = new Color(c.r, c.g, c.b, a);
        cb.normalColor = c;
        button.colors = cb;
        return ra;
    }
    Color infoPColor;
    private void ShowItem(GameObject item, Button button, bool force = false) {
        Debug.Log("Change help: " + onDisplay + " --> " + item + " Tutorial? " + (item == tutorial));
        if (!force && item == onDisplay) return;

        if (onDisplay != null) {
            onDisplay.SetActive(false);
            setAlphaInButton(onButton, onColorA);
            if (onDisplay == tutorial) {
                tutorial.GetComponent<TutorialManager>().activate(false);
            }
        }
        onDisplay = item;
        onButton = button;
        if (onDisplay != null) {
            onDisplay.SetActive(true);
            onColorA = setAlphaInButton(button, 1f);
        }
        Color c = infoPColor;
        if (item == tutorial) {
            c = Color.clear;
            item.GetComponent<TutorialManager>().activate(true);
        }
        GetComponent<Image>().color = c;
        transform.gameObject.SetActive(true);
    }

    public void activate(bool active) {
        if (!active && onDisplay == tutorial) {
            tutorial.GetComponent<TutorialManager>().activate(false);
            // ShowItem(null, null);
        }
        gameObject.SetActive(active); 
        if (active && onDisplay == tutorial) ShowTutorial();      
    }
    private void ShowKeyboard() { ShowItem(keyboard, showKeyboardB); }
    private void ShowLayout() { ShowItem(layout, showLayoutB); }
    private void ShowTutorial() { ShowItem(tutorial, tutorialB, true); }
    private void Stop() { gameObject.SetActive(false);}
}
