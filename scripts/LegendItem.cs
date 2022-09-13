using Godot;
using System;
using System.Collections.Generic;

public class LegendItem : Control { 

    [Node]
    private Label _label = null;
    [Node]
    private Label _labelKey = null;
    [Node]
    private ColorRect _colorRect = null;
    [Node]
    private TextureButton _buttonUp = null;
    [Node]
    private TextureButton _buttonDown = null;
    [Node("../../../..")]
    private Legend _legend = null;

    public KeyMapping keyMapping;

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        // UpdateLabel();
        _buttonUp.Connect("pressed", this, nameof(MoveUp));
        _buttonDown.Connect("pressed", this, nameof(MoveDown));
    }

    public void UpdateLabel() {
        _colorRect.Color = keyMapping.color;
        _label.Text = keyMapping.text;
        _labelKey.Text = keyMapping.key;
    }

	public override void _Process(float delta) {

	}

    private void MoveUp() {
        int newIdx = Math.Max(GetIndex() - 1, 0);
        Move(_legend.columnMaps[keyMapping.column], GetIndex(), newIdx);
        GetParent().MoveChild(this, newIdx);
    }

    private void MoveDown() {
        int newIdx = Math.Max(GetIndex() - 1, 0);
        Move(_legend.columnMaps[keyMapping.column], GetIndex(), newIdx);
        GetParent().MoveChild(this, Math.Min(GetIndex() + 1, GetParent().GetChildCount()));
    }

    public void Move<T>(List<T> list, int oldIndex, int newIndex) {
        var item = list[oldIndex];

        list.RemoveAt(oldIndex);

        if (newIndex > oldIndex) newIndex--;
        // the actual index could have shifted due to the removal

        list.Insert(newIndex, item);
    }
}