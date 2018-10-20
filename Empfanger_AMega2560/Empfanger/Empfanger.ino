//Hung Truong - Besondere Lernleistung - Gewächshaus-Automatisierung
//Arduino Mega 2560

//-------------Einbinden seperater Bibilotheken-------------------------
#include <Wire.h> //Bibilothek für Master-Slave-Kommunikation mit Master NodeMCU
#include <EEPROM.h> //Bibilothek zum Schreiben in den Speicher
#include <DHT.h> //Bibilothek zum Auswerten des Humid/Temp DH11 Sensor

//-------------------Deffinition der Pins-------------------------------
//TODO TEMP Sensoren vergleichen (eingebauter vs. seperater)
#define TempHumidSensor 5 //Pin des Temperatur-Humid-Sensor DH11
#define FeuchtSensor A0 //Pin des Feuchtigkeitssensor
#define UVSensor A1 //Pin des Ultra-Violet-Sensor GUVA-S12SD-3528

//Pins deffinieren für die Relays
#define Pumpe 3 //TODO - Ausprobieren
#define Heizung 4 //TODO -Zu wenig RELAYS!!!!
#define Klimaanlage 9
#define Luftbefeuchter 6
#define UVLicht 7
#define Rolladen 8


#define DHTTYPE DHT11    // Temp-Humid-Sensor ist Type DH11


//--------------------------initalisieren-------------------
DHT dht(TempHumidSensor, DHTTYPE); //initalisieren des Temp-Humid-Sensor
 int EEPROMAddress=0; //int-Wert für EEProm Addresse
 int higher[]{
   0,0,0,0 //heizung,befeuchter,pumpe,uvlicht
 },
 higherPin[]{
   Heizung,Luftbefeuchter,Pumpe,UVLicht
 },
 lower[]{
   0,-1,-1,0 //klimaanlage,-,-,rolladen
 },
 lowerPin[]{
   Klimaanlage,0,0,Rolladen
 };


//------------------------SetUp-----------------------------
void setup() {
 Wire.begin(8);                //I2C Bus an Adresse 8
 Wire.onReceive(receiveEvent); //initalisieren des receive Event
 Wire.onRequest(requestEvent); //initalisieren des request Event
 Serial.begin(9600);           //Serial Communication für Debug starten
 dht.begin(); //Temp-Humid-Sensor starten
Serial.println("-----------Start--------------");
}


//-----------------------Loop------------------------------
void loop() {
  Ueberprufung(0);
  Ueberprufung(1);
  Ueberprufung(2);
  Ueberprufung(3);
 delay(1000);
}

//------------Überprüfung der Sensoren und Reaktion--------
float GetTemp(){
  float temp[10]; //initalisieren des temporärem ErgebnisArray
  for(int f=0;f<10;f++)
  {
    //Einlesen von 10 Temperaturen im Abstand von 20ms
  temp[f]=dht.readTemperature();
  Serial.println(temp[f]);
  Serial.println(f);
  delay(20);
  }
  //Berechnung der Durschnittstemperatur
  float temperatur=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
  Serial.println(FloatToString(temperatur));
 return temperatur;
}

String FloatToString(float value){
   char help[15];
  dtostrf(value,7,3,help);
  return help;
  }
  void Ueberprufung(int index){
    int eepromMin;
    float value;
    switch(index){
      case 0:
        value=GetTemp();
        eepromMin=0;
        break;
      case 1:
        value=GetHumid();
        eepromMin=2;
        break;
      case 2:
        value=GetGroundHumid();
        eepromMin=4;
        break;
      case 3:
      eepromMin=6;
      value=GetUV();
        break;
    }
    float middlevalue=GetEEProm(eepromMin).toFloat()+GetEEProm(eepromMin+1).toFloat();
    middlevalue/=2;
    if(higher[index]==1&&(value>=middlevalue)){
      digitalWrite(higherPin[index],LOW);
      higher[index]=0;
    }
    if(lower[index]==1&&(value<=middlevalue)){
      digitalWrite(lowerPin[index],LOW);
      lower[index]=0;
    }
    if(higher[index]==0&&(value<GetEEProm(eepromMin).toFloat())){
      higher[index]=1;
      digitalWrite(higherPin[index],HIGH);
    }
    else if(lower[index]==0&&(value>GetEEProm(eepromMin+1).toFloat())){
      lower[index]=1;
      digitalWrite(lowerPin[index],HIGH);
    }
  }



float GetGroundHumid(){
    float temp[10];//initalisieren des temporärem ErgebnisArray

  for(int f=0;f<10;f++)
  {
    //Einlesen von 10 Feuchtigkeiten im Abstand von 20ms
  temp[f]=analogRead(FeuchtSensor);
  delay(20);
  }
  //Berecnung der Duschnittfeuchtigkeit
  float feuchtigkeit=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
  return feuchtigkeit;
}


