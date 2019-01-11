//-------------Einbinden seperater Bibilotheken-------------------------
//#include <DHTesp.h>
#include <DHT.h>
//#include <EEPROM.h>
#include <WiFi.h>
#include <Preferences.h>
#define LightSensor 34
#define GroundHumidSensor 36
#define TempHumidSensor 17
#define pump 32 //TODO - Ausprobieren
#define heater 33 //TODO -Zu wenig RELAYS!!!!
#define cooler 25
#define sprayer 26
#define uvLight 27
#define shutters 14
#define DHTTYPE DHT11


//--------------------------initalisieren-------------------
const int leng = 20;
const char* ssid     = "VTHHH";
const char* password = "v19t25h16h06h11";
const char* host = "192.168.178.26";
const int serverPort = 5000;
WiFiClient client, c1;
WiFiServer server(serverPort);
bool IsActive = false, live = false;
int sendcounter = 0, tempI = 0, humidI = 0, groundHumidI = 0, lightI = 0;
float temp[leng], humid[leng], groundHumid[leng], light[leng];
//DHTesp dht;
DHT dht(TempHumidSensor, DHTTYPE);
Preferences preferences;
int higherPin[] {
  heater, sprayer, pump, uvLight
},
lowerPin[] {
  cooler, 0, 0, shutters
}, high[] {
  0, 0, 0, 0
}, low[] {
  0, 0, 0, 0
};

void setup() {

  Serial.begin(9600);
  for (int f = 0; f < leng; f++) {
    temp[f] = -100;
    humid[f] = -100;
    groundHumid[f] = -100;
    light[f] = -100;
  }
  //SetToSave("0§-100$-100%-100&-100/-100(-100)-100");
  pinMode(LightSensor, INPUT);
  pinMode(GroundHumidSensor, INPUT);
  pinMode(TempHumidSensor, INPUT);
  pinMode(pump, OUTPUT);
  pinMode(heater, OUTPUT);
  pinMode(cooler, OUTPUT);
  pinMode(sprayer, OUTPUT);
  pinMode(uvLight, OUTPUT);
  pinMode(shutters, OUTPUT);
  digitalWrite(higherPin[0], LOW);
  digitalWrite(higherPin[1], LOW);
  digitalWrite(higherPin[2], LOW);
  digitalWrite(higherPin[3], LOW);
  digitalWrite(lowerPin[0], LOW);
  digitalWrite(lowerPin[3], LOW);
  high[0] = 0;
  high[1] = 0;
  high[2] = 0;
  high[3] = 0;
  low[0] = 0;
  low[3] = 0;




  // put your setup code here, to run once:
  //dht.setup(TempHumidSensor, DHTesp::DHT11);
  dht.begin();
  Serial.println("----------Start-------------");
  Connecting();

  Serial.println("\n\nStarting Server");
  server.begin();
  Serial.print("Server gestartet unter Port ");
  Serial.println(serverPort);

  Serial.println("\n\nCheck ID");
  preferences.begin("storage", false);
  int id = preferences.getUInt("id", 0);
  preferences.end();
  Serial.println("Checking");
  Serial.println(id);

  if (id < 1) {
    Serial.println("new");
    SendMessage("new arduino", false);
  }
  else {
    Serial.println("reconect");
    SendMessage("reconect arduino_" + String(id), false);
  }
  /* if (erg != "Success") {
     if (erg.substring(0, 1) == "_") {
       SetToSave(erg + ". , : - ; _ + *");
     }
     else {
       SetToSave(erg);
       IsActive = true;
     }
    }*/
}

void loop() {
  // put your main code here, to run repeatedly:
  if (live) {
    LiveLoop();

  }
  else {
    NonLiveLoop();

  }
  if (IsActive) {
    //TempAndHumidity lastValues = dht.getTempAndHumidity();
    if (tempI++ > leng) {
      tempI = 0;
    }
    if (humidI++ > leng) {
      humidI = 0;
    }
    if (groundHumidI++ > leng) {
      groundHumidI = 0;
    }
    if (lightI++ > leng) {
      lightI = 0;
    }
    temp[tempI] = dht.readTemperature();//lastValues.temperature;
    humid[humidI] = dht.readHumidity();//lastValues.humidity;
    groundHumid[groundHumidI] = analogRead(GroundHumidSensor);
    light[lightI] = analogRead(LightSensor);
    //Serial.println(analogRead(LightSensor));
    Serial.print("-------------------------------------------------------------");
    Serial.println(sendcounter);
    if (sendcounter++ > 500) {
      sendcounter = 0;
      preferences.begin("storage", false);
      int id = preferences.getUInt("id", 0);
      preferences.end();
      SendMessage("" + String(id) + "_" + String(GetAverage(temp)) + "_" + GetAverage(humid) + "_" + GetAverage(groundHumid) + "_" + ((int)GetAverage(light)), true);
    }
  }
  // Serial.println("Message?");
}
void NonLiveLoop() {
  if (IsActive) {
    Check();
  }
  GetMessage();
}



