using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TempoDjManager : MonoBehaviour
{
    //----------------------------------------------------------------------------------------------------------------------
    //Inicializamos las variables
    
    //Creo las variables donde irán las AudioSource del juego, es decir cada uno de los audios que ocupo.
    AudioSource[] audioSources;
    AudioSource commandMutedBeat;
    AudioSource pataSound;
    AudioSource chakaSound;
    AudioSource ponSound;
    AudioSource donSound;
    AudioSource mistakeSound;
    AudioSource masterBeatPatapon;
    AudioSource masterBeatChakapon;
    AudioSource masterBeatPonpon;
    
    //------------------------------------------------------------------------------------------------------------------------
    
    //Referencias públicas
    [Header("Public references")]
    public DjController djController;
    bool allowedToBeat;

    //------------------------------------------------------------------------------------------------------------------------

    //beat track variables
    [Header("Beat timing variables")]
    [Range(0, 120)]
    public float beatsPerMinute = 120;
    [Range(0, 1)]
    public float errorMarginTime = .3f;  //Margen de error que tiene el jugador para introducir un tecleo.
    string commandStyle; //Palabra que identifica que comando hizo el jugador.
    int[] commandType; //Array que contiene las digitaciones de comandos.
    int commandCount = 0; //contador de comandos completados
    int inactiveBeatCount = 0; //Cuantos beats después de ejecutar un comando están desactivados
    float  beatFallTime;  //Cantidad de tiempo en la que no se puede ingresar un Beat
    bool lastBeatHasInput = true;    //true means no, false means yes, used with offset along with hasBeatInput
    private float beatActiveTime = 0f;  //count how long beat is active without an input

    // ------------------------------------------------------------------------------------------------------------------------
    //fever variables
    bool fever;
    float feverTimeHold;

    //------------------------------------------------------------------------------------------------------------------------
    
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
    //------------------------------------------------------------------------------------------------------------------------
    //Metodo Start donde se declaran las variables
    void Start()
    {
        

        //--------------------------------------------------------------------------------------------------------------------
        //Declaro variables relacionadas al tempo.
        allowedToBeat = true;
        hasBeatInput = false;
        inactiveBeatCount = 0;
        float invokeTime = 60f / beatsPerMinute;
        djController.secondsToBeats = (int)(invokeTime * 4);
        commandType = new int[4]{0, 0, 0, 0};
        commandStyle = "caminar";
        beatFallTime = errorMarginTime;

        //--------------------------------------------------------------------------------------------------------------------
        //Variables de los audios declaradas
        audioSources = GetComponents<AudioSource>();
        commandMutedBeat = audioSources[0];
        pataSound = audioSources[1];
        chakaSound = audioSources[2];
        ponSound = audioSources[3];
        donSound = audioSources[4];
        mistakeSound = audioSources[5];
        masterBeatPatapon = audioSources[6];
        masterBeatChakapon = audioSources[7];
        masterBeatPonpon = audioSources[8];
        //--------------------------------------------------------------------------------------------------------------------
        //Se inician los métodos que se repiten

        InvokeRepeating("PlayMutedBeat", 2f, invokeTime);
        InvokeRepeating("PlayMasterBeat", 0f, 2f);
        InvokeRepeating("AllowBeat", 0f, invokeTime);  
        //--------------------------------------------------------------------------------------------------------------------

    }



    // Update is called once per frame
    void Update()
    {
        //Lo que está almacenado en bFT se lo resto a la variable con Time.deltaTime
        beatFallTime -= Time.deltaTime;
        if(beatFallTime < 0f){
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
            //Debug.Log("Conteo reseteado");
        }

        allowedToBeat = true;
        
        if(hasBeatInput){
            hasBeatInput = false;
        }

    }

    void PlayMutedBeat(){
        if((inactiveBeatCount--)<0){
            commandMutedBeat.Play();   
        }
    }

    void PlayMasterBeat(){

        string comparador = SetCommandStyle(commandStyle);      
        
        if((inactiveBeatCount--)>0 && comparador.Equals("caminar")){
            masterBeatPatapon.Play();
        }else if((inactiveBeatCount--)>0 && comparador.Equals("defender")){
            masterBeatChakapon.Play();     
        }else if((inactiveBeatCount--)>0 && comparador.Equals("atacar")){
            masterBeatPonpon.Play();     
        }
    }

    //Método para saber si el comando fue digitado en un tempo correcto.
    bool SetInput(int[] commandType){
        bool commandMatched = djController.GetInput(commandType);
        return commandMatched;
    }

    //Metodo para saber que comando fue el ingresado en DjController
    public string SetCommandStyle(string commandStyle){
        commandStyle = djController.GetCommandStyle(commandStyle);      
        return commandStyle;
    }

    void GetDrumInputs(){
        if(allowedToBeat == true){
            if(commandType[0] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[0] = 1;
                    hasBeatInput = true;
                    pataSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[0] = 2;
                    hasBeatInput = true;
                    chakaSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[0] = 3; 
                    hasBeatInput = true;
                    ponSound.Play();
                    
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
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[1] = 2;
                    hasBeatInput = true;
                    chakaSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[1] = 3; 
                    hasBeatInput = true;
                    ponSound.Play();
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
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[2] = 2;
                    hasBeatInput = true;
                    chakaSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[2] = 3; 
                    hasBeatInput = true;
                    ponSound.Play();
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
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[3] = 2;
                    hasBeatInput = true;
                    chakaSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[3] = 3; 
                    hasBeatInput = true;
                    ponSound.Play();
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



