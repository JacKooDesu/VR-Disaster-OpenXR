using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class VirtualKeyboard : MonoBehaviour
{
    [SerializeField]
    Text _inputField;
    [SerializeField]
    Transform _keyContainer;

    [SerializeField]
    int _maxLength = 10;
    [SerializeField]
    string _backspaceStr = "<";
    [SerializeField]
    string _submitStr = "↳";

    StringBuilder _builder;
    Button[] _buttons;

    [field: SerializeField]
    public UnityEvent<string> OnSubmit { get; private set; } = new();

    void Start()
    {
        _builder ??= new();
        _builder.Clear();

        _buttons = _keyContainer.GetComponentsInChildren<Button>();
        foreach (var button in _buttons)
            button.onClick.AddListener(new(BuildButtonCallback(this, button)));
    }

    static Action BuildButtonCallback(VirtualKeyboard instance, Button button)
    {
        var textObj = button.GetComponentInChildren<Text>();
        if (textObj is null)
            return () => { };

        string str = textObj.text;
        return str switch
        {
            _ when string.IsNullOrEmpty(str) => () => { }
            ,
            _ when str == instance._backspaceStr => () => instance.Backspace(),
            _ when str == instance._submitStr => () => instance.Submit(),
            _ => () => instance.Input(str),
        };
    }

    void Backspace()
    {
        if (_builder.Length > 0)
            _builder.Length--;
        UpdateInputField();
    }

    void Submit()
    {
        OnSubmit?.Invoke(_builder.ToString());
    }

    void Input(string str)
    {
        if (_builder.Length < _maxLength)
            _builder.Append(str);
        UpdateInputField();
    }

    void UpdateInputField()
    {
        _inputField.text = _builder.ToString();
    }
}