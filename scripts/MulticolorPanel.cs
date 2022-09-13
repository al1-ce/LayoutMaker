using Godot;
using System;
using System.Collections.Generic;

public class MulticolorPanel : Panel { 
    private Gradient _gradient = new Gradient();
    private GradientTexture _tex = new GradientTexture();
    [Export]
    public List<Color> _list = new List<Color>{Color.ColorN("White")};
    [Export]
    private bool _updateGradient {
        set {UpdateGradient();}
        get => false;
    }

    private ShaderMaterial _shaderMaterial;

    public override void _EnterTree() { this.WireNodes(); }

    public override void _Ready() {
        _tex.Gradient = _gradient;
        _gradient.InterpolationMode = Gradient.InterpolationModeEnum.Constant;
        _shaderMaterial = (ShaderMaterial) Material;
        UpdateGradient();
    }

	public override void _Process(float delta) {

	}

    public void UpdateGradient() {
        _gradient.Colors = _list.ToArray();
        _gradient.Offsets = MakeOffsets();
        _shaderMaterial.SetShaderParam("tex", _tex);
    }

    private float[] MakeOffsets() {
        List<float> list = new List<float>();
        float offset = 1.0f / _list.Count;
        float cur = 0.0f;
        for (int i = 0; i < _list.Count; i ++) {
            list.Add(cur);
            cur += offset;
        }
        return list.ToArray();
    }

    private void ClearGradient() {
        _gradient.Colors = new Color[]{Color.ColorN("White")};
        _gradient.Offsets = new float[]{0.0f};
    }
}