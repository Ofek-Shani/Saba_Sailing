using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListManager : MonoBehaviour
{
    public GameObject togglePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    TutorialChapter[] chapters;
    public void setup(TutorialChapter[] chapters) {
        this.chapters = chapters;
        int n = chapters.Length;
        if (n == 0) return;
        float height1 = 60 + 40 * n,height2 = 40*n;
        Transform chaptersList = gameObject.transform.GetChild(1);
        setHeight(transform, 80 + 40*n);
        setHeight(chaptersList, 40*n);
        for (int i= 0; i < n; i++) {
            if (togglePrefab == null) makeOne(i);
            else addOne(i);
        }
    }

    void setHeight(Transform transform, float height) {
        RectTransform rt = transform.GetComponent<RectTransform>();
        Vector2 currentPosition = rt.anchoredPosition,
            currentSize = rt.sizeDelta;
        float y = currentSize.y;
        float dy = currentPosition.y;
        float deltay = dy + y/2;    
        currentSize.y = height;
        currentPosition.y = deltay - height/2;
        rt.sizeDelta = currentSize;
        rt.anchoredPosition = currentPosition;
    }
    void addOne(int i) {
        GameObject textObject = Instantiate(togglePrefab, transform);
        Text textComponent = textObject.GetComponent<Text>();

        if (textComponent == null)
        {
            // If the prefab does not have a Text component, create a new one
            textComponent = textObject.AddComponent<Text>();
        }

        // Optionally, you can modify the text properties here
        textComponent.text = "Prefab Text Element " + i;
    }
    void makeOne(int i) {
        GameObject textObject = new GameObject("Text Element " + i);
        textObject.transform.SetParent(transform, false);

        // Add a Text component to the GameObject
        Text textComponent = textObject.AddComponent<Text>();

        // Optionally, you can modify other properties of the Text component
        textComponent.text = "Chapter # " + i;
            textComponent.color = Color.yellow;
            textComponent.fontSize = 24;
            textComponent.alignment = TextAnchor.MiddleLeft;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    int currentChapter= 0;
    internal void setChapter(TutorialChapter chapter)
    {
        currentChapter = 0;
        if (chapters == null) return;
        foreach (TutorialChapter c in chapters){
            if (chapter == c) break;
            currentChapter++;
        }
    }
}
 