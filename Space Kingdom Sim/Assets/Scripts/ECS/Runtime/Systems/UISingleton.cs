using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UISingleton : MonoBehaviour
{
    public static UISingleton singleton;

    public UIDocument uIDocument;

    private VisualElement parent;

    private List<Label> labels = new List<Label>();

    private int currentIndex;

    private void Awake()
    {
        singleton = this;

        var root = uIDocument.rootVisualElement;

        parent = root.Q<VisualElement>("StatisticsPanel");
    }

    private void LateUpdate()
    {
        Reset();
    }

    private void Reset()
    {
        currentIndex = 0;
    }

    public void AddLabel(string unitInfo)
    {
        currentIndex++;

        if (labels.Count > currentIndex)
        {
            labels[currentIndex].text = unitInfo;
            return;
        }

        var label = new Label(unitInfo);

        label.AddToClassList("text");
        parent.Add(label);
        labels.Add(label);
    }
}