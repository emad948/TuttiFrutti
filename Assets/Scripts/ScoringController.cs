using System.Collections;
using System.Collections.Generic;
using Mirror.Examples.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoringController : MonoBehaviour
{
    public GameObject player;
    public TMP_Text scoreText;
    public int currentLevelScore = 0;
    void Start()
    {
        // add scoring script to Player, which executes the right method for the current mode
        var levelName = SceneManager.GetActiveScene().name;
        switch(levelName){
            case "Level_HillKing":
                InvokeRepeating("HillKing",0f, 0.25f);
                break;
            case "Level_??":
                break;
            default:
                Debug.Log("Unknown scene name");
                break;
        }
    }

    private void HillKing()
    {
        var pos = player.transform.position;
        if (pos.x > 18 && pos.x < 23)
        {
            if (pos.z > -23 && pos.z < -18)
            {
                if (pos.y > 12)
                {
                    currentLevelScore++;
                    scoreText.text = currentLevelScore.ToString();
                }
            }
        }
    }
}
