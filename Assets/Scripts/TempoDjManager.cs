using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoDjManager : MonoBehaviour
{


    [Header("Public references")]
    public DjController djController;
    bool allowedToBeat;
    //beat track variables
    [Header("Beat timing variables")]
    [Range(0, 120)]
    public float beatsPerMinute = 80;
    [Range(0, 1)]
    public float errorMarginTime =.3f;


    //Commands variables
    int[] commandType;
    int commandCount = 0;
    int inactiveBeatCount = 0; //how many beats after command are inactive

    //measure how long an active beat time has no input
    float  beatFallTime;

    //count how long beat is active without an input
    private float beatActiveTime = 0f;
    new private bool enabled;
    public bool hasBeatInput{
        get{
            return enabled;
        }
        set{
            enabled = value;
            if(!enabled)
                beatActiveTime = Time.time;
        }
    }

    bool lastBeatHasInput = true;    //true means no, false means yes, used with offset along with hasBeatInput

    //fever variables
    bool fever;
    float feverTimeHold;


    // Start is called before the first frame update
    void Start()
    {
        allowedToBeat = true;
        hasBeatInput = false;

        inactiveBeatCount = 0;

        float invokeTime = 60f / beatsPerMinute;
        djController.secondsToBeats = (int)(invokeTime * 4);

        commandType = new int[4]{0, 0, 0, 0};

        beatFallTime = errorMarginTime;

        InvokeRepeating("PlayMasterBeat", errorMarginTime/2f, invokeTime);
        InvokeRepeating("AllowBeat", 0f, invokeTime);      
    }



    // Update is called once per frame
    void Update()
    {
        beatFallTime -= feverTimeHold.deltaTime;
        if(beatFallTime <0f){
            allowedToBeat = false;

            if(commandType[3] != 0){
                bool commandMatched = SetInput(commandType);
                if(commandMatched){
                    commandCount++;
                    inactiveBeatCount = 4; //4 beats after input are inactive
                }
                else{
                    inactiveBeatCount = 0;
                    commandCount = 0; 
                }
                Array.Clear(commandType, 0, commandType.Lenght);
            }
        }

        if (allowedToBeat && hasBeatInput && Input.anyKeyDown){     //double beat per master beat
                print("double beat not allowed");
                hasBeatInput = false;
                lastBeatHasInput = true;
                Array.Clear(commandType, 0, commandType.Lenght);
        }        

        GetDrumInputs();

        if(!allowedToBeat && Input.anyKeyDown){                     //mistiming beat with master beat
            beatMissSigh.Play();
            Array.Clear(commandType, 0, commandType.Length);
            commandCount = 0;
        }  
        
        if(inactiveBeatCount >0 && Input.anyKeyDown){               //interrupting command
            Array.Clear(commandType, 0, commandType.Length);
            commandCount = 0;
            //do physical motion stop here
        }
    
        
        if(Time.time - beatActiveTime >= errorMarginTime && lastBeatHasInput && allowedToBeat){      //skipping a master beat
            lastBeatHasInput = true;
            Array.Clear(commandType, 0, commandType.Length);
            
        }

        //continuos beats required to maintain fever
        if(commandCount >=4 ){
            fever = true;
            feverSprite.gameObject.SetActive(true);
        }    

        if(inactiveBeatCount>=0){
            feverTimeHold = Time.time;
        }
        if(Time.time - feverTimeHold >= ((errorMarginTime)*2) + 1f && fever ){    
            commandCount = 0;
            fever = false;
            feverSprite.gameObject.SetActive(false);
        }
    }


    void AllowBeat(){
        beatFallTime = errorMarginTime;

        if(inactiveBeatCount == 0)
                    
        allowedToBeat = true; 
        
        if(hasBeatInput){
            hasBeatInput = false;
        }
    }

    void PlayMasterBeat(){
        if((inactiveBeatCount--)>0){
            commandMutedBeat.Play();
        }      
        else
            masterBeat.Play();    
    }

    bool SetInput(int[] commandType){
        bool commandMatched = teamController.GetInput(commandType);
        return commandMatched;
    }

    void GetDrumInputs(){
        if(allowedToBeat){
            if(commandType[0] == 0){
                if(Input.GetButtonDown("Left")){
                    commandType[0] = 1;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Right")){ 
                    commandType[0] = 2;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Top")){ 
                    commandType[0] = 3; 
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Down")){ 
                    commandType[0] = 4;            
                    hasBeatInput = true;
                    
                    }
            }
            else if(commandType[1] == 0){
                if(Input.GetButtonDown("Left")){
                    commandType[1] = 1;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Right")){ 
                    commandType[1] = 2;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Top")){ 
                    commandType[1] = 3; 
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Down")){ 
                    commandType[1] = 4;  
                    hasBeatInput = true;
                    
                }
            }
            else if(commandType[2] == 0){
                if(Input.GetButtonDown("Left")){
                    commandType[2] = 1;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Right")){ 
                    commandType[2] = 2;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Top")){ 
                    commandType[2] = 3; 
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Down")){ 
                    commandType[2] = 4;  
                    hasBeatInput = true;
                    
                }
            }
            else if(commandType[3] == 0){
                if(Input.GetButtonDown("Left")){
                    commandType[3] = 1;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Right")){ 
                    commandType[3] = 2;
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Top")){ 
                    commandType[3] = 3; 
                    hasBeatInput = true;
                    
                }
                else if(Input.GetButtonDown("Down")){ 
                    commandType[3] = 4;
                    hasBeatInput = true;
                    
                }
            }
        }
        lastBeatHasInput = !hasBeatInput;
    }


}
