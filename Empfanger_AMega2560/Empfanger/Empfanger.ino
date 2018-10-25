//Hung Truong - Besondere Lernleistung - Gewächshaus-Automatisierung
//Arduino Mega 2560

//-------------Einbinden seperater Bibilotheken-------------------------
#include <Wire.h>
#include <EEPROM.h>
#include <DHT.h>

//-------------------Deffinition of pins-------------------------------
//TODO TEMP Sensoren vergleichen (eingebauter vs. seperater)
#define TempHumidSensor 5
#define GroundHumidSensor A0
#define UVSensor A1


#define pump 3 //TODO - Ausprobieren
#define heater 4 //TODO -Zu wenig RELAYS!!!!
#define cooler 9
#define sprayer 6
#define uvLight 7
#define shutters 8


#define DHTTYPE DHT11


//--------------------------init-------------------------
DHT dht(TempHumidSensor, DHTTYPE);
int EEPROMAddress = 0;
int higher[] {
  0, 0, 0, 0 //heater,sprayer,pump,uv light
},
higherPin[] {
  heater, sprayer, pump, uvLight
},
lower[] {
  0, -1, -1, 0 //cooler,-,-,shutters
},
lowerPin[] {
  cooler, 0, 0, shutters
};


//------------------------SetUp-----------------------------
void setup() {
  Wire.begin(8);                //start Wire Communication
  Wire.onReceive(receiveEvent);
  Wire.onRequest(requestEvent);
  Serial.begin(9600);           //start Serial Communication
  dht.begin();
  Serial.println("-----------Start--------------");
}


//-----------------------Loop------------------------------
void loop() {
  Check(0);
  Check(1);
  Check(2);
  Check(3);
  delay(1000);
}

//------------Check the Sensor--------
float GetTemp() {
  float temp[10];
  for (int f = 0; f < 10; f++)
  {
    //Read 10 Data in 20 ms distance
    temp[f] = dht.readTemperature();
    Serial.println(temp[f]);
    Serial.println(f);
    delay(20);
  }
  float temperatur = (temp[0] + temp[1] + temp[2] + temp[3] + temp[4] + temp[5] + temp[6] + temp[7] + temp[8] + temp[9]) / 10;
  Serial.println(FloatToString(temperatur));
  return temperatur;
}

String FloatToString(float value) {
  char help[15];
  dtostrf(value, 7, 3, help);
  return help;
}
void Check(int index) {
  int eepromMin;
  float value;
  switch (index) {
    case 0:
      value = GetTemp();
      eepromMin = 0;
      break;
    case 1:
      value = GetHumid();
      eepromMin = 2;
      break;
    case 2:
      value = GetGroundHumid();
      eepromMin = 4;
      break;
    case 3:
      eepromMin = 6;
      value = GetUV();
      break;
  }
  float middlevalue = GetEEProm(eepromMin).toFloat() + GetEEProm(eepromMin + 1).toFloat();
  middlevalue /= 2;
  if (higher[index] == 1 && (value >= middlevalue)) {
    digitalWrite(higherPin[index], LOW);
    higher[index] = 0;
  }
  if (lower[index] == 1 && (value <= middlevalue)) {
    digitalWrite(lowerPin[index], LOW);
    lower[index] = 0;
  }
  if (higher[index] == 0 && (value < GetEEProm(eepromMin).toFloat())) {
    higher[index] = 1;
    digitalWrite(higherPin[index], HIGH);
  }
  else if (lower[index] == 0 && (value > GetEEProm(eepromMin + 1).toFloat())) {
    lower[index] = 1;
    digitalWrite(lowerPin[index], HIGH);
  }
}



float GetGroundHumid() {
  int temp[10];

  for (int f = 0; f < 10; f++)
  {
    //Read 10 Data in 20 ms distance
    temp[f] = analogRead(GroundHumidSensor);
    delay(20);
  }
  float groundHumid = (temp[0] + temp[1] + temp[2] + temp[3] + temp[4] +
  temp[5] + temp[6] + temp[7] + temp[8] + temp[9]) / 10.0;
  return groundHumid;
}


