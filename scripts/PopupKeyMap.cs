using Godot;
using System;
using System.Collections.Generic;

public class PopupKeyMap : Popup { 
    public KeyButton keyButton;
    public List<KeyMapping> keyMappings = new List<KeyMapping>();

    private PackedScene keyMapItemScene = ResourceLoader.Load<PackedScene>("res://scenes/KeyMapItem.tscn");

    [Node("ScrollContainer/KeymapHolder")]
    private VBoxContainer _vbox = null;
    [Node("ScrollContainer/KeymapHolder/Control/ButtonAdd")]
    private Button _buttonAdd = null;
    [Node("ScrollContainer/KeymapHolder/Control")]
    private Control _addControl = null;

    [Node]
    private Button _buttonApply = null;
    [Node]
    private Button _buttonCancel = null;
    [Node]
    private Label _label = null;

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        _buttonCancel.Connect("pressed", this, nameof(Cancel));
        _buttonApply.Connect("pressed", this, nameof(Apply));
        _buttonAdd.Connect("pressed", this, nameof(AddNew));
    }

	public override void _Process(float delta) {

	}

    private void AddNew() {
        keyMappings.Add(new KeyMapping( keyButton._key, "None", 0, Color.ColorN("White") ));
        UpdateMapping();
    }

    public void UpdateMapping() {
        Godot.Collections.Array childs = _vbox.GetChildren();
        foreach (var child in childs) {
            if (child is KeyMapItem) _vbox.RemoveChild((Node) child);
        }
        for (int i = 0; i < keyMappings.Count; i ++) {
            KeyMapping map = keyMappings[i];
            KeyMapItem item = keyMapItemScene.Instance<KeyMapItem>();
            // _vbox.AddChildBelowNode(_addControl, item);
            _vbox.AddChild(item);
            item.keyMapping = map;
            item.keyParent = keyButton;
            item.UpdateLabel();
        }
        _label.Text = keyButton._key;
    }

    public void RemoveItem(KeyMapping item) {
        keyMappings.Remove(item);
        UpdateMapping();
    }

    private void Cancel() {
        Hide();
    }

    private void Apply() {
        keyButton.keyMappings = keyMappings;
        keyButton.UpdateMapping();
        Hide();
    }
}