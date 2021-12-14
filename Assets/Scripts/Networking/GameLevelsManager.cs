
using System;
using System.Threading;
    using Mirror;
    using UnityEngine;

    public class GameLevelsManager:NetworkManager
    {
    
        public static  void startLevel(string level)
        {
            switch (level)
            {
                case"Level_HillKing":
                    //put a timer for 90 seconds 
                    // Timer t = new Timer(TimerCallback, null, 0, 1000);
                    Console.ReadLine();

                    ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene("Level_HillKing");
                    break;
                default:
                    Debug.Log("Unknown scene name");
                    break;
            }
        }


        // private static void TimerCallback(Object o) {
        //     Console.WriteLine("In TimerCallback: " + DateTime.Now);
        // }
        //
    }



