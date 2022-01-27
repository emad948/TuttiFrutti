using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSaves : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameTextField;
    public FlexibleColorPicker fcp;

    public void savePlayerColor()
    {
        Color c = fcp.color;
        PlayerPrefs.SetFloat("color_r", c.r);
        PlayerPrefs.SetFloat("color_g", c.g);
        PlayerPrefs.SetFloat("color_b", c.b);
        PlayerPrefs.SetFloat("color_a", c.a);
        PlayerPrefs.Save();
    }

    public void savePlayerName()
    {
        PlayerPrefs.SetString("playerName", playerNameTextField.text);
        PlayerPrefs.Save();
    }
}