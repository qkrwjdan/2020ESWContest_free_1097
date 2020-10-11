using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO.Ports;
using UnityEngine.UI;

public class btnType : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;
    public Transform buttonScale;
    Vector3 defaultScale;

    public CanvasGroup MainGroup;
    public CanvasGroup OptionGroup;

    private SerialPortHandler SPHandler;

    private void Start(){
        defaultScale = buttonScale.localScale;
    }

    public void OnBtnClick(){

        switch(currentType){
            case BTNType.Start:
            Debug.Log("Start");
            SceneManager.LoadScene("LoadingScene");
            break;

            case BTNType.Setting:
            Debug.Log("Setting");
            CanvasGroupOn(OptionGroup);
            CanvasGroupOff(MainGroup);
            break;

            case BTNType.Adopt:
            Debug.Log("Adopt");
            SendArduino();
            break;

            case BTNType.ExitSetting:
            Debug.Log("ExitSetting");
            CanvasGroupOff(OptionGroup);
            CanvasGroupOn(MainGroup);
            break;

            case BTNType.Exit:
            Application.Quit();
            Debug.Log("Exit!");
            break;            

        }

    }

    public void OnPointerEnter(PointerEventData eventData){
        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData){
        buttonScale.localScale = defaultScale;
    }

    public void CanvasGroupOn(CanvasGroup cg){
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void CanvasGroupOff(CanvasGroup cg){
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    public void SendArduino(){

        SPHandler = GameObject.Find("SP").GetComponent<SerialPortHandler>();

        var op = GameObject.Find("OptionGroup");
        Dropdown[] dropdowns = op.GetComponentsInChildren<Dropdown>();
        
        string sendData = "s";

        foreach(var dropdown in dropdowns){
            sendData = sendData.Insert(sendData.Length,dropdown.value.ToString());
        }

        sendData = sendData.Insert(sendData.Length,"e\n");
        SPHandler.servoControl = sendData;
        print(SPHandler.servoControl);
        SPHandler.SendString(sendData);
        
    }
}