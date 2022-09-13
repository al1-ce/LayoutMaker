using Godot;
using System;

public class PopupKeyEdit : Popup { 

    [Node]
    private Button _buttonApply = null;
    [Node]
    private Button _buttonCancel = null;
    [Node]
    private TextEdit _textEdit = null;
    [Node]
    private ColorPickerButton _colorPickerButton = null;
    
    private Button[] columns = new Button[7];

    private int selectedColumn = 0;

    public KeyMapping keyMapping;

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        _buttonCancel.Connect("pressed", this, nameof(Cancel));
        _buttonApply.Connect("pressed", this, nameof(Apply));

        for (int i = 0; i < columns.Length; i ++) {
            columns[i] = GetNode<Button>("ColumnSelect/Column" + (i + 1).ToString());
            // columns[i].Connect("pressed", this, nameof(SelectColumn));
        }
        columns[0].Connect("pressed", this, nameof(SelectColumn1));
        columns[1].Connect("pressed", this, nameof(SelectColumn2));
        columns[2].Connect("pressed", this, nameof(SelectColumn3));
        columns[3].Connect("pressed", this, nameof(SelectColumn4));
        columns[4].Connect("pressed", this, nameof(SelectColumn5));
        columns[5].Connect("pressed", this, nameof(SelectColumn6));
        columns[6].Connect("pressed", this, nameof(SelectColumn7));
    }

	public override void _Process(float delta) {
	}

    public void UpdateMapping() {
        _textEdit.Text = keyMapping.text;
        _colorPickerButton.Color = keyMapping.color;
        SelectColumn(keyMapping.column);
    }

    private void Cancel() {
        Hide();
    }

    private void Apply() {
        keyMapping.text = _textEdit.Text;
        keyMapping.color = _colorPickerButton.Color;
        keyMapping.column = selectedColumn;
        PopupKeyMap keyMap = (PopupKeyMap) GetTree().GetNodesInGroup("PopupMap")[0];
        keyMap.UpdateMapping();
        Hide();
    }

    private void SelectColumn1() {SelectColumn(0);}
    private void SelectColumn2() {SelectColumn(1);}
    private void SelectColumn3() {SelectColumn(2);}
    private void SelectColumn4() {SelectColumn(3);}
    private void SelectColumn5() {SelectColumn(4);}
    private void SelectColumn6() {SelectColumn(5);}
    private void SelectColumn7() {SelectColumn(6);}

    private void SelectColumn(int idx) {

        for (int i = 0; i < columns.Length; i ++) {
            if (i == idx) {
                columns[i].Pressed = true;
            } else {
                columns[i].Pressed = false;
            }
        }
        selectedColumn = idx;
    }
}