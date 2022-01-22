using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Mirror.Examples.Chat;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class PerfectMatchScoring : MonoBehaviour
{

    public class AddProperties{ // hier kannst du fuer PerfectMatch Spieler-relevante Eigenschaften definieren.
        public AddProperties(){}
        public bool hasFallenOut = false;
    }

    private List<(NetworkPlayer, AddProperties)> players;
    private GameNetworkManager _gameNetMan;
    public GlobalTime globalTime;
    private float _time;
    //List<NetworkPlayer> players = ((GameNetworkManager)Mirror.NetworkManager.singleton).PlayersList;


    public float sceneChangeTimer; // TODO change to correct value
    public bool testingMode = true;
    
    void Start(){
        _gameNetMan = ((GameNetworkManager) NetworkManager.singleton);
        globalTime = FindObjectOfType<GlobalTime>();
        players = new List<(NetworkPlayer, AddProperties)>();
        foreach(var player in ((GameNetworkManager)NetworkManager.singleton).PlayersList){
            players.Add( (player, new AddProperties()) ); // adding tuple 
        }
    }

    private void scoreChangeHelper(GameObject obj, int score){
        foreach ((NetworkPlayer player, AddProperties prop) in players){
            if (player.playerCharacter == obj){
                player.ChangeScore(score);
                prop.hasFallenOut = true;
                print("score of player changed!");
            }
        }
    }


   public void playerFellOut(GameObject player){
       
        // hier score von player zeitabhaengig erhoehen.
       if (globalTime.matchTime > 52){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
            scoreChangeHelper(player, 0);        //0 Points if you fall in the first Round
                       
        }
        else if (globalTime.matchTime > 33 && globalTime.matchTime <51 ){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
            scoreChangeHelper(player, 2);         //2 Points if you fall in the secound Round
                       
                      
        }
        else if (globalTime.matchTime > 18 && globalTime.matchTime <32){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
           scoreChangeHelper(player, 4);          //4 points if you fall in the third round
                        
        }
        else{
            scoreChangeHelper(player, 6);         //6 points if you clear the level
        }
        print("playerFellOut");
    }
    public void update(){

    }


 public void gameEndScoring(){
     
        foreach ((NetworkPlayer player, AddProperties prop) in players)
        {
            if (!prop.hasFallenOut){
                // spieler ist nicht rausgefallen. Beispiel, maximalen Score zuweisen:
                player.ChangeScore(8);
                print("gameEndChangeScore");
            }
        }
    
    }



}
