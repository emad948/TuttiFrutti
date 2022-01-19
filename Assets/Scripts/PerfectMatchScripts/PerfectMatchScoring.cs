using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
public class PerfectMatchScoring : MonoBehaviour
{

    public class AddProperties{ // hier kannst du fuer PerfectMatch Spieler-relevante Eigenschaften definieren.
        public AddProperties(){}
        public bool hasFallenOut = false;
    }

    private List<(NetworkPlayer, AddProperties)> players;
    private GameNetworkManager _gameNetMan;
    public GlobalTime globalTime;
    //List<NetworkPlayer> players = ((GameNetworkManager)Mirror.NetworkManager.singleton).PlayersList;
    
    void Start(){
        
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
            }
        }
    }


   public void playerFellOut(GameObject player){
       print("playerFellOut");
        // hier score von player zeitabhaengig erhoehen.
       if (globalTime.matchTime > 125){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
            scoreChangeHelper(player, 0);        //0 Points if you fall in the first Round
                       
        }
        else if (globalTime.matchTime > 105 && globalTime.matchTime <125 ){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
            scoreChangeHelper(player, 2);         //2 Points if you fall in the secound Round
                       
                      
        }
        else if (globalTime.matchTime > 85 && globalTime.matchTime <105){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
           scoreChangeHelper(player, 4);          //4 points if you fall in the third round
                        
        }
        else{
            scoreChangeHelper(player, 6);         //6 points if you clear the level
        }
    }
    public void update(){

    }


 public void gameEnds(){
        foreach ((NetworkPlayer player, AddProperties prop) in players)
        {
            if (!prop.hasFallenOut){
                // spieler ist nicht rausgefallen. Beispiel, maximalen Score zuweisen:
                player.ChangeScore(8);
            }
        }

        if(globalTime.matchTime <=0){
            _gameNetMan.AfterLevelEnd();
        }
    }

    

//Player dont fell off an cleared the level
    //public void clear(GameObject player){
      // if(!notfalling && globalTime.matchTime ==0){
     //       player.ChangeScore(6); 

       //     _gameNetMan.AfterLevelEnd();
      //  }
    //}
}
