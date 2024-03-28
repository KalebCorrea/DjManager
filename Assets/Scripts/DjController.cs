using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DjController : MonoBehaviour
{

    AudioSource unmatchedCommand;
    string commandStylus;
    int commandState;
    public TempoDjManager tempoDjManager;

    [HideInInspector] public int secondsToBeats;
    // Start is called before the first frame update
    void Start()
    {
        unmatchedCommand = GetComponent<AudioSource>();
        commandState = 0;
        commandStylus = "";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool GetInput(int[] commandType){

        //keep going with the party!
        if (ArrayCompare(commandType, new int[]{1, 1, 1, 3})){
            commandState = 0;
            Debug.Log("¡Caminar!");
            return true;
        }
        else if(ArrayCompare(commandType, new int[]{3, 3, 1, 3})){
            commandState = 1;
            Debug.Log("¡Atacad!");
            return true;
        }else if(ArrayCompare(commandType, new int[]{2, 2, 1, 3})){
            commandState =2;
            Debug.Log("¡Defender!");
            return true;
        }else{
            unmatchedCommand.Play();
            return false;
        }
    }


    public string GetCommandStyle(string commandStyle){
        
        if (commandState == 0){
            commandStylus = "caminar";
            //Debug.Log("retornando" + commandStylus);
            return commandStylus;
        }else if(commandState == 1){
            commandStylus = "atacar";
            //Debug.Log("retornando" + commandStylus);
            return commandStylus;
        }else if(commandState == 2){
             commandStylus = "defender";
             //Debug.Log("retornando" + commandStylus);
             return commandStylus;
             
        }else 
        return commandStylus;
    }

    bool ArrayCompare(int[] array1, int[] array2){
        if(array1.Length != array2.Length)
            return false;
        for(var i = 0; i < array1.Length; i++){
            if(array1[i] != array2[i])
            return false;
        }    
        return true;
    }
}

    

