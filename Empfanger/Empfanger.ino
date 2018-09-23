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
bool pumpon=false,heizungon=false,
klimaanlageon=false,luftbefeuchteron=false,
uvlichton=false,rolladenon=false; //initalisieren der bool Werte für die Relays
 int EEPROMAddress=0; //int-Wert für EEProm Addresse


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
  TEMPUeberprufung();
  FEUCHTUeberprufung();
  HUMIDUeberprufung();
  UVUeberprufung();
 delay(1000);
}

//------------Überprüfung der Sensoren und Reaktion--------
void TEMPUeberprufung(){
  float temp[10]; //initalisieren des temporärem ErgebnisArray
  for(int f=0;f<sizeof(temp);f++)
  {
    //Einlesen von 10 Temperaturen im Abstand von 20ms
  temp[f]=dht.readTemperature();
  delay(20);
  }
  //Berechnung der Durschnittstemperatur
  float temperatur=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
 Serial.println("Temperatur: "+String(temperatur));
//Berechnung des Mittelwerts (min+max)/2
float mittelwert=(GetEEProm(2).toFloat()+GetEEProm(3).toFloat())/2;
//Wenn Heizung an und Mittelwert erreicht, Heizung aus
  if(heizungon&&temperatur>=mittelwert){
  digitalWrite(Heizung,LOW);
  heizungon=false;
  }
  //Wenn Klimaanlage an und Mittelwert erreicht, Klimaanlage aus
  if(klimaanlageon&&temperatur<=mittelwert){
  digitalWrite(Klimaanlage,LOW);
  klimaanlageon=false;
  }
  //Wenn IS-Temperatur unter Min-Soll-Temperatur, Heizung an
  if(temperatur<=GetEEProm(2).toFloat()){
    heizungon=true;
    digitalWrite(Heizung,HIGH);
    }
    //Wenn IS-Temperatur über Max-Soll-Temperatur, Klimaanlage an
    else if(temperatur>=GetEEProm(3).toFloat()){
    klimaanlageon=true;
    digitalWrite(Klimaanlage,HIGH);
    }
  }


void FEUCHTUeberprufung(){
   float temp[10];//initalisieren des temporärem ErgebnisArray

  for(int f=0;f<sizeof(temp);f++)
  {
    //Einlesen von 10 Feuchtigkeiten im Abstand von 20ms
  temp[f]=analogRead(FeuchtSensor);
  delay(20);
  }
  //Berecnung der Duschnittfeuchtigkeit
  float feuchtigkeit=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
  //Berechnung des Mittelwerts
  float mittelwert=(GetEEProm(4).toFloat()+GetEEProm(5).toFloat())/2;
  //Wenn Pumpe an und Mittelwert erreicht, Pumpe aus
  if(pumpon&&feuchtigkeit>=mittelwert){
  digitalWrite(Pumpe,LOW);
  pumpon=false;
  }
  //Wenn IS-Feuchtigkeit unter Min-Soll-Feuchtigkeit, Pumpe an
  if(feuchtigkeit<=GetEEProm(4).toFloat()){
    pumpon=true;
    digitalWrite(Pumpe,HIGH);
    }
  }


void HUMIDUeberprufung(){
   float temp[10];//initalisieren des temporärem ErgebnisArray

  for(int f=0;f<sizeof(temp);f++)
  {
    //Einlesen von 10 Luftfeuchtigkeiten im Abstand von 20ms
  temp[f]=dht.readHumidity();
  delay(20);
  }
  //Berecnung der Duschnittsluftfeuchtigkeit
  float humid=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
    Serial.println("Humid: "+String(humid));
   //Berechnung des Mittelwerts
    float mittelwert=(GetEEProm(6).toFloat()+GetEEProm(7).toFloat())/2;
    //Wenn Luftbefeuchter an und Mittelwert erreicht, Luftbefeuchter aus
    if(luftbefeuchteron&&humid>=mittelwert){
  digitalWrite(Luftbefeuchter,LOW);
  luftbefeuchteron=false;
  }
  //Wenn IS-Luftfeuchtigkeit unter Min-Soll-Luftfeuchtigkeit, Luftbefeuchter an
  if(humid<=GetEEProm(6).toFloat()){
    luftbefeuchteron=true;
    digitalWrite(Luftbefeuchter,HIGH);
    }
  }


