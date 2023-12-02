using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] public TutorialChapter[] chapters;
    public GameObject lessonTemplate, listContainer;
    public Button nextButton, prevButton, firstButton, listButton;
    public Text explanationT, chapterNameT;
    public GameObject explanationP;

    public LineRenderer arrowLR;
    public BoatController boat;

    private int currentChapterIndex = -1, currentStepIndex = 0;
    // private GameObject currentTextBanner;
    private ListManager listManager;
    void Start()
    {
        // Debug.Log("Starting tutorial manager " + transform);
        chapterNameT = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        firstButton = transform.GetChild(1).GetChild(0).GetComponent<Button>();
        prevButton = transform.GetChild(1).GetChild(1).GetComponent<Button>();
        nextButton = transform.GetChild(1).GetChild(2).GetComponent<Button>();
        listButton = transform.GetChild(1).GetChild(3).GetComponent<Button>();
        nextButton.onClick.AddListener(NextChapter);
        prevButton.onClick.AddListener(PrevChapter);
        firstButton.onClick.AddListener(FirstChapter);
        listButton.onClick.AddListener(ListChapters);
        explanationT = lessonTemplate.transform.GetChild(0).GetChild(0).GetComponent<Text>();
        arrowLR = lessonTemplate.transform.GetChild(0).GetComponent<LineRenderer>();
        listManager = listContainer.GetComponent<ListManager>();
        listContainer.transform.GetComponent<ListManager>().setup(chapters);
        LoadChapter(currentChapterIndex);
    }

    public void activate(bool active) {
        transform.gameObject.SetActive(active);
        boat.setDemoMode(active);
    }

    void Update() { if (chapter != null) chapter.drawArrows(); }
    TargetObject[] targetObjects;
    TutorialChapter chapter;
    private void LoadChapter(int chapterIndex)
    {
        if (chapter != null) chapter.clear(this);
        if (chapterIndex < 0 || chapterIndex >= chapters.Length) {chapter = null; return;}
        chapter = chapters[chapterIndex];
        chapter.load(this);
        listManager.setChapter(chapter);
        // Instantiate and position the text banner
        // currentTextBanner = Instantiate(textBannerPrefab, textBannerParent);
        // currentTextBanner.transform.position = Camera.main.WorldToScreenPoint(chapter.targetObject.transform.position);
    }

    // computes world coordinates from a position of a canvas object.


    private void NextChapter()
    {
        if (currentChapterIndex < chapters.Length - 1)
        {
            currentChapterIndex++;
            LoadChapter(currentChapterIndex);
        }
        clearButtons();
    }

    private void PrevChapter()
    {
        if (currentChapterIndex > 0)
        {
            currentChapterIndex--;
            LoadChapter(currentChapterIndex);
        }
        clearButtons();
    }
    private void FirstChapter()
    {
        if (currentChapterIndex > 0)
        {
            currentChapterIndex = 0;
            LoadChapter(currentChapterIndex);
        }
        clearButtons();
    }

    private void ListChapters() {
        clearButtons();
        listContainer.SetActive(true);
    }
    void clearButtons() {
        foreach (Button button in new Button[]{firstButton, prevButton, nextButton /*, listButton*/}) button.transform.gameObject.SetActive(true);
        if (currentChapterIndex == 0) prevButton.transform.gameObject.SetActive(false);
        if (currentChapterIndex >= chapters.Length - 1) nextButton.transform.gameObject.SetActive(false);
    }
}
