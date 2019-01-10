//Hung Truong - Besondere Lernleistung - Gewächshaus-Automatisierung

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
int EEPROMend;
bool IsActive;
String erg;
WiFiClient client;
WiFiServer server(5001);
bool MinAction[] = {0, 0, 0, 0}, MaxAction[] = {0, 0, 0, 0};

//------------------------SetUp-----------------------------
void setup() {
  Serial.begin(9600);
  Serial.println("----------Start-------------");
  Serial1.begin(96000);
  ConnectToWifi();
  server.begin();
  int id = GetFromSave(0);
  if (id == 0)
  {
    erg = SendToServer("new arduino");
    IsActive = false;
  }
  else
  {
    erg = SendToServer("reconect arduino_" + id);
    IsActive = true;
  }
  if (erg != "Success") {
    SetToSave(erg + "." + GetFromSave(1) + "," + GetFromSave(11) + ":" + GetFromSave(2) + "-" + GetFromSave(21) + ";" + GetFromSave(3) + "_" + GetFromSave(31) + "+" + GetFromSave(4) + "*" + GetFromSave(41));
  }
}

void loop() {
  MessageFromCSharpServer();
  if (IsActive) {
    if (Serial1.available()) {
      Check();
    }
  }
}



void Check() {
  delay(100);
  String rcv = Serial1.readStringUntil('\n');
  SendToServer(rcv);
  Serial.print(rcv);
  float data[] = {0, 0, 0, 0};
  data[0] = rcv.substring(0, rcv.indexOf(".")).toFloat();
  data[1] = rcv.substring(rcv.indexOf("."), rcv.indexOf(",")).toFloat();
  data[2] = rcv.substring(rcv.indexOf(","), rcv.indexOf("-")).toFloat();
  data[3] = rcv.substring(rcv.indexOf("-"), rcv.length()).toFloat();
  //1_MinTemp 11_MaxTemp
  //2_MinHumid 21_MaxHumid
  //3_MinGroundHumid 31_MaxGroundHumid
  //4_MinUV 41_MaxUV
  float imin, imax, middle;
  for (int f = 0; f < 4; f++) {
    imin = GetFromSave(f + 1);
    imax = GetFromSave(f * 10 + 1);
    middle = (imin + imax) / 2;
    if (data[f] < imin) { //Unter Min
      MinAction[f] = true;
    }
    else if (data[f] > imax) { //Über Max
      MaxAction[f] = true;
    }
    if (MinAction[f] && data[f] > middle) {
      MinAction[f] = false;
    }
    else if (MaxAction[f] && data[f] < middle) {
      MaxAction[f] = false;
    }
  }
  bool sendt[8];
  sendt[0] = MinAction[0];
  sendt[1] = MinAction[1];
  sendt[2] = MinAction[2];
  sendt[3] = MinAction[3];
  sendt[4] = MaxAction[0];
  sendt[5] = MaxAction[1];
  sendt[6] = MaxAction[2];
  sendt[7] = MaxAction[3];
  Serial1.write(ToByte(sendt));
}
unsigned char ToByte(bool b[8])
{
  unsigned char c = 0;
  for (int i = 0; i < 8; ++i)
    if (b[i])
      c |= 1 << i;
  return c;
}
// --------------------Communication with local C# server ----------
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
    SetToSave(help);
    IsActive = true;
  }
}


String SendToServer(String command) { //send to server
  command = "data_" + command;
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
float GetFromSave(int index) {
  String erg = "";
  for (int f = 0; f < EEPROMend; f++) {
    erg += char(EEPROM.read(f));
  }
  switch (index) {
    case 0:
      return erg.substring(0, erg.indexOf("§")).toFloat();
    //index
    case 1:
      return erg.substring(erg.indexOf("§"), erg.indexOf(",")).toFloat();
    //MinTemp
    case 11:
      return erg.substring(erg.indexOf(","), erg.indexOf(":")).toFloat();
    //MaxTemp
    case 2:
      return erg.substring(erg.indexOf(":"), erg.indexOf("-")).toFloat();
    //MinHumid
    case 21:
      return erg.substring(erg.indexOf("-"), erg.indexOf(";")).toFloat();
    //MaxHumid
    case 3:
      return erg.substring(erg.indexOf(";"), erg.indexOf("_")).toFloat();
    //MinGroundHumid
    case 31:
      return erg.substring(erg.indexOf("_"), erg.indexOf("+")).toFloat();
    //MaxGroundHumid
    case 4:
      return erg.substring(erg.indexOf("+"), erg.indexOf("*")).toFloat();
    //MinUV
    case 41:
      return erg.substring(erg.indexOf("*"), erg.length()).toFloat();
    //MaxUV
    default: return 0;
  }
}
void SetToSave(String save) {
  char help2[save.length()];
  save.toCharArray(help2, save.length());
  for (int f = 0; f < save.length(); f++) {
    EEPROM.write(f, help2[f]);
  }
  EEPROM.commit();
  EEPROMend = save.length();
}
