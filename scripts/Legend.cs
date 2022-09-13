using Godot;
using System;
using System.Collections.Generic;

public class Legend : Control { 
    public VBoxContainer[] columns = new VBoxContainer[7];
    public List<KeyMapping>[] columnMaps = new List<KeyMapping>[7];

    private PackedScene legendItemScene = ResourceLoader.Load<PackedScene>("res://scenes/LegendItem.tscn");

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        for (int i = 0; i < columns.Length; i ++) {
            columns[i] = GetNode<VBoxContainer>("ScrollContainer/HBoxContainer/Column" + (i + 1).ToString());
            columnMaps[i] = new List<KeyMapping>();
        }
    }

	public override void _Process(float delta) {

	}

    public void AddMap(KeyMapping keyMapping, int column) {
        columnMaps[column].Add(keyMapping);
    }

    public void RemoveMap(KeyMapping keyMapping, int column) {
        columnMaps[column].Remove(keyMapping);
    }

    public void UpdateLegend() {
        for (int i = 0; i < columns.Length; i ++) {
            Godot.Collections.Array childs = columns[i].GetChildren();
            foreach (var child in childs) {
                columns[i].RemoveChild((Node) child);
            }
        }
        for (int i = 0; i < columns.Length; i ++) {
            foreach (KeyMapping key in columnMaps[i]) {
                LegendItem item = legendItemScene.Instance<LegendItem>();
                columns[i].AddChild(item);
                item.keyMapping = key;
                item.UpdateLabel();
            }
        }
    }
}