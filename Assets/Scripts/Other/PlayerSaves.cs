using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSaves : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameTextField;
    public FlexibleColorPicker fcp;
    [SerializeField] private GameObject michelin;
    [SerializeField] private TMP_Text playerDisplayName;

    private void Start()
    {
        updateName(PlayerPrefs.GetString("playerName", "Anonymous"));
        var c_r = PlayerPrefs.GetFloat("color_r", 0.5f);
        var c_g = PlayerPrefs.GetFloat("color_g", 0.5f);
        var c_b = PlayerPrefs.GetFloat("color_b", 0.5f);
        var c_a = PlayerPrefs.GetFloat("color_a", 0.7f);
        var color = new Color(c_r, c_g, c_b, c_a);
        updateColor(color);
    }

    private void updateName(string newName)
    {
        playerDisplayName.text = newName;
    }

    private void updateColor(Color newColor)
    {
        if (michelin)
        {
            foreach (var component in michelin.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                Debug.Log(component.name);
                component.material.color = newColor;
            }
        }
    }

    public void savePlayerColor()
    {
        Color c = fcp.color;
        updateColor(c);
        PlayerPrefs.SetFloat("color_r", c.r);
        PlayerPrefs.SetFloat("color_g", c.g);
        PlayerPrefs.SetFloat("color_b", c.b);
        PlayerPrefs.SetFloat("color_a", c.a);
        PlayerPrefs.Save();
    }

    public void savePlayerName()
    {
        var s = playerNameTextField.text;
        updateName(s);
        PlayerPrefs.SetString("playerName", s);
        PlayerPrefs.Save();
    }
}