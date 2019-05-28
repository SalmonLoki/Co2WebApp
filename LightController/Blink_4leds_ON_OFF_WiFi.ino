#include "ESP8266WiFi.h"

const char* ssid = "toki";
const char* password = "bananana";

//they should be diff
int main_led = 2 ; //D4
int green_led = 16; //D0 GPIO16
int yellow_led = 5; //D1 GPIO5
int red_led = 4; //D2 GPIO4
int light;
char* state;

WiFiServer server(80);

void setup() {
  Serial.begin(115200);
  
  pinMode(main_led, OUTPUT);
  pinMode(green_led, OUTPUT);
  pinMode(yellow_led, OUTPUT);
  pinMode(red_led, OUTPUT);
  digitalWrite(main_led, HIGH);
  digitalWrite(green_led, HIGH);
  digitalWrite(yellow_led, HIGH);
  digitalWrite(red_led, HIGH);
  light = 0;
  state = "";
  Serial.println("Leds are OFF now");

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED){
    delay(500);
    Serial.print(".");
  }
  Serial.println();
  Serial.println("ssid connected");

  server.begin();
  Serial.print("server started : connect to http://");
  Serial.print(WiFi.localIP());
  Serial.println("/");
  Serial.println("Setup done");
}

void loop() {
  WiFiClient client = server.available();
  if (!client){
    return;
  }

  while (!client.available()){
    delay(1);
  }

  String request = client.readStringUntil('\r');
  Serial.print("request: ");
  Serial.println(request);
  client.flush();

  if (request.indexOf("/light/on") != -1){
    digitalWrite(main_led, LOW);
    light = 1;
  }
  
  if (request.indexOf("/light/off") != -1){
    digitalWrite(main_led, HIGH);
    light = 0;
  }

  if (request.indexOf("/state/green") != -1){
    digitalWrite(yellow_led, HIGH);
    digitalWrite(red_led, HIGH);
    digitalWrite(green_led, LOW);
    state = "GREEN";
  }
  if (request.indexOf("/state/yellow") != -1){
    digitalWrite(green_led, HIGH);
    digitalWrite(red_led, HIGH);
    digitalWrite(yellow_led, LOW);
    state = "YELLOW";
  }
  if (request.indexOf("/state/red") != -1){
    digitalWrite(green_led, HIGH);
    digitalWrite(yellow_led, HIGH);
    digitalWrite(red_led, LOW);
    state = "RED";
  }
  if (request.indexOf("/state/off") != -1){
    digitalWrite(green_led, HIGH);
    digitalWrite(yellow_led, HIGH);
    digitalWrite(red_led, HIGH);
    state = "";
  }
  
  // Return the response 
  
  
  if(light == 1) {
    client.println("main led is ON");
  } else {
    client.println("main led is OFF");
  }
  client.print("colour state: ");
  client.println(state);
  
  delay(1);
}