void UVUeberprufung(){
   float temp[10];//initalisieren des temporärem ErgebnisArray

  for(int f=0;f<sizeof(temp);f++)
  {
    //Einlesen von 10 UV-WErten im Abstand von 20ms
  temp[f]=analogRead(UVSensor);
  delay(20);
  }
  //Berecnung der Duschnittfeuchtigkeit
  float UV=(temp[0]+temp[1]+temp[2]+temp[3]+temp[4]+temp[5]+temp[6]+temp[7]+temp[8]+temp[9])/10;
  Serial.println("UV: "+String(UV));
  //Berechnung des Mittelwerts
  float mittelwert=(GetEEProm(8).toFloat()+GetEEProm(9).toFloat())/2;
  //Wenn UV-Licht an und IS-UV-Wert über Min-Soll-Wert, UVLicht aus
  if(uvlichton&&UV>=GetEEProm(8).toFloat()){
  digitalWrite(UVLicht,LOW);
  uvlichton=false;
  }
  //Wenn Rolladen an und IS-UV-Wert unter Max-Soll-Wert, Rolladen hoch
  if(rolladenon&&UV<=GetEEProm(9).toFloat()){
  digitalWrite(Rolladen,LOW);
  rolladenon=false;
  }
  //Wenn IS-UV-Wert unter Min-Soll-UV-Wert, UV-Licht an
  if(UV<=GetEEProm(8).toFloat()){
    uvlichton=true;
    digitalWrite(UVLicht,HIGH);
    }
    //Wenn IS-UV-Wert über Max-Soll-UV-Wert, Rolladen runter
  else if(UV>=GetEEProm(9).toFloat()){
    rolladenon=true;
    digitalWrite(Rolladen,HIGH);
  }
}



//-----------------Master-Slave.Kommunikation mit NodeMCU -------------------
// Funktion, die aufgerufen wird, wenn Master etwas sendet
void receiveEvent(int howMany) {
  String help="";
 while (0 <Wire.available()) {
    help += Wire.read();  //bytes lesen, solange Wire available ist
  }
  int seperator=help.indexOf("_"); //Index vom Unterstrich
 String temp=help.substring(0,seperator); //Index Teil
 help=help.substring(seperator+1,sizeof(help)); //Data Teil
 SaveEEProm(temp.toInt(),help);//Data an Index speichern
}


// Funktion, die ausgeführt wird, wenn Master eine ID erwartet
void requestEvent() {
  String help=GetEEProm(0); //ID aus Speicher holen
  char buf[5]; //buffer initalisieren
  if(sizeof(help)>5)return; //Wenn ID länger als 5 Ziffer, ID ist falsch
  while(sizeof(help)!=5){
    help="0"+help; //0 auffüllen, um die Länge 5 zu erhalten
    }
    help.toCharArray(buf,5);
 Wire.write(buf);  //buffer senden
}



//-------------------------speichern der Werte lokal ---------------------------------
void SaveEEProm(int index,String wert){//Speichern des wertes an index
  switch(index){ //Speicherstelle anhand index herausfinden
    case 0:EEPROMAddress=0; break; //index
    case 1:EEPROMAddress=90; break;//name
    case 2:EEPROMAddress=10; break; //mintemp
    case 3:EEPROMAddress=20; break; //maxtemp
    case 4:EEPROMAddress=30; break; //minfeucht
    case 5:EEPROMAddress=40; break; //maxfeucht
    case 6:EEPROMAddress=50; break; //minhumid
    case 7:EEPROMAddress=60; break; //maxhumid
    case 8:EEPROMAddress=70; break; //minUV
    case 9:EEPROMAddress=80; break; //maxUV
    }
   // byte help[sizeof(wert)];
    //wert.getBytes(help,sizeof(wert));

    for(int f=0;f<sizeof(wert)&&f<10;f++){
       EEPROM.put(EEPROMAddress+f,wert.charAt(f));  //data speichern bis maximal 10 stellen
      }
      if(index!=1||(index==1&&sizeof(wert)<10)){
      for(int f=sizeof(wert);f<10;f++){
        EEPROM.put(EEPROMAddress+f,'$'); //freien speicherplatz makieren bis maximal 10 stellen
        }
      }
      else{
        for(int f=10;f<sizeof(wert);f++){
          EEPROM.put(EEPROMAddress+f,wert.charAt(f)); //data speichern ohne maximum
          }
          EEPROM.put(EEPROMAddress+sizeof(wert),'$');//Ende makieren
        }
  //EEPROM.commit();
  Serial.println("Save");
  }


 String GetEEProm(int index){//Daten an index im Speicher holen
     switch(index){//Speicherplatzstelle anhand index bekommen
    case 0:EEPROMAddress=0; break; //index
    case 1:EEPROMAddress=90; break;//name
    case 2:EEPROMAddress=10; break; //mintemp
    case 3:EEPROMAddress=20; break; //maxtemp
    case 4:EEPROMAddress=30; break; //minfeucht
    case 5:EEPROMAddress=40; break; //maxfeucht
    case 6:EEPROMAddress=50; break; //minhumid
    case 7:EEPROMAddress=60; break; //maxhumid
    case 8:EEPROMAddress=70; break; //minUV
    case 9:EEPROMAddress=80; break; //maxUV
    }
    String erg="";
    if(EEPROMAddress!=90){//Alles außer Name, da Name keine begrenzung
      for(int f=0;f<10;f++){
        char help=(char)EEPROM.read(EEPROMAddress+f); //Data lesen
        if(help!='$') erg+=help; //stoppen an $
        else return erg;
        }
      }
      else{
for(int f=0;true;f++){
        char help=(char)EEPROM.read(EEPROMAddress+f); //DAta lesen
        if(help!='$') erg+=help;//stoppen an $
        else return erg;
        }
      }
    return erg;
    }
