using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UISingleton : MonoBehaviour
{
    public static UISingleton singleton;

    public UIDocument uIDocument;

    private VisualElement parent;

    private List<Label> labels = new List<Label>();
    private int currentLabel;
    public int MaxLabelCount { get; set; }

    private void Awake()
    {
        singleton = this;

        var root = uIDocument.rootVisualElement;

        parent = root.Q<VisualElement>("StatisticsPanel");
    }

    public void SetUnitDebugInfo(string unitInfo)
    {
        if (labels.Count >= MaxLabelCount)
        {
            labels[currentLabel].text = unitInfo;
            currentLabel = (currentLabel + 1) % MaxLabelCount;
            return;
        }
        
        var label = new Label(unitInfo);
        
        label.AddToClassList("text");
        labels.Add(label);
        parent.Add(label);
    }
}