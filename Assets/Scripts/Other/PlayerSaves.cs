using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSaves : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameTextField;
    public FlexibleColorPicker fcp1;
    public FlexibleColorPicker fcp2;
    public FlexibleColorPicker fcp3;
    [SerializeField] private GameObject michelin;
    [SerializeField] private TMP_Text playerDisplayName;

    private List<string> bottomComponents = new List<string>()
    {
        "footL", "footR", "kneecapL", "kneecapR", "lowerLegL", "lowerLegR", "shinL", "shinR", "thighJointL",
        "thighJointR", "thighL", "thighR", "footL", "footR", "ankleL", "ankleR", "hips"
    };

    private List<string> topComponents = new List<string>()
    {
        "elbowL.001", "elbowR", "handL", "handR", "lowerArmL", "lowerArmR", "schoulderL", "shoulderR", "upperArmL",
        "upperArmR", "wristL", "wristR", "head"
    };

    private List<string> middleComponents = new List<string>() {"lowerBreast", "stomach",};

    private void Start()
    {
        updateName(PlayerPrefs.GetString("playerName", "Anonymous"));

        var c_r = PlayerPrefs.GetFloat("color_T_r", 0.5f);
        var c_g = PlayerPrefs.GetFloat("color_T_g", 0.5f);
        var c_b = PlayerPrefs.GetFloat("color_T_b", 0.5f);
        var c_a = PlayerPrefs.GetFloat("color_T_a", 0.7f);
        var color = new Color(c_r, c_g, c_b, c_a);
        updateTopColor(color);

        c_r = PlayerPrefs.GetFloat("color_M_r", 0.5f);
        c_g = PlayerPrefs.GetFloat("color_M_g", 0.5f);
        c_b = PlayerPrefs.GetFloat("color_M_b", 0.5f);
        c_a = PlayerPrefs.GetFloat("color_M_a", 0.7f);
        color = new Color(c_r, c_g, c_b, c_a);
        updateMiddleColor(color);

        c_r = PlayerPrefs.GetFloat("color_B_r", 0.5f);
        c_g = PlayerPrefs.GetFloat("color_B_g", 0.5f);
        c_b = PlayerPrefs.GetFloat("color_B_b", 0.5f);
        c_a = PlayerPrefs.GetFloat("color_B_a", 0.7f);
        color = new Color(c_r, c_g, c_b, c_a);
        updateBottomColor(color);
    }

    public void updateBottomColor()
    {
        Color c = fcp3.color;
        updateBottomColor(c);
        PlayerPrefs.SetFloat("color_B_r", c.r);
        PlayerPrefs.SetFloat("color_B_g", c.g);
        PlayerPrefs.SetFloat("color_B_b", c.b);
        PlayerPrefs.SetFloat("color_B_a", c.a);
    }

    public void updateBottomColor(Color c)
    {
        foreach (var component in michelin.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var n = component.name;
            if (bottomComponents.Contains(n))
            {
                component.material.color = c;
            }
        }
    }

    public void updateMiddleColor()
    {
        Color c = fcp2.color;
        updateMiddleColor(c);
        PlayerPrefs.SetFloat("color_M_r", c.r);
        PlayerPrefs.SetFloat("color_M_g", c.g);
        PlayerPrefs.SetFloat("color_M_b", c.b);
        PlayerPrefs.SetFloat("color_M_a", c.a);
    }

    public void updateMiddleColor(Color c)
    {
        foreach (var component in michelin.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var n = component.name;
            if (middleComponents.Contains(n))
            {
                component.material.color = c;
            }
        }
    }

    public void updateTopColor()
    {
        Color c = fcp1.color;
        updateTopColor(c);
        PlayerPrefs.SetFloat("color_T_r", c.r);
        PlayerPrefs.SetFloat("color_T_g", c.g);
        PlayerPrefs.SetFloat("color_T_b", c.b);
        PlayerPrefs.SetFloat("color_T_a", c.a);
    }

    public void updateTopColor(Color c)
    {
        foreach (var component in michelin.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var n = component.name;
            if (topComponents.Contains(n))
            {
                component.material.color = c;
            }
        }
    }

    private void updateName(string newName)
    {
        playerDisplayName.text = newName;
    }

    public void savePlayerName()
    {
        var s = playerNameTextField.text;
        updateName(s);
        PlayerPrefs.SetString("playerName", s);
        PlayerPrefs.Save();
    }
}