//-------------Einbinden seperater Bibilotheken-------------------------
#include <DHTesp.h>
#include <EEPROM.h>
#include <WiFi.h>

#define LightSensor 39
#define GroundHumidSensor 36
#define TempHumidSensor 17
#define pump 32 //TODO - Ausprobieren
#define heater 33 //TODO -Zu wenig RELAYS!!!!
#define cooler 25
#define sprayer 26
#define uvLight 27
#define shutters 14

//--------------------------initalisieren-------------------
const char* ssid     = "VTHHH";
const char* password = "v19t25h16h06h11";
const char* host = "192.168.178.29";
const int serverPort = 5000, localserverport = 5001;
WiFiClient client;
WiFiServer server(localserverport);

DHTesp dht;
bool IsActive=false;
int sendcounter=0, tempI=0, humidI00, groundHumidI=0, lightI=0;
float temp[] = { 
  -100, -100, -100, -100, -100, -100, -100, -100, -100, -100
  }, 
humid[] = { 
  -100, -100, -100, -100, -100, -100, -100, -100, -100, -100
  }, 
groundHumid[] = { 
  -100, -100, -100, -100, -100, -100, -100, -100, -100, -100
  }, 
light[] = {
  -100, -100, -100, -100, -100, -100, -100, -100, -100, -100
  };
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

void setup() {
   // put your setup code here, to run once:
  pinMode(LightSensor, INPUT);
  pinMode(GroundHumidSensor, INPUT);
  pinMode(TempHumidSensor, INPUT);
  pinMode(pump, OUTPUT);
  pinMode(heater, OUTPUT);
  pinMode(cooler, OUTPUT);
  pinMode(sprayer, OUTPUT);
  pinMode(uvLight, OUTPUT);
  pinMode(shutters, OUTPUT);


  dht.setup(TempHumidSensor, DHTesp::DHT11);

  
  Serial.begin(9600);
  Serial.println("----------Start-------------");
  Connecting();
  
  Serial.println("\n\nStarting Server");
  server.begin();
  Serial.print("Server gestartet unter Port ");
  Serial.println(localserverport);

  
  int id = GetFromSave(0);
  if (id == -1) {
    SendMessage("new arduino", false);
  }
  else {
    SendMessage("reconect arduino_" + id, false);
  }
}

void loop() {
  // put your main code here, to run repeatedly:
  if (IsActive) {
    Check();

    
    TempAndHumidity lastValues = dht.getTempAndHumidity();
    if (tempI++ > 10) {
      tempI = 0;
    }
    if (humidI++ > 10) {
      humidI = 0;
    }
    if (groundHumidI++ > 10) {
      groundHumidI = 0;
    }
    if (lightI++ > 10) {
      lightI = 0;
    }
    temp[tempI] = lastValues.temperature;
    humid[humidI] = lastValues.humidity;
    groundHumid[groundHumidI] = analogRead(GroundHumidSensor);
    light[lightI] = analogRead(LightSensor);


    if (sendcounter++ > 1000) {
      sendcounter = 0;
      SendMessage(String(GetAverage(temp)) + "§" + GetAverage(humid) + "," + GetAverage(groundHumid) + "-" + GetAverage(light), true);
    }
  }

  
  GetMessage();

  
}




