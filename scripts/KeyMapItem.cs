using Godot;
using System;

public class KeyMapItem : Control { 
    [Node]
    private Button _buttonEdit = null;
    [Node]
    private Button _buttonRemove = null;
    [Node]
    private Label _label = null;
    [Node]
    private Label _labelColumn = null;
    [Node]
    private ColorRect _colorRect = null;

    public KeyButton keyParent;
    public KeyMapping keyMapping;

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        _buttonEdit.Connect("pressed", this, nameof(Edit));
        _buttonRemove.Connect("pressed", this, nameof(Remove));
    }

	public override void _Process(float delta) {

	}

    public void UpdateLabel() {
        _colorRect.Color = keyMapping.color;
        _label.Text = keyMapping.text;
        _labelColumn.Text = (keyMapping.column + 1).ToString();
    }

    private void Edit() {
        PopupKeyEdit keyEdit = (PopupKeyEdit) GetTree().GetNodesInGroup("PopupEdit")[0];
        keyEdit.PopupCentered();
        keyEdit.keyMapping = keyMapping;
        keyEdit.UpdateMapping();
    }

    private void Remove() {
        PopupKeyMap keyEdit = (PopupKeyMap) GetTree().GetNodesInGroup("PopupMap")[0];
        keyEdit.RemoveItem(keyMapping);
    }
}