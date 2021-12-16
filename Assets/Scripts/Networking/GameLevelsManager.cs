
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mirror;
    using UnityEngine;

    public class GameLevelsManager:NetworkManager
    {
        
        private string[] _gameLevels = {"Level_HillKing"};
       
        public override void Start()
        {
            //Shuffle Game Levels
            _gameLevels = RandomStringArrayTool.RandomizeStrings(_gameLevels);
        }   
        
        
        public  void startLevel()
        {
            string level = GETNextGameLevel();
            switch (level)
            {
                case"Level_HillKing":
                    ChangeScene("Level_HillKing");
                    break;
                default:
                    Debug.Log("Unknown scene name");
                    break;
            }
        }



        public void  EndLevel()
        {
            //TODO @Emad add end game scene if all levels are played
            if (_gameLevels.Length == 0) ChangeScene("WinnerScene");
          
            // ChangeScene("Level_HillKing");
            
        }


        #region HelperFunctions

        public   void ChangeScene(string scene)
        {
            ((GameNetworkManager) NetworkManager.singleton).ServerChangeScene(scene);
            
            Debug.Log("after game scene changed");
            
        }
        
        public string GETNextGameLevel()
        {
            //TODO @Emad change to End Game Scene
            if (_gameLevels.Length == 0) return "WinnerScene";
            var nextGameLevel = _gameLevels[0];
            _gameLevels = _gameLevels.Skip(1).ToArray();
            return nextGameLevel;
        }
        

        #endregion
      
    }




