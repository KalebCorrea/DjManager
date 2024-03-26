using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TempoDjManager : MonoBehaviour
{
    //Array que contiene los componentes audioSources donde van los .mp3 del juego
    AudioSource[] audioSources;
    AudioSource masterBeat;
    AudioSource pataSound;
    AudioSource chakaSound;
    AudioSource ponSound;
    AudioSource donSound;
    AudioSource mistakeSound;
    AudioSource commandMutedBeat;
    
    
    
    //Referencias públicas
    [Header("Public references")]
    public DjController djController;
    bool allowedToBeat;

    //beat track variables
    [Header("Beat timing variables")]
    [Range(0, 120)]
    public float beatsPerMinute = 120;
    [Range(0, 1)]
    public float errorMarginTime = .3f;


    //Commands variables

    //Array que contiene las digitaciones de comandos
    int[] commandType;
    //contador de comandos
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
            if(!enabled){
                beatActiveTime = Time.time;
            }
                
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

        audioSources = GetComponents<AudioSource>();
        masterBeat = audioSources[0];
        pataSound = audioSources[1];
        chakaSound = audioSources[2];
        ponSound = audioSources[3];
        donSound = audioSources[4];
        mistakeSound = audioSources[5];
        commandMutedBeat = audioSources[6];

        beatFallTime = errorMarginTime;
        

        //Llamo los métodos PlayMasterBeat y AllowBeat

        InvokeRepeating("PlayMasterBeat", errorMarginTime/2f, invokeTime);
        InvokeRepeating("AllowBeat", 0f, invokeTime);      
    }



    // Update is called once per frame
    void Update()
    {


        //Lo que está almacenado en bFT se lo resto a la variable con Time.deltaTime
        beatFallTime -= Time.deltaTime;
        if(beatFallTime < 0f){
            allowedToBeat = false;
            Debug.Log("Beat no permitido");

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
                Array.Clear(commandType, 0, commandType.Length);
            }
        }

        //No puedes presionar dos veces la tecla aunque estés en el intervalo correcto del beat
        
        if (allowedToBeat && hasBeatInput && Input.anyKeyDown){     //double beat per master beat
                print("double beat not allowed");
                hasBeatInput = false;
                lastBeatHasInput = true;
                Array.Clear(commandType, 0, commandType.Length);
        }        

        GetDrumInputs();

        if(!allowedToBeat && Input.anyKeyDown){                     //mistiming beat with master beat
            mistakeSound.Play();
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
            
        }    

        if(inactiveBeatCount>=0){
            feverTimeHold = Time.time;
        }
        if(Time.time - feverTimeHold >= ((errorMarginTime)*2) + 1f && fever ){    
            commandCount = 0;
            fever = false;
            
        }
    }


    void AllowBeat(){
        beatFallTime = errorMarginTime;

        if(inactiveBeatCount == 0){
            Debug.Log("Conteo reseteado");
        }

        allowedToBeat = true;
        
        if(hasBeatInput){
            hasBeatInput = false;
        }

        Debug.Log("AllowBeat llamado");
    }

    void PlayMasterBeat(){
        if((inactiveBeatCount--)>0){
            commandMutedBeat.Play();
        }      
        else
            masterBeat.Play();    
    }

    bool SetInput(int[] commandType){
        bool commandMatched = djController.GetInput(commandType);
        return commandMatched;
    }

    void GetDrumInputs(){
        if(allowedToBeat == true){
            if(commandType[0] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[0] = 1;
                    hasBeatInput = true;
                    pataSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[0] = 2;
                    hasBeatInput = true;
                    ponSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[0] = 3; 
                    hasBeatInput = true;
                    chakaSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[0] = 4;            
                    hasBeatInput = true;
                    donSound.Play();
                    
                }
            }
            else if(commandType[1] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[1] = 1;
                    hasBeatInput = true;
                    pataSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[1] = 2;
                    hasBeatInput = true;
                    ponSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[1] = 3; 
                    hasBeatInput = true;
                    chakaSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[1] = 4;  
                    hasBeatInput = true;
                    donSound.Play();
                }
            }
            else if(commandType[2] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[2] = 1;
                    hasBeatInput = true;
                    pataSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[2] = 2;
                    hasBeatInput = true;
                    ponSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[2] = 3; 
                    hasBeatInput = true;
                    chakaSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[2] = 4;  
                    hasBeatInput = true;
                    donSound.Play();
                }
            }
            else if(commandType[3] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[3] = 1;
                    hasBeatInput = true;
                    pataSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[3] = 2;
                    hasBeatInput = true;
                    ponSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[3] = 3; 
                    hasBeatInput = true;
                    chakaSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[3] = 4;
                    hasBeatInput = true;
                    donSound.Play();
                }
            }
        }
        lastBeatHasInput = !hasBeatInput;
    }


}