float GetHumid(){
   float temp[10];//initalisieren des temporärem ErgebnisArray

  for(int f=0;f<10;f++)
  {
    //Einlesen von 10 Luftfeuchtigkeiten im Abstand von 20ms
  temp[f]=dht.readHumidity();
  delay(20);
  }
  //Berecnung der Duschnittsluftfeuchtigkeit
  float humid=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
    Serial.println("Humid: "+FloatToString(humid));
    return humid;
}


float GetUV(){
  float temp[10];//initalisieren des temporärem ErgebnisArray

  for(int f=0;f<10;f++)
  {
    //Einlesen von 10 UV-WErten im Abstand von 20ms
  temp[f]=analogRead(UVSensor);
  delay(20);
  }
  //Berecnung der Duschnittfeuchtigkeit
  float UV=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
  Serial.println("UV: "+FloatToString(UV));
  return UV;
}



//-----------------Master-Slave.Kommunikation mit NodeMCU -------------------
// Funktion, die aufgerufen wird, wenn Master etwas sendet
void receiveEvent(int howMany) {
  String help="";
 while (0 <Wire.available()) {
    help += Wire.read();  //bytes lesen, solange Wire available ist
  }
  if(help=="on"){
    //TODO on (live)
  }else
  {
  int seperator=help.indexOf("_"); //Index vom Unterstrich
 String temp=help.substring(0,seperator); //Index Teil
 help=help.substring(seperator+1,sizeof(help)); //Data Teil
 SaveEEProm(temp,help);
 }
}

// Funktion, die ausgeführt wird, wenn Master eine Nachricht erwartet
void requestEvent() {//----------------------------------------------------------------------------------------------------------------------------
  char buf[50]; //buffer initalisieren
  String help=FloatToString(GetTemp())+"_"+FloatToString(GetGroundHumid())+
  "_"+FloatToString(GetHumid())+"_"+FloatToString(GetUV());
  help.toCharArray(buf,sizeof(help));
  Wire.write(buf);  //buffer senden
}



//-------------------------speichern der Werte lokal ---------------------------------
void SaveEEProm(String type,String value){//Speichern des wertes an index
    /*case "Name":
    value+="_";
      for(int f=0;f<sizeof(value);f++)
      {
        EEPROM.write(80+f,value.charAt(f).toInt());
      }
      return;//name*/
    if(type== "MinTemp")
    {
      EEPROMAddress=0;
    }//mintemp
    else if(type== "MaxTemp")
    {
      EEPROMAddress=10;
    }//maxtemp
    else if(type== "MinGroundHumid")
    {
      EEPROMAddress=20;
    }//minfeucht
    else if(type== "MaxGroundHumid")
    {
      EEPROMAddress=30;   
    }//maxfeucht
    else if(type== "MinHumid")
    {    
      EEPROMAddress=40;
    }//minhumid
    else if(type== "MaxHumid")
    {
      EEPROMAddress=50;
    }//maxhumid
    else if(type== "MinUV")
    {
      EEPROMAddress=60;
    }//minUV
    else if(type== "MaxUV")
    {
      EEPROMAddress=70;
    }//maxUV
    int help[10];
    for(int f=0;f<10;f++){
      if(f<sizeof(value)){
        help[f]=value.charAt(f);
      }
      else{
        help[f]='_';
      }
    }
    for(int f=0;f<10;f++)
    {
      EEPROM.write(EEPROMAddress+f,help[f]);
    }
   // byte help[sizeof(wert)];
    //wert.getBytes(help,sizeof(wert));
  //EEPROM.commit();
  Serial.println("Save");
  }


 String GetEEProm(int index){//Daten an index im Speicher holen
   String erg=""; char last;
   switch(index){ //Speicherstelle anhand index herausfinden
     case 0:
       EEPROMAddress=0;
       break; //mintemp
     case 1:
       EEPROMAddress=10;
       break; //maxtemp
     case 2:
       EEPROMAddress=20;
       break; //minfeucht
     case 3:
       EEPROMAddress=30;
       break; //maxfeucht
     case 4:
       EEPROMAddress=40;
       break; //minhumid
     case 5:
       EEPROMAddress=50;
       break; //maxhumid
     case 6:
       EEPROMAddress=60;
       break; //minUV
     case 7:
       EEPROMAddress=70;
       break; //maxUV
     }

     do
     {
       last=EEPROM.read(EEPROMAddress);
       erg+=last;
       EEPROMAddress++;
     }
     while(last!='_');
      return erg;
    }
