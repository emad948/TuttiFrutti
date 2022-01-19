using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PerfectMatchScoring : MonoBehaviour
{

    private List<NetworkPlayer> players;
    private GameNetworkManager _gameNetMan;
    public GlobalTime globalTime;
    //List<NetworkPlayer> players = ((GameNetworkManager)Mirror.NetworkManager.singleton).PlayersList;
    
    


   public void playerFellOut(GameObject player){
        // hier score von player zeitabhaengig erhoehen.
       if (globalTime.matchTime > 125){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
            player.ChangeScore(0);         //0 Points if you fall in the first Round
                       
        }
        else if (globalTime.matchTime > 105 && globalTime.matchTime <125 ){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
            player.ChangeScore(2);         //2 Points if you fall in the secound Round
                       
                      
        }
        else if (globalTime.matchTime > 85 && globalTime.matchTime <105){ // matchTime wird auf Wert x gesetzt und zu 0 heruntergezaehlt.
          
           player.ChangeScore(4);          //4 points if you fall in the third round
                        
        }
        else{
            player.ChangeScore(6);          //6 points if you clear the level
        }
    }

    public void update(){
        foreach (NetworkPlayer player in players)
        {

        }
    }


 public void gameEnds(){
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