float GetHumid() {
  float temp[10];

  for (int f = 0; f < 10; f++)
  {
    //Read 10 Data in 20 ms distance
    temp[f] = dht.readHumidity();
    delay(20);
  }
  float humid = (temp[0] + temp[1] + temp[2] + temp[3] + temp[4] + temp[5] + temp[6] + temp[7] + temp[8] + temp[9]) / 10;
  Serial.println("Humid: " + FloatToString(humid));
  return humid;
}


float GetUV() {
  float temp[10];

  for (int f = 0; f < 10; f++)
  {
    //Read 10 Data in 20 ms distance
    temp[f] = analogRead(UVSensor);
    delay(20);
  }
  float UV = (temp[0] + temp[1] + temp[2] + temp[3] + temp[4] + temp[5] + temp[6] + temp[7] + temp[8] + temp[9]) / 10;
  Serial.println("UV: " + FloatToString(UV));
  return UV;
}



//-----------------Master-Slave Communication with NodeMCU -------------------
void receiveEvent(int howMany) {
  String help = "";
  while (0 < Wire.available()) {
    help += Wire.read();
  }
  if (help == "on") {
    //TODO on (live)
  } else
  {
    int seperator = help.indexOf("_");
    String temp = help.substring(0, seperator);
    help = help.substring(seperator + 1, sizeof(help));
    SaveEEProm(temp, help);
  }
}

void requestEvent() {//get data
  char buf[50];
  String help = FloatToString(GetTemp()) + "_" + FloatToString(GetGroundHumid()) + "_" + FloatToString(GetHumid()) + "_" + FloatToString(GetUV());
  help.toCharArray(buf, sizeof(help));
  Wire.write(buf);
}



//-------------------------save value local ---------------------------------
void SaveEEProm(String type, String value) {
  if (type == "MinTemp")
  {
    EEPROMAddress = 0;
  }//mintemp
  else if (type == "MaxTemp")
  {
    EEPROMAddress = 10;
  }//maxtemp
  else if (type == "MinGroundHumid")
  {
    EEPROMAddress = 20;
  }//mingroundhumid
  else if (type == "MaxGroundHumid")
  {
    EEPROMAddress = 30;
  }//maxgroundhumid
  else if (type == "MinHumid")
  {
    EEPROMAddress = 40;
  }//minhumid
  else if (type == "MaxHumid")
  {
    EEPROMAddress = 50;
  }//maxhumid
  else if (type == "MinUV")
  {
    EEPROMAddress = 60;
  }//minUV
  else if (type == "MaxUV")
  {
    EEPROMAddress = 70;
  }//maxUV
  int help[10];
  for (int f = 0; f < 10; f++) {
    if (f < sizeof(value)) {
      help[f] = value.charAt(f);
    }
    else {
      help[f] = '_';
    }
  }
  for (int f = 0; f < 10; f++)
  {
    EEPROM.write(EEPROMAddress + f, help[f]);
  }
  Serial.println("Save");
}


String GetEEProm(int index) {
  String erg = ""; char last;
  switch (index) {
    case 0:
      EEPROMAddress = 0;
      break; //mintemp
    case 1:
      EEPROMAddress = 10;
      break; //maxtemp
    case 2:
      EEPROMAddress = 20;
      break; //mingroundhumid
    case 3:
      EEPROMAddress = 30;
      break; //maxgroundhumid
    case 4:
      EEPROMAddress = 40;
      break; //minhumid
    case 5:
      EEPROMAddress = 50;
      break; //maxhumid
    case 6:
      EEPROMAddress = 60;
      break; //minUV
    case 7:
      EEPROMAddress = 70;
      break; //maxUV
  }

  do
  {
    last = EEPROM.read(EEPROMAddress);
    erg += last;
    EEPROMAddress++;
  }
  while (last != '_');
  return erg;
}
