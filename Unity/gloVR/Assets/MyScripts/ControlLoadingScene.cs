using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlLoadingScene : MonoBehaviour
{
  public Slider progressBar;
  public float max;

  private SerialPortHandler SPHandler;
  private UDPHandler UHandler;
  string start_string = "s1e";
  
  float timer;
  float waiting_time;

  bool sp;
  bool up;


  // Start is called before the first frame update
  void Start()
  {
    try{
      SPHandler = GameObject.Find("SP").GetComponent<SerialPortHandler>();
    }
    catch(Exception e){
      Debug.Log(e);
    }

    try{
      UHandler = GameObject.Find("UP").GetComponent<UDPHandler>();
    }
    catch(Exception e){
      Debug.Log(e);
    }

    UHandler.InitUDP();
    sp = false;
    up = false;
    
    max = progressBar.maxValue;

    timer = 0;
    waiting_time = 2;

  }

  // Update is called once per frame
  void Update()
  {
    timer += Time.deltaTime;
  
    if(timer > waiting_time)
    {
      //serial port로 데이터 보내기
      try{
        SPHandler.SendString(start_string);
      }
      catch(Exception e){
        Debug.Log(e);
      }

      //udp로 데이터 보내기
      try{
        UHandler.SendString("s");
      }
      catch(Exception e){
        Debug.Log(e);
      }

      //Serial으로 데이터 받기
      if(SPHandler.IsConnected() && !sp){
        progressBar.value += (float)0.5;
        sp = true;
      }

      //udp로 데이터 받기
      if(UHandler.newData && !up){
        progressBar.value += (float)0.5;
        up = true;
      }

      if (progressBar.value >= max)
      {
        SceneManager.LoadScene("GameScene");
      }
      
      timer = 0;
    }

  }
}
