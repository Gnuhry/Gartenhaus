//Hung Truong - Besondere Lernleistung - Gewächshaus-Automatisierung
//Arduino Mega 2560

//-------------Einbinden seperater Bibilotheken-------------------------
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

int counter;

//------------------------SetUp-----------------------------
void setup() {
  Serial.begin(9600);           //start Serial Communication
  Serial1.begin(10000);
  dht.begin();
  counter = 6000;
  Serial.println("-----------Start--------------");
}


//-----------------------Loop------------------------------
void loop() {
  if (counter-- == 0) {
    Serial1.println(FloatToString(GetTemp()) + "§" + FloatToString(GetHumid()) + "," + FloatToString(GetGroundHumid()) + "-" + FloatToString(GetUV()) + "\n");
    counter = 3000;
  }
  if (Serial1.available()) {
    delay(100);
    unsigned char data = Serial1.read();
    bool datab[8];
    FromByte(data, datab);
    //MinAction 0-4
    //MaxAction 0-4
    if (datab[0]) {
      digitalWrite(heater, HIGH);
    }//MinTempAction - Heizung
    else {
      digitalWrite(heater, LOW);
    }
    if (datab[1]) {
      digitalWrite(sprayer, HIGH);
    }//MinHumidAction - Luftbefeuchter
    else {
      digitalWrite(sprayer, LOW);
    }
    if (datab[2]) {
      digitalWrite(pump, HIGH);
    }//MinGroundHumidAction - Pumpe
    else {
      digitalWrite(pump, LOW);
    }
    if (datab[3]) {
      digitalWrite(uvLight, HIGH);
    }//MinUVAction - UV Licht
    else {
      digitalWrite(uvLight, LOW);
    }
    if (datab[4]) {
      digitalWrite(cooler, HIGH);
    }//MaxTempAction - Klimaanlage
    else {
      digitalWrite(cooler, LOW);
    }
    if (datab[5]) {
    }//MaxHumidAction - none
    if (datab[6]) {
    }//MaxGroundHumidAction - none
    if (datab[7]) {
      digitalWrite(shutters, HIGH);
    }//MaxUVAction - Rolladen
    else {
      digitalWrite(shutters, LOW);
    }
    //TODO SET ACTION ON/ OFF
  }
  delay(1);
}
void FromByte(unsigned char c, bool b[8])
{
  for (int i = 0; i < 8; ++i)
    b[i] = (c & (1 << i)) != 0;
}
//------------Check the Sensor--------
float GetTemp() {
  float temp[10];
  for (int f = 0; f < 10; f++)
  {
    //Read 10 Data in 20 ms distance
    temp[f] = dht.readTemperature();
    // Serial.println(temp[f]);
    // Serial.println(f);
    delay(20);
  }
  float temperatur = (temp[0] + temp[1] + temp[2] + temp[3] + temp[4] + temp[5] + temp[6] + temp[7] + temp[8] + temp[9]) / 10;
  // Serial.println(FloatToString(temperatur));
  return temperatur;
}

String FloatToString(float value) {
  char help[15];
  dtostrf(value, 7, 3, help);
  if (help == "") {
    return "-1";
  }
  return help;
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
  // Serial.println("Humid: " + FloatToString(humid));
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
  // Serial.println("UV: " + FloatToString(UV));
  return UV;
}
