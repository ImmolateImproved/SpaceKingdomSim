using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UISingleton : MonoBehaviour
{
    public static UISingleton singleton;

    public UIDocument uIDocument;

    private VisualElement parent;

    private List<Label> labels = new List<Label>();

    private void Awake()
    {
        singleton = this;

        var root = uIDocument.rootVisualElement;

        parent = root.Q<VisualElement>("StatisticsPanel");
    }

    public void ShowUnitDebugInfo(int index, string unitInfo)
    {
        if (labels.Count > index)
        {
            labels[index].text = unitInfo;
            return;
        }

        var label = new Label(unitInfo);

        label.AddToClassList("text");
        parent.Add(label);
        labels.Add(label);
    }
}