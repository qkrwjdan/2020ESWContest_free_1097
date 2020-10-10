#include "DataHandler.h"

DataHandler test(9,8);

void setup() 
{
  Serial.begin(115200);
  
  test.InitServo();
  Serial.println("Done");
  
} 

void loop() 
{
  test.ReceiveData();

  if(test.newData == true){

    if(test.recvLen == 1){

      //when message is start message
      if(test.unityToArduinoDataArray[0] == '1'){
        test.ClearArr();
      }

      //when message is end message
      else if(test.unityToArduinoDataArray[0] == '3'){
        test.ClearArr();
      }

      else if(test.unityToArduinoDataArray[0] == '2'){
        test.TurnVibeOn();
        test.ClearArr();
      }

    }
    //when servo motor control data, vibe motor control data
    else if(test.recvLen == 5){
      Serial.println("rotate servo");
      test.RotateServo();
      test.ClearArr();
    }

    test.newData = false;
  }

  delay(5);
}