#include "DataHandler.h"
#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"

DataHandler test(9,8);
uint8_t *flexData;

bool SendButton = false;
bool newData = false;

MPU6050 mpu;

#define INTERRUPT_PIN 2

bool dmpReady = false;
uint8_t mpuIntStatus;
uint8_t devStatus;
uint16_t packetSize;
uint16_t fifoCount;
uint8_t fifoBuffer[64];

Quaternion q;
VectorFloat gravity;
float ypr[3];

volatile bool mpuInterrupt = false;

void InitMPU(){
    
  Wire.begin();
  Wire.setClock(400000);

  Serial.begin(115200);

  mpu.initialize();
  pinMode(INTERRUPT_PIN, INPUT);

  Serial.println(F("Testing device connections..."));
  Serial.println(mpu.testConnection() ? F("MPU6050 connection successful") : F("MPU6050 connection failed"));

  Serial.println(F("Initializing DMP..."));
  devStatus = mpu.dmpInitialize();

  mpu.setXGyroOffset(131);
  mpu.setYGyroOffset(-56);
  mpu.setZGyroOffset(-39);
  mpu.setXAccelOffset(-789);
  mpu.setYAccelOffset(-379);
  mpu.setZAccelOffset(910);

  if (devStatus == 0) {
  Serial.println(F("Enabling DMP..."));
  mpu.setDMPEnabled(true);

  Serial.println(F("Enabling interrupt detection (Arduino external interrupt 0)..."));
  attachInterrupt(digitalPinToInterrupt(INTERRUPT_PIN), dmpDataReady, RISING);
  mpuIntStatus = mpu.getIntStatus();

  Serial.println(F("DMP ready! Waiting for first interrupt..."));
  dmpReady = true;

  packetSize = mpu.dmpGetFIFOPacketSize();
  } else {
  Serial.print(F("DMP Initialization failed (code "));
  Serial.print(devStatus);
  Serial.println(F(")"));
  }

}


void getYPR() {
 if (!dmpReady) return;

 while (!mpuInterrupt && fifoCount < packetSize) {

 }

 mpuInterrupt = false;
 mpuIntStatus = mpu.getIntStatus();

 fifoCount = mpu.getFIFOCount();

 if ((mpuIntStatus & 0x10) || fifoCount == 1024) {
 mpu.resetFIFO();
 Serial.println(F("FIFO overflow!"));

 } else if (mpuIntStatus & 0x02) {
 while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

 mpu.getFIFOBytes(fifoBuffer, packetSize);
 
 fifoCount -= packetSize;

 mpu.dmpGetQuaternion(&q, fifoBuffer);
 mpu.dmpGetGravity(&gravity, &q);
 mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);
 }
}

void dmpDataReady() {
 mpuInterrupt = true;
}

void setup() 
{
  test.InitFlex();
  test.FilterDeg(0.5);
  test.InitVibe();

  InitMPU();
  getYPR();
  Serial.println("Done");
} 

void loop() 
{
  test.ReceiveData();

  if(test.newData == true){

    if(test.recvLen == 1){

      //when message is start message
      if(test.unityToArduinoDataArray[0] == '1'){
        SendButton = true;
        test.ClearArr();
      }
      //when message is end message
      else if(test.unityToArduinoDataArray[0] == '3'){
        SendButton = false;
        test.ClearArr();
      }
      //when message is vibe message
      else if(test.unityToArduinoDataArray[0] == '2'){
        test.TurnVibeOn();
        test.ClearArr();
      }

    }

    test.newData = false;
  }

  if(SendButton){
    flexData = test.GetFlexData();
    getYPR();
    test.SendData(flexData,ypr);
  }

  test.MakeVibe();
  delay(5);
}
