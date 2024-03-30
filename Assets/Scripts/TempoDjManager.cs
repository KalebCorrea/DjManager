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
    AudioSource firstSound;
    AudioSource secondSound;
    AudioSource thirdSound;
    AudioSource fourthSound;
    AudioSource mistakeSound;
    AudioSource masterBeatPatapon;
    AudioSource masterBeatChakapon;
    AudioSource masterBeatPonpon;
    AudioSource dusk;
    
    //------------------------------------------------------------------------------------------------------------------------
    
    //Referencias públicas
    [Header("Public references")]
    public DjController djController;
    bool allowedToBeat;

    //------------------------------------------------------------------------------------------------------------------------

    //beat track variables
    [Header("Beat timing variables")]
    [Range(0, 150)]
    public float beatsPerMinute = 122;
    [Range(0, 1)]
    public float errorMarginTime;  //Margen de error que tiene el jugador para introducir un tecleo.

    private bool shouldStartInvokeRepeating = false; // Indica si se deben iniciar los InvokeRepeating
    public float invokeTime;
    public float initialDelay = 0f; // Retraso inicial
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
        invokeTime = 60f / beatsPerMinute;
        djController.secondsToBeats = (int)(invokeTime * 4);
        commandType = new int[4]{0, 0, 0, 0};
        commandStyle = "caminar";
        errorMarginTime = invokeTime/2;
        beatFallTime = errorMarginTime;

        //--------------------------------------------------------------------------------------------------------------------
        //Variables de los audios declaradas
        audioSources = GetComponents<AudioSource>();
        commandMutedBeat = audioSources[0];
        firstSound = audioSources[1];
        secondSound = audioSources[2];
        thirdSound = audioSources[3];
        fourthSound = audioSources[4];
        mistakeSound = audioSources[5];
        masterBeatPatapon = audioSources[6];
        masterBeatChakapon = audioSources[7];
        masterBeatPonpon = audioSources[8];
        dusk = audioSources[9];
        //--------------------------------------------------------------------------------------------------------------------
        //Se inician los métodos que se repiten
        Invoke("StartAllInvokeRepeating", initialDelay);
        //--------------------------------------------------------------------------------------------------------------------
        //Se reproduce el tema principal.          
    }


    

    // Update is called once per frame
    void Update()
    {
        //BeatFallTime me genera dudaaaaaaaaaaaaas
        beatFallTime -= Time.deltaTime; //Temporizador que va hacia atrás disminuyendo el valor de beatFallTime.
        if(beatFallTime < 0f){
            allowedToBeat = false;
            Debug.Log("Bit no permitido");

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

        if (shouldStartInvokeRepeating)
        {
            // Iniciar todos los InvokeRepeating
            InvokeRepeating("PlayMutedBeat", invokeTime-0.15f, invokeTime);
            Invoke("PlaySong", invokeTime); 
            InvokeRepeating("PlayMasterBeat", invokeTime, 2f);
            InvokeRepeating("AllowBeat", invokeTime+0.15f, invokeTime);

            // Cambiar shouldStartInvokeRepeating a false para que esto no se vuelva a ejecutar
            shouldStartInvokeRepeating = false;
        }
    }

    void StartAllInvokeRepeating()
    {
       shouldStartInvokeRepeating = true;
    }

    void PlaySong(){
            dusk.Play();           
    }

    void AllowBeat(){
        beatFallTime = errorMarginTime;

        if(inactiveBeatCount == 0){
            //Debug.Log("Conteo reseteado");
        }

        allowedToBeat = true;
        Debug.Log("Bit permitido");
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
                    firstSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[0] = 2;
                    hasBeatInput = true;
                    secondSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[0] = 3; 
                    hasBeatInput = true;
                    thirdSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[0] = 4;            
                    hasBeatInput = true;
                    fourthSound.Play();
                }
            }
            else if(commandType[1] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[1] = 1;
                    hasBeatInput = true;
                    firstSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[1] = 2;
                    hasBeatInput = true;
                    secondSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[1] = 3; 
                    hasBeatInput = true;
                    thirdSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[1] = 4;  
                    hasBeatInput = true;
                    fourthSound.Play();
                }
            }
            else if(commandType[2] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[2] = 1;
                    hasBeatInput = true;
                    firstSound.Play();
                    
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[2] = 2;
                    hasBeatInput = true;
                    secondSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[2] = 3; 
                    hasBeatInput = true;
                    thirdSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[2] = 4;  
                    hasBeatInput = true;
                    fourthSound.Play();
                }
            }
            else if(commandType[3] == 0){
                if(Input.GetKeyDown(KeyCode.LeftArrow)){
                    commandType[3] = 1;
                    hasBeatInput = true;
                    firstSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.UpArrow)){ 
                    commandType[3] = 2;
                    hasBeatInput = true;
                    secondSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.RightArrow)){ 
                    commandType[3] = 3; 
                    hasBeatInput = true;
                    thirdSound.Play();
                }
                else if(Input.GetKeyDown(KeyCode.DownArrow)){ 
                    commandType[3] = 4;
                    hasBeatInput = true;
                    fourthSound.Play();
                }
            }
        }
        lastBeatHasInput = !hasBeatInput;
    }
}



