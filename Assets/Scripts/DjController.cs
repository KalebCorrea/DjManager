using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DjController : MonoBehaviour
{

    AudioSource unmatchedCommand;

    [HideInInspector] public int secondsToBeats;
    // Start is called before the first frame update
    void Start()
    {
        unmatchedCommand = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public bool GetInput(int[] commandType){

        //keep going with the party!
        if (ArrayCompare(commandType, new int[]{1, 1, 1, 3})){
            
            Debug.Log("¡Que siga la fiesta!");
            return true;
        }
        else if(ArrayCompare(commandType, new int[]{3, 3, 1, 2})){
            
            Debug.Log("¡Atacad!");
            return true;
        }
        else{
            unmatchedCommand.Play();
            return false;
        }
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