float GetAverage(float array_[leng]) {
  float help = 0;
  int help2 = 0;
  for (int f = 0; f < leng; f++) {
    if (array_[f] != -100) {
      help += array_[f];
      help2++;
    }
  }
  if (help2 == 0) {
    return 0;
  }
  return help / help2;
}

void Check() {
  float data[3] = {GetAverage(temp), GetAverage(humid), GetAverage(groundHumid)};
  preferences.begin("storage", false);
  float Min[3] = {preferences.getFloat("MinTemp", -100) , preferences.getFloat("MinHumid", -100), preferences.getFloat("MinGroundHumid", -100)};
  float Max[3] = {preferences.getFloat("MaxTemp", -100) , preferences.getFloat("MaxHumid", -100) , preferences.getFloat("MaxGroundHumid", -100)};
  preferences.end();
  float middle;
  for (int f = 0; f < 3; f++) {
    middle = (Min[f] + Max[f]) / 2;
    switch (f) {
      case 0:    Serial.print("Vergleich Temperatur "); break;
      case 1:    Serial.print("Vergleich Luftfeuchtigkeit "); break;
      case 2:    Serial.print("Vergleich Bodenfeuchtigkeit "); break;
    }
    Serial.print(": min-");
    Serial.print(Min[f]);
    Serial.print(" max-");
    Serial.print(Max[f]);
    Serial.print(" middle-");
    Serial.print(middle);
    Serial.print(" value-");
    Serial.println(data[f]);
    if (data[f] < Min[f]) { //Unter Min
      digitalWrite(higherPin[f], HIGH);
      high[f] = 1;
    }
    else if (data[f] > Max[f] && f != 2 && f != 1) { //Über Max
      digitalWrite(lowerPin[f], HIGH);
      low[f] = 1;
    }
    else if (data[f] > middle) {
      digitalWrite(higherPin[f], LOW);
      high[f] = 0;
    }
    else if (data[f] < middle) {
      digitalWrite(lowerPin[f], LOW);
      low[f] = 0;
    }
  }
  //TODO light Check
  preferences.begin("storage", false);
  int x = preferences.getUInt("Light", -100), counter = 0;
  Serial.print("Vergleich Licht");
  Serial.print(": wert-");
  Serial.print(x);
  Serial.print(" value-");
  for (int f = 0; f < x && f < leng; f++) {
    if (light[leng - f] == -100) {
      x++;
    }
    else if (light[leng - f] > 1000) {
      counter++;
    }
  }
  Serial.println(counter);
  if (counter == preferences.getUInt("Light", -100)) {
    digitalWrite(lowerPin[3], HIGH);
    low[3] = 1;
  }
  else if (counter == 0 ) {
    digitalWrite(higherPin[3], HIGH);
    high[3] = 1;
  }
  else if ( counter < 3) {
    digitalWrite(lowerPin[3], LOW);
    low[3] = 0;
  }
  else if ( counter > 1) {
    digitalWrite(higherPin[3], LOW);
    high[3] = 0;
  }
  preferences.end();
}


void Connecting() {
  Serial.print("\n\nConnecting to ");
  Serial.println(ssid);

  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("\nWiFi connected\nIP address: ");
  Serial.println(WiFi.localIP());
}

void GetMessage() {
  String message = "";
  WiFiClient client_ = server.available();
  if (client_) {
    Serial.println("\n\nConnected");
    while (client_.connected()) {
      if (client_.available()) {
        char c = client_.read(); //read command from Server
        if (c == '|' ) {
          if (message.indexOf("_") == 0) {
            live = true;
            byte ip[4];
            ip[0] = message.substring(0, message.indexOf(".")).toInt();
            String ip_help = message.substring(message.indexOf(".") + 1, message.length() + 1);
            ip[1] = ip_help.substring(0, message.indexOf(".")).toInt();
            ip_help = ip_help.substring(message.indexOf(".") + 1, message.length() + 1);
            ip[2] = ip_help.substring(0, message.indexOf(".")).toInt();
            ip[3] = ip_help.substring(message.indexOf(".") + 1, message.length() + 1).toInt();
            c1.connect(ip, 5000);
          }
          else {
            SetToSave(message);
          }
          client_.println("Done<EOF>");
          client_.flush();
          client_.stop();
          Serial.print("Message from Socket: ");
          Serial.println(message);
          return;
        }
        else {
          message += c;
        }
      }
    }
  }
}

