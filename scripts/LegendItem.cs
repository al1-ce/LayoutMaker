using Godot;
using System;

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
        // FIXME move in list too
        GetParent().MoveChild(this, Math.Max(GetIndex() - 1, 0));
    }

    private void MoveDown() {
        GetParent().MoveChild(this, Math.Min(GetIndex() + 1, GetParent().GetChildCount()));
    }
}