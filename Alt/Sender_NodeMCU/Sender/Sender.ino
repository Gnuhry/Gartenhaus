//Hung Truong - Besondere Lernleistung - Gewächshaus-Automatisierung
//NodeMCU

//-------------Einbinden seperater Bibilotheken-------------------------
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
  Serial1.begin(10000);
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
      String request = ToMega2560("request"); //Request Data
      SendToServer("set arduino data_" + request);
    }
  }
}

//-----------------Master-Slave Communication with AMega2560 -------------------
String ToMega2560(String tx) {
  Serial1.println(tx + "\n");
  while (true) {
    if (Serial1.available()) {
      delay(100);
      String rcv = Serial1.readStringUntil('\n');
      Serial.print(rcv);
      return rcv;
    }
  }
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
        if (c == '|' ) {
          client2.println("Done<EOF>");
          client2.stop();
        }
        else {
          help += c;
        }
      }
    }
    Serial.println(help);
    int seperator2 = help.indexOf(";");
    SaveID(help.substring(0, seperator2).toInt());
    ToMega2560(help.substring(seperator2 + 1, help.length() + 1));
  }
}


String SendToServer(String command) { //send to server
  Serial.println(command);
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
  if (id < 1) {
    IsActive = false;
  }
  else {
    IsActive = true;
  }
  EEPROM.write(0, id);
  EEPROM.commit();
  Serial.println("Save");
}


int GetID() {
  return EEPROM.read(0);
}
