//Hung Truong - Besondere Lernleistung - Gewächshaus-Automatisierung
//NodeMCU

//-------------Einbinden seperater Bibilotheken-------------------------
#include <Wire.h> //Bibilothek für Master-Slave-Kommunikation mit Slave AMega2560
#include <EEPROM.h> //Bibilothek zum Schreiben in den Speicher
#include <ESP8266WiFi.h> //Bibilothek für Server Kommunikation

//--------------------------initalisieren-------------------
const char* ssid     = "VT"; //Name des Wifi
const char* password = "1631492500942971"; //Code des Wifi
const char* host = "192.168.178.78"; //IP-Addresse des Server
const int serverPort=5000; //Port des C# Server
int wifiStatus; // Status des Wifi
int counter=0;
bool IsActive;
WiFiClient client; //Client für erste Kommunikation mit C# Server
WiFiServer server(5001); //Server für Kommunikation mit C# Client

//------------------------SetUp-----------------------------
void setup() {
  IsActive=false;
  Serial.begin(9600); //Serial Communication für Debug starten
  Wire.begin(D1, D2); //I2C Bus an SDA(D1) und SCL(D2)
  Serial.println("-----------Start--------------");
  ConnectToWifi(); //Zu Wifi verbinden
  server.begin();
  int id=GetID();
  String erg="";
  if(id==0)
  {
    erg=SendToServer("new arduino"); //An Server melden, dass Arduino erreichbar
  }
  else 
  {
    erg=SendToServer("reconect arduino_"+id); //An Server melden, dass Arduino erreichbar
    IsActive=true;
  }
  if(erg!="Success"){
    SaveID(erg.toInt());
    }
}

//-----------------------Loop------------------------------
void loop() {
  //TODO auf ServerKommunikation warten
  MessageFromCSharpServer();
   if(IsActive){
  if(counter++==10){
    counter=0;
    String request=GetFromArduino(); //Request an Slave AMega2560
    SendToServer("set arduino data_"+request); //An Server schicken
  }
  }
  delay(1000);
}

//-----------------Master-Slave.Kommunikation mit AMega2560 -------------------
void SendToArduino(String message){ //String an Slave AMega2560 senden
   char buffer_[sizeof(message)];
  message.toCharArray(buffer_,sizeof(message));
  Wire.beginTransmission(8); //I2C Bus an Adresse 8
 Wire.write(buffer_);  //string senden
 Wire.endTransmission();    //Übertragung beenden
  }

  String GetFromArduino(){ //Request von Slave AMega2560
    String erg="";
    Wire.requestFrom(8, 5); //erwarte einen 5 byte langen Wert an Adresse 8
 while(Wire.available()){
    erg+= Wire.read(); //bytes lesen, solange Wire available ist
  }
  return erg;
    }


    //--------------------Kommunikation mit lokalem C# Server ----------
 void ConnectToWifi(){ //Zu Wifi verbinden
   Serial.print("Your are connecting to;");
      Serial.println(ssid);

      WiFi.begin(ssid, password); //verbinden

      while (WiFi.status() != WL_CONNECTED) {//Solange nicht verbunden, erneut versuchen nach 500ms
        delay(500);
        Serial.println("WiFi not connected");
      }
      //verbunden
      Serial.println("");
         Serial.println("Your ESP is connected!");
         Serial.println("Your IP address is: ");
         Serial.println(WiFi.localIP());
  }

  
//Methode, die Befehle vom C# Client einließt
void MessageFromCSharpServer(){
  String help="";
    WiFiClient client2=server.available(); //Überprügung ob Client versucht Server Daten zu schicken
    if(client2){
      while(client2.connected()){ //Wenn der Client verbunden
        if(client2.available()){ //Wenn der Client Daten sendet
          char c=client2.read(); //Zeichen auslesen
          if(c=='|'){ //End Zeichen ausfiltern
            client2.println("Done<EOF>"); //antworten, dass nachricht angekommen ist
            client2.stop(); //Verbindung beenden
            }
            else{
              help+=c; //Zeichen speichern
              }
          }
        }
        SendToArduino(help); //Zeichenkette als Befehl ausführen und speichern
      }
  }


  String SendToServer(String command){ //Zum Sever senden
  if (!client.connect(host, serverPort)) {Serial.print("X");} //Wenn keine Verbindung zum Server
 else{
  command+="_<EOF>"; //Ende-Flagge anhängen
  client.println(command); //senden
  String erg=client.readStringUntil('|'); //warten auf antwort
  Serial.print(erg);
 Serial.println();
 Serial.println("Verbindung schliessen");
 client.flush(); //Verbindung schließen
 client.stop();
 Serial.println("Verbindung geschlossen");
 return erg;
  }
  }

  //-------------------------speichern der id lokal ---------------------------------
  void SaveID(int id){//Speichern der id
    EEPROM.write(0,id);
    EEPROM.commit();
    Serial.println("Save");
    }


   int GetID(){//id aus dem Speicher holen
      return EEPROM.read(0);
      }
