using Godot;
using System;
using System.Collections.Generic;
using Array = Godot.Collections.Array;
using Dictionary = Godot.Collections.Dictionary;

public class SaveLoad : Control { 
    [Node]
    private Button _buttonSave = null;
    [Node]
    private Button _buttonSaveAs = null;
    [Node]
    private Button _buttonLoad = null;
    [Node("../SaveDialog")]
    private FileDialog _saveDialog = null;
    [Node("../LoadDialog")]
    private FileDialog _loadDialog = null;
    [Node("../ErrorDialog")]
    private AcceptDialog _errorDialog = null;
    [Node("../Legend")]
    private Legend _legend = null;

    [Node("../KeysKeyboard")]
    private Control _keysKeyboard = null;
    [Node("../KeysMouse")]
    private Control _keysMouse = null;

    private string _loadFilePath = "";

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        _saveDialog.Connect("file_selected", this, nameof(SaveTo));
        _loadDialog.Connect("file_selected", this, nameof(LoadFrom));

        _buttonSave.Connect("pressed", this, nameof(Save));
        _buttonSaveAs.Connect("pressed", this, nameof(SaveAs));
        _buttonLoad.Connect("pressed", this, nameof(Load));
    }

	public override void _Process(float delta) {

	}

    public void Save() {
        if (_loadFilePath == "") {
            SaveAs();
            return;
        }
        SaveFileTo(_loadFilePath);
    }

    public void SaveAs() {
        _saveDialog.PopupCentered();
    }

    public void Load() {
        _loadDialog.PopupCentered();
    }

    public void SaveTo(string path) {
        if (IsValidDir(_saveDialog.CurrentDir)) {
            _saveDialog.Hide();
            SaveFileTo(path);
            _loadFilePath = path;
        }
    }

    public void LoadFrom(string path) {
        if (IsValidPath(path)) {
            _loadDialog.Hide();
            LoadFileFrom(path);
            _loadFilePath = path;
        }
    }

    private void SaveFileTo(string path) {
        File file = new File();
        file.Open(path, File.ModeFlags.Write);

        string output = GetStringObj();

        file.StoreString(output);
        file.Close();
    }

    private void LoadFileFrom(string path) {
        File file = new File();
        file.Open(path, File.ModeFlags.Read);
        JSONParseResult data = JSON.Parse(file.GetAsText());
        if (data.Error != Error.Ok) {
            _errorDialog.DialogText = "Error parsing JSON:\n" + data.ErrorString;
            _errorDialog.PopupCentered();
        } else {
            if (data.Result.GetType() == typeof(Array)) {
                ClearKeys();
                LoadJSON((Array) data.Result);
            } else {
                _errorDialog.DialogText = "Error parsing JSON: Incorrect JSON format.";
                _errorDialog.PopupCentered();
            }
        }
        file.Close();
    }

    private bool IsValidPath(string path) {
        File file = new File();
        bool v = file.FileExists(path);
        return v;
    }
    

    private bool IsValidDir(string path) {
        Directory dir = new Directory();
        bool v = dir.DirExists(path);
        return v;
    }

    private void LoadJSON(Array arr) {
        if (Assert(arr.Count == 7, "Array must be size of 7.")) return;
        for (int i = 0; i < arr.Count; i ++) {
            if (Assert(arr[i].GetType() == typeof(Array), "Columns must be as array.")) return;
            Array a = (Array) arr[i];
            for (int j = 0; j < a.Count; j ++) {
                if (Assert(a[j].GetType() == typeof(Dictionary), "Keys must be as dictionary.")) return;
                Dictionary d = (Dictionary) a[j];
                if (Assert(d.Contains("key"), "Key must contain field key")) return;
                if (Assert(d["key"].GetType() == typeof(string), "Field key must be string")) return;
                if (Assert(d.Contains("text"), "Key must contain field text")) return;
                if (Assert(d["text"].GetType() == typeof(string), "Field text must be string")) return;
                if (Assert(d.Contains("color"), "Key must contain field color")) return;
                if (Assert(d["color"].GetType() == typeof(string), "Field color must be string")) return;
                if (Assert(d.Contains("column"), "Key must contain field column")) return;
                if (Assert(d["column"].GetType() == typeof(float), "Field column must be number")) return;
                
                string key = (string) d["key"];
                string text = (string) d["text"];
                Color color = new Color((string) d["color"]);
                int column = Mathf.FloorToInt((float) d["column"]);

                KeyMapping map = new KeyMapping(key, text, column, color);
                AddToKey(map);
            }
        }
    }

    private void AddToKey(KeyMapping map) {
        Array kkeys = _keysKeyboard.GetChildren();
        Array mkeys = _keysMouse.GetChildren();

        foreach (KeyButton key in kkeys) {
            if (map.key == key._key) {
                key.AddKey(map);
                key.UpdateMapping();
                return;
            }
        }

        foreach (KeyButton key in mkeys) {
            if (map.key == key._key) {
                key.AddKey(map);
                key.UpdateMapping();
                return;
            }
        }
    }

    private void ClearKeys() {
        Array kkeys = _keysKeyboard.GetChildren();
        Array mkeys = _keysMouse.GetChildren();

        foreach (KeyButton key in kkeys) {
            key.Clear();
        }

        foreach (KeyButton key in mkeys) {
            key.Clear();
        }
    }

    private string GetStringObj() {
        string o = "";

        o += "[";

        foreach (List<KeyMapping> column in _legend.columnMaps) {
            o += "[";
            foreach (KeyMapping map in column) {
                o += "{";
                
                o += $"\"key\": \"{map.key}\",";
                o += $"\"text\": \"{map.text}\",";
                o += $"\"color\": \"#{map.color.ToHtml(false)}\",";
                o += $"\"column\": {map.column}";

                o += "}";
                o += ",";
            }
            if (column.Count > 0) {
                o = o.Remove(o.Length - 1);
            }
            o += "]";
            o += ",";
        }
        if (_legend.columnMaps.Length > 0) {
            o = o.Remove(o.Length - 1);
        }

        o += "]";

        return o;
    }

    private bool Assert(bool cond, string err) {
        if (!cond) {
            _errorDialog.DialogText = err;
            _errorDialog.PopupCentered();
            return true;
        }
        return false;
    }
}