String SendMessage(String message, bool data_) {
  if (data_) {
    message = "set arduino data_" + message;
  }
  host = "192.168.178.26";
  Serial.print("Connecting to ");
  Serial.print(host);
  Serial.print(":");
  Serial.println(serverPort);
  while (!client.connect(host, serverPort)) {
    Serial.print("X");
  }

  Serial.print("Send to server: ");
  Serial.println(message);
  message += "_<EOF>";
  client.println(message); //send
  String erg = client.readStringUntil('|'); //wait for answer
  Serial.print(erg);
  Serial.println("\nClosing connection");
  client.println("Done<EOF>");
  client.flush();
  client.stop();
  Serial.println("Close connection");
  return erg;

}

void SetToSave(String save) {
  if (save.substring(save.indexOf("a") + 1, save.indexOf("b")) == "-100") {
    IsActive = false;
    digitalWrite(higherPin[0], LOW);
    digitalWrite(higherPin[1], LOW);
    digitalWrite(higherPin[2], LOW);
    digitalWrite(higherPin[3], LOW);
    digitalWrite(lowerPin[0], LOW);
    digitalWrite(lowerPin[3], LOW);
    high[0] = 0;
    high[1] = 0;
    high[2] = 0;
    high[3] = 0;
    low[0] = 0;
    low[3] = 0;
  }
  else {
    IsActive = true;
  }
  save.replace(",", ".");
  preferences.begin("storage", false);
  preferences.putUInt("id", save.substring(0, save.indexOf("a")).toFloat());
  preferences.putFloat("MinTemp", save.substring(save.indexOf("a") + 1, save.indexOf("b")).toFloat());
  preferences.putFloat("MaxTemp", save.substring(save.indexOf("b") + 1, save.indexOf("c")).toFloat());
  preferences.putFloat("MinGroundHumid", save.substring(save.indexOf("c") + 1, save.indexOf("d")).toFloat());
  preferences.putFloat("MaxGroundHumid", save.substring(save.indexOf("d") + 1, save.indexOf("e")).toFloat());
  preferences.putFloat("MinHumid", save.substring(save.indexOf("e") + 1, save.indexOf("f")).toFloat());
  preferences.putFloat("MaxHumid", save.substring(save.indexOf("f") + 1, save.indexOf("g")).toFloat());
  switch (save.substring(save.indexOf("g") + 1, save.length() + 1).toInt()) {
    case 0:   preferences.putUInt("Light", 6);  break;
    case 1:   preferences.putUInt("Light", 4);  break;
    case 2:   preferences.putUInt("Light", 2);  break;
  }
  /* String test = "-100";
    Serial.println(test.toFloat(), 3);
    Serial.println(save.substring(save.indexOf("a"), save.indexOf("b")));
    Serial.println(save.substring(save.indexOf("a"), save.indexOf("b")).toFloat(), 3);
    Serial.println(String(preferences.getFloat("MinTemp", -100)));*/
  preferences.end();
  /*
    Serial.println(save.substring(save.indexOf("a"),save.indexOf("b")));
    int length=save.length()+1;
    char help2[length];
    save.toCharArray(help2, length);
    preferences.putUInt("length",length);
    //EEPROM.write(0,length);
    Serial.print("\n\nLänge: ");
    Serial.println(length);
    Serial.print("Write: _");
    for(int f=0;f<length;f++){
      EEPROM.write(f+1,help2[f]);
      preferences.putUChar("data"+f,help2[f]);
      Serial.print(help2[f]);
    }
    Serial.println("_");
    EEPROM.writeBlock<char>(1, save, save.length());
    EEPROM.write(0,save.length());
    EEPROM.commit();*/
}

void LiveLoop() {
  String onoff = "" + high[0] + high[1] + high[2] + high[3] + low[0] + low[3];
  c1.println(String(GetAverage(temp)) + "_" + GetAverage(humid) + "_" + GetAverage(groundHumid) + "_" + ((int)GetAverage(light)) + "_" + onoff + "|");
  String message = "";
  while (c1.connected()) {
    if (c1.available()) {
      char c = c1.read(); //read command from Server
      if (c == '|' ) {
        if (message == "live off") {
          live = false;
          c1.println("Done<EOF>");
          c1.flush();
          c1.stop();
          return;
        }
        Serial.print("Message from Socket: ");
        Serial.println(message);
        char datas[6];
        message.toCharArray(datas, 6);
        for (int f = 0; f < 4; f++) {
          if (datas[f] == 1) {
            digitalWrite(higherPin[f], HIGH);
            high[f] = 1;
          }
          else {
            digitalWrite(higherPin[f], LOW);
            high[f] = 0;
          }
        }
        if (datas[4] == 1) {
          digitalWrite(lowerPin[0], HIGH);
          low[0] = 1;
        }
        else {
          digitalWrite(lowerPin[0], LOW);
          low[0] = 0;
        }
        if (datas[5] == 1) {
          digitalWrite(lowerPin[3], HIGH);
          low[3] = 1;
        }
        else {
          digitalWrite(lowerPin[3], LOW);
          low[3] = 0;
        }
      }
      else {
        message += c;
      }
    }
  }
}
