//Hung Truong - Besondere Lernleistung - Gewächshaus-Automatisierung
//NodeMCU

//-------------Einbinden seperater Bibilotheken-------------------------
#include <Wire.h>
#include <EEPROM.h>
#include <ESP8266WiFi.h>

//--------------------------initalisieren-------------------
const char* ssid     = "VTHHH";
const char* password = "v19t25h16h06h11";
const char* host = "192.168.178.29";
const int serverPort = 5000;
int wifiStatus;
int counter = 0;
bool IsActive;
WiFiClient client;
WiFiServer server(5001);

//------------------------SetUp-----------------------------
void setup() {
  IsActive = false;
  Serial.begin(9600); //start Serial Communication
  Wire.begin(D1, D2); //start Wire Communication
  Serial.println("-----------Start--------------");
  ConnectToWifi();
  server.begin();
  int id = GetID();
  String erg = "";
  //connect to server
  if (id == 0)
  {
    erg = SendToServer("new arduino");
  }
  else
  {
    erg = SendToServer("reconect arduino_" + id);
    IsActive = true;
  }
  if (erg != "Success") {
    SaveID(erg.toInt());
  }
}

//-----------------------Loop------------------------------
void loop() {
  MessageFromCSharpServer();
  if (IsActive) {
    if (counter++ == 10) {
      counter = 0;
      String request = GetFromArduino(); //Request Data
      SendToServer("set arduino data_" + request);
    }
  }
  delay(1000);
}

//-----------------Master-Slave Communication with AMega2560 -------------------
void SendToArduino(String message) {
  char buffer_[sizeof(message)];
  message.toCharArray(buffer_, sizeof(message));
  Wire.beginTransmission(8);
  Wire.write(buffer_);
  Wire.endTransmission();
}

String GetFromArduino() {
  String erg = "";
  Wire.requestFrom(8, 5);
  while (Wire.available()) {
    erg += Wire.read();
  }
  return erg;
}


//--------------------Communication with local C# server ----------
void ConnectToWifi() {
  Serial.print("Your are connecting to;");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {//try to connect evere 500 ms
    delay(500);
    Serial.println("WiFi not connected");
  }
  //connected
  Serial.println("");
  Serial.println("Your ESP is connected!");
  Serial.println("Your IP address is: ");
  Serial.println(WiFi.localIP());
}



void MessageFromCSharpServer() {
  String help = "";
  WiFiClient client2 = server.available();
  if (client2) {
    while (client2.connected()) {
      if (client2.available()) {
        char c = client2.read(); //read command from Server
        if (c == '|') {
          client2.println("Done<EOF>");
          client2.stop();
        }
        else {
          help += c;
        }
      }
    }
    int seperator = help.indexOf("_");
    String temp = help.substring(0, seperator);
    if (temp == "Id") {//save ID
      SaveID(help.substring(seperator + 1, sizeof(help)).toInt());
    }
    else {
      SendToArduino(help);
    }
  }
}


String SendToServer(String command) { //send to server
  if (!client.connect(host, serverPort)) {
    Serial.print("X");
  }
  else {
    command += "_<EOF>";
    client.println(command); //send
    String erg = client.readStringUntil('|'); //wait for answer
    Serial.print(erg);
    Serial.println();
    Serial.println("Closing connection");
    client.flush();
    client.stop();
    Serial.println("Close connection");
    return erg;
  }
}

//-------------------------local save of the Arduino ID ---------------------------------
void SaveID(int id) {
  EEPROM.write(0, id);
  EEPROM.commit();
  Serial.println("Save");
}


int GetID() {
  return EEPROM.read(0);
}
