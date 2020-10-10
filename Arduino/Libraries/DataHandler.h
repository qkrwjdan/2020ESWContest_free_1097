
// ensure this library description is only included once
#ifndef DataHandler_h
#define DataHandler_h

// include types & constants of Wiring core API
#include "Arduino.h"
#include "SoftwareSerial.h"
#include "Servo.h"
#include "String.h"


// PIN setting
	//FlexSensor pin number
	#define flex0Pin A0
	#define flex1Pin A1
	#define flex2Pin A2
	#define flex3Pin A3
	#define flex4Pin A6

	//Servo motor pin number
	#define servo0Pin 3
	#define servo1Pin 4
	#define servo2Pin 5
	#define servo3Pin 6
	#define servo4Pin 7

	//vibe moter pin number
	#define vibePin 10
	#define vibeDuration 10

	//define data len
	#define receiveDataArrayLen 7

// library interface description
class DataHandler
{
public:
	DataHandler(uint8_t BluetoothRxPin, uint8_t BluetoothTxPin);
	Servo servoArr[5];

	float alpha;                                      
	uint16_t filteredValue[5];
	uint8_t angleValue[5] = {0,};
	uint16_t flexValueArr[5];
	uint16_t flexMax[5] = { 890, 800, 890, 750, 820 };
	uint16_t flexMin[5] ={ 730, 550, 730, 520, 760 };

	uint8_t vibeNum = 0;
	bool vibeState = false;
	
	//Arduino to Unity  setting
	char unityToArduinoDataArray[receiveDataArrayLen];
	boolean recvInProgress = false;
	byte ndx = 0;
	byte recvLen = 0;
	char startMarker = 's';
	char endMarker = 'e';
	char rc;
	boolean newData = false;

	SoftwareSerial mySerial;

	//Init
	void InitFlex();
	void FilterDeg(float alpha);
	void InitServo();
	void InitVibe();


	//Sensor Value
	uint8_t* GetFlexData();
	void FiltFlexData();
	
	//Send and Receive Data
	void SendData(uint8_t * flexData, float * ypr);
	void ReceiveData();

	//Rotate
	void RotateServo();

	//make vibe
	void MakeVibe();
	void TurnVibeOn();

	void ClearArr();
};

#endif

