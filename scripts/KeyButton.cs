using Godot;
using System;
using System.Collections.Generic;

public class KeyButton : Button { 
    [Node("MulticolorPanel")]
    private MulticolorPanel _panel = null;

    private StyleBoxFlat _stylebox;

    [Export]
    public String _key = "A";

    public List<KeyMapping> keyMappings = new List<KeyMapping>();

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        _stylebox = (StyleBoxFlat) _panel.Get("custom_styles/panel");
        this.Connect("pressed", this, nameof(OnPressed));
        _panel.Visible = false;
    }

	public override void _Process(float delta) {

	}

    public void UpdateMapping() {
        _panel._list.Clear();

        Legend legend = GetNode<Legend>("../../Legend");

        foreach (KeyMapping i in keyMappings) {
            _panel._list.Add(i.color);
            
            for (int j = 0; j < legend.columns.Length; j ++) {
                List<KeyMapping> keyToRemove = new List<KeyMapping>();
                foreach (KeyMapping key in legend.columnMaps[j]) {
                    if (CheckNeedsRemove(key)) {
                        keyToRemove.Add(key);
                    }
                }
                foreach (KeyMapping key in keyToRemove) {
                    legend.RemoveMap(key, key.column);
                }
            }
            if (CheckNeedsAdd(i, legend.columnMaps[i.column])) {
                legend.AddMap(i, i.column);
            }
        }

        legend.UpdateLegend();

        _panel.UpdateGradient();
        if (_panel._list.Count > 0) {
            _panel.Show();
        } else {
            _panel.Hide();
        }
    }

    public void AddKey(KeyMapping key) {
        keyMappings.Add(key);
    }

    public void Clear() {
        keyMappings.Clear();
        UpdateMapping();
    }

    private void OnPressed() {
        PopupKeyMap _popupKeyMap = (PopupKeyMap) GetTree().GetNodesInGroup("PopupMap")[0];
        _popupKeyMap.PopupCentered();
        _popupKeyMap.keyButton = this;
        _popupKeyMap.keyMappings = Clone(keyMappings);
        _popupKeyMap.UpdateMapping();
    }

    public static List<KeyMapping> Clone(List<KeyMapping> listToClone)  {
        List<KeyMapping> list = new List<KeyMapping>();

        foreach (KeyMapping t in listToClone) {
            list.Add(t.Clone());
        }

        return list;
    }

    private bool CheckNeedsRemove(KeyMapping keyMap) {
        bool hasKey = false;
        if (keyMap.key != _key) return false;
        foreach (KeyMapping key in keyMappings) {
            if (key.IsEquals(keyMap)) {
                hasKey = true;
            }
        }
        return !hasKey;
    }

    private bool CheckNeedsAdd(KeyMapping keyMap, List<KeyMapping> list) {
        bool hasItem = false;
        foreach (KeyMapping key in list) {
            if (key.IsEquals(keyMap)) {
                hasItem = true;
            }
        }
        return !hasItem;
    }
}

public class KeyMapping {
    public string key;
    public string text;
    public int column;
    public Color color;

    public KeyMapping(string _key, string _text, int _column, Color _color) {
        key = _key;
        text = _text;
        column = _column;
        color = _color;
    }

    public KeyMapping Clone() {
        return new KeyMapping(key, text, column, new Color(color.r, color.g, color.b));
    }

    public string GetText() {
        return key + " " + text;
    }

    public bool IsEquals(KeyMapping map) {
        return  (key == map.key) && 
                (text == map.text) && 
                (column == map.column) && 
                (color.ToHtml() == map.color.ToHtml());
    }
}