float GetAverage(float array_[10]) {
  float help = 0;
  int help2 = 0;
  for (int f = 0; f < 10; f++) {
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
  //1_MinTemp 11_MaxTemp
  //2_MinHumid 21_MaxHumid
  //3_MinGroundHumid 31_MaxGroundHumid
  //4_Light
  float imin, imax, middle;
  for (int f = 0; f < 3; f++) {
    imin = GetFromSave(f + 1);
    imax = GetFromSave((f + 1) * 10 + 1);
    middle = (imin + imax) / 2;
    Serial.print("Vergleich ");
    Serial.print(f);
    Serial.print(": min-");
    Serial.print(imin);
    Serial.print(" max-");
    Serial.print(imax);
    Serial.print(" middle-");
    Serial.print(middle);
    Serial.print(" value-");
    Serial.println(data[f]);
    if (data[f] < imin && higher[f] != -1) { //Unter Min
      digitalWrite(higherPin[f], HIGH);
      higher[f] = 1;
    }
    else if (data[f] > imax && lower[f] != -1) { //Über Max
      digitalWrite(lowerPin[f], HIGH);
      lower[f] = 1;
    }
    if (higher[f] == 1 && data[f] > middle) {
      digitalWrite(higherPin[f], LOW);
      higher[f] = 0;
    }
    else if (lower[f] == 1 && data[f] < middle) {
      digitalWrite(lowerPin[f], LOW);
      lower[f] = 0;
    }
  }
  //TODO light Check
  int x = GetFromSave(4), counter = 0;
  for (int f = 0; f < x; f++) {
    if (light[10 - f] == -100) {
      x++;
    }
    else if (!light[10 - f] < 1000) {
      counter++;
    }
  }
  if (counter == GetFromSave(4)) {
    digitalWrite(higherPin[4], HIGH);
    higher[4] = 1;
  }
  else if (counter == 0 && higher[4] == 1) {
    digitalWrite(higherPin[4], LOW);
    higher[4] = 0;
  }

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



String GetMessage() {
  String message = "";
  WiFiClient client_ = server.available();
  if (client_) {
    Serial.println("\n\nConnected");
    while (client_.connected()) {
      if (client_.available()) {
        char c = client_.read(); //read command from Server
        if (c == '|' ) {
          client_.println("Done<EOF>");
          client_.stop();
          Serial.print("Message from Socket: ");
          Serial.println(message);
          SetToSave(message);
          return message;
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
    message = "data_" + message;
  }
  Serial.print("Connecting to ");
  Serial.print(host);
  Serial.print(":");
  Serial.println(host);
  if (!client.connect(host, serverPort)) {
    Serial.print("X");
  }
  else {
    Serial.print("Send to server: ");
    Serial.println(message);
    message += "_<EOF>";
    client.println(message); //send
    String erg = client.readStringUntil('|'); //wait for answer
    Serial.print(erg);
    Serial.println("\nClosing connection");
    client.flush();
    client.stop();
    Serial.println("Close connection");
    return erg;
  }
}

float GetFromSave(int index) {
  String erg = "";
  for (int f = 0; char(EEPROM.read(f)) != '<'; f++) {
    erg += char(EEPROM.read(f));
  }
  String substrin;
  switch (index) {
    case 0:

      substrin = erg.substring(0, erg.indexOf("§"));
    //index
    case 1:
      substrin = erg.substring(erg.indexOf("§"), erg.indexOf(","));
    //MinTemp
    case 11:
      substrin = erg.substring(erg.indexOf(","), erg.indexOf(":"));
    //MaxTemp
    case 2:
      substrin = erg.substring(erg.indexOf(":"), erg.indexOf("-"));
    //MinHumid
    case 21:
      substrin = erg.substring(erg.indexOf("-"), erg.indexOf(";"));
    //MaxHumid
    case 3:
      substrin = erg.substring(erg.indexOf(";"), erg.indexOf("_"));
    //MinGroundHumid
    case 31:
      substrin = erg.substring(erg.indexOf("_"), erg.indexOf("+"));
    //MaxGroundHumid
    case 4:
      substrin = erg.substring(erg.indexOf("+"), erg.length()+1));
    //light
    default: substrin = "";
  }
  if (substrin != "") {
    return substrin.toFloat();
  }
  else {
    return -1;
  }
}
void SetToSave(String save) {
  char help2[save.length()];
  save.toCharArray(help2, save.length());
  for (int f = 0; f < save.length(); f++) {
    EEPROM.write(f, help2[f]);
  }
  EEPROM.write(save.length(), '<');
  EEPROM.commit();
}
