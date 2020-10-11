using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class SerialPortHandler : MonoBehaviour
{
    SerialPort sp = new SerialPort("/dev/tty.PARK-DevB",9600);
    SerialPort sp2 = new SerialPort("/dev/tty.SERV-DevB",9600);

    public string servoControl;

    void Awake(){
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        //port open
        sp.ReadTimeout = 100;
        sp.Open();
        print("Serial ports open");

        servoControl = "s00000e";

        sp2.ReadTimeout = 100;
        sp2.Open();
        print("Serial port2 open");
    }

    public bool SendString(string string_data){

        if(sp.IsOpen){
            sp.Write(string_data);
        }
        else{
            // throw Exception;
            return false;
        }

        if(sp2.IsOpen){
            sp2.Write(string_data);
            return true;
        }
        else{
            return false;
        }
    }

    public bool IsConnected(){
        int b = 0;
        
        if(sp.IsOpen){
            try{                
                b = sp.ReadByte();
            }
            catch(System.Exception e){
                Debug.Log(e);
            }
            if(b != 0){
                return true;
            }
        }

        return false;
    }

    public bool ReceiveArduinoData(ref int[] flex_data,ref float[] zyro_data){
        int start_byte = 0;
        int end_byte = 0;

        int i = 0;
        string receiveData1 = "";
        string receiveData2 = "";
        string receiveData3 = "";
        float yaw;
        float pitch;
        float roll;


        if(sp.IsOpen){
            try{
                start_byte = sp.ReadByte();
            }
            catch(System.Exception e){
                Debug.Log(e);
            }

            if(start_byte == 200){
                for(i=0;i<5;i++){
                    flex_data[i] = sp.ReadByte();
                }

                receiveData1 = sp.ReadLine();
                receiveData2 = sp.ReadLine();
                receiveData3 = sp.ReadLine();
                
                end_byte = sp.ReadByte();
            }

            if(end_byte == 201){
                yaw = float.Parse(receiveData1);
                pitch = float.Parse(receiveData2);
                roll = float.Parse(receiveData3);

                yaw = (float)(yaw * 180 / 3.14);
                pitch = (float)(pitch * 180 / 3.14);
                roll = (float)((roll-0.5f) * 180 / 3.14);
                // roll = (float)(roll * -180f / 3.14);



                zyro_data[0] = yaw;
                zyro_data[1] = pitch;
                zyro_data[2] = roll;

                return true;
            }
        }
        return false;
    }

    public void setServo(int level){
        int i=0;
        string temp = servoControl;

        for(i=1;i<6;i++){
            char a = servoControl[i];
            a = (char)((int)a + level);

            if((int)a > 51){
                print((int)a);
                a = '3';
            }

            temp = temp.Insert(i,a.ToString());
            temp = temp.Remove(i+1,1);
        }
        
        print(temp);

        for(i=0;i<5;i++){
            SendString(temp);
        }
    }

    public void SendVibe(){
        int i=0;
        for(i=0;i<5;i++){
            SendString("s2e");
        }
    }

    public void DiscardBuffer(){
        sp.DiscardInBuffer();
    }

}