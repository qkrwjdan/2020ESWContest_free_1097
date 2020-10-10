
// include this library's description file
#include "DataHandler.h"
#include "Arduino.h"
#include "SoftwareSerial.h"
#include "String.h"
#include "Servo.h"


DataHandler::DataHandler(uint8_t rxPin, uint8_t txPin) : mySerial(rxPin, txPin){
	mySerial.begin(9600);
}

void DataHandler::InitFlex() {
	//pin mode setting
	pinMode(flex0Pin, INPUT);
	pinMode(flex1Pin, INPUT);
	pinMode(flex2Pin, INPUT);
	pinMode(flex3Pin, INPUT);
	pinMode(flex4Pin, INPUT);
	//LPF value Initialize
	filteredValue[0] = analogRead(flex0Pin);
    filteredValue[1] = analogRead(flex1Pin);
	filteredValue[2] = analogRead(flex2Pin);
	filteredValue[3] = analogRead(flex3Pin);
	filteredValue[4] = analogRead(flex4Pin);
}
bool DataHandler::IsReady(int* ptr) {
	if (ptr == nullptr)
		return false;
	else
		return true;
}

void DataHandler::FilterDeg(float alpha) {
	this->alpha = alpha;
}


void DataHandler::InitServo() {
	
	servoArr[0].attach(servo0Pin);
	servoArr[1].attach(servo1Pin);
	servoArr[2].attach(servo2Pin);
	servoArr[3].attach(servo3Pin);
	servoArr[4].attach(servo4Pin);

	servoArr[0].write(0);
	servoArr[1].write(0);
	servoArr[2].write(150);
	servoArr[3].write(150);
	servoArr[4].write(150);

}

void DataHandler::InitVibe(){
	pinMode(vibePin,OUTPUT);
	vibeState = false;
	vibeNum = 0;
	analogWrite(vibePin,0);
}

uint8_t* DataHandler::GetFlexData() {
	
	flexValueArr[0] = analogRead(flex0Pin);
	flexValueArr[1] = analogRead(flex1Pin);
	flexValueArr[2] = analogRead(flex2Pin); 
	flexValueArr[3] = analogRead(flex3Pin); 
	flexValueArr[4] = analogRead(flex4Pin); 

	FiltFlexData();

	return angleValue;
}

void DataHandler::FiltFlexData() {

	for (int i = 0; i < 5; i++) {
		filteredValue[i] = filteredValue[i] * (1 - alpha) + flexValueArr[i] * alpha;

		if (filteredValue[i] <= flexMin[i]) filteredValue[i] = flexMin[i];
		else if (filteredValue[i] >= flexMax[i]) filteredValue[i] = flexMax[i];
		angleValue[i] = map((int)filteredValue[i], flexMin[i], flexMax[i], 50, 180);
		angleValue[i] *= -1;
		angleValue[i] += 230;
	}

}

void DataHandler::SendData(uint8_t * flexData, float * ypr) {

	char pChrBuffer[6];
  	int i=0;

	mySerial.write(200);

	for(i=0;i<5;i++){
	mySerial.write(flexData[i]);
	}
  
	dtostrf(ypr[0] , 5, 2, pChrBuffer);
	mySerial.write(pChrBuffer);
	mySerial.write('\n');
	dtostrf(ypr[1] , 5, 2, pChrBuffer);
	mySerial.write(pChrBuffer);
	mySerial.write('\n');
	dtostrf(ypr[2] , 5, 2, pChrBuffer);
	mySerial.write(pChrBuffer);
	mySerial.write('\n');
	
	mySerial.write(201);
}


void DataHandler::ReceiveData() {

	int i=0;

	while(mySerial.available() > 0 && newData == false){
		rc = mySerial.read();

		if(recvInProgress == true){
			if(rc != endMarker){
				unityToArduinoDataArray[ndx] = rc;
				ndx++;
				if(ndx >= receiveDataArrayLen - 1){
					ndx = receiveDataArrayLen - 2;
				}
			}
			else{
				if(ndx <= receiveDataArrayLen - 2){
					unityToArduinoDataArray[ndx + 1] = '\0';
					recvInProgress = false;
					recvLen = ndx;
					ndx = 0;
					newData = true;
				}
				else{
					ClearArr();
					recvInProgress = false;
					ndx = 0;
					newData = false;
				}
			}
		}

		else if(rc == startMarker){
			recvInProgress = true;
		}
	}
}

void DataHandler::RotateServo() {

	int i=0;

	if (unityToArduinoDataArray[0] == '0') {
		servoArr[4].write(150);
	}
	else if (unityToArduinoDataArray[0] == '1') {
		servoArr[4].write(120);
	}
	else if (unityToArduinoDataArray[0] == '2') {
		servoArr[4].write(60);
	}
	else if (unityToArduinoDataArray[0] == '3') {
		servoArr[4].write(0);
	}

	if (unityToArduinoDataArray[1] == '0') {
		servoArr[3].write(150);
	}
	else if (unityToArduinoDataArray[1] == '1') {
		servoArr[3].write(120);
	}
	else if (unityToArduinoDataArray[1] == '2') {
		servoArr[3].write(60);
	}
	else if (unityToArduinoDataArray[1] == '3') {
		servoArr[3].write(0);
	}

	if (unityToArduinoDataArray[2] == '0') {
		servoArr[2].write(150);
	}
	else if (unityToArduinoDataArray[2] == '1') {
		servoArr[2].write(120);
	}
	else if (unityToArduinoDataArray[2] == '2') {
		servoArr[2].write(60);
	}
	else if (unityToArduinoDataArray[2] == '3') {
		servoArr[2].write(0);
	}

	if (unityToArduinoDataArray[3] == '0') {
		servoArr[1].write(0);
	}
	else if (unityToArduinoDataArray[3] == '1') {
		servoArr[1].write(60);
	}
	else if (unityToArduinoDataArray[3] == '2') {
		servoArr[1].write(120);
	}
	else if (unityToArduinoDataArray[3] == '3') {
		servoArr[1].write(150);
	}

	if (unityToArduinoDataArray[4] == '0') {
		servoArr[0].write(0);
	}
	else if (unityToArduinoDataArray[4] == '1') {
		servoArr[0].write(60);
	}
	else if (unityToArduinoDataArray[4] == '2') {
		servoArr[0].write(120);
	}
	else if (unityToArduinoDataArray[4] == '3') {
		servoArr[0].write(150);
	}

}

void DataHandler::MakeVibe(){
	if(vibeState){
		analogWrite(vibePin,255);
		vibeNum += 1;
	}
	else{
		analogWrite(vibePin,0);
	}

	if(vibeNum > vibeDuration){
		vibeNum = 0;
		vibeState = false;
	}
}

void DataHandler::TurnVibeOn(){
	if(!vibeState){
		vibeState = true;
	}
}

void DataHandler::ClearArr(){
	int i=0;
	for(i=0;i<7;i++){
		unityToArduinoDataArray[i] = '\0';
	}
}
