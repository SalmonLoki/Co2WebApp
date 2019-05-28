#include "ESP8266WiFi.h"
#include "ESP8266HTTPClient.h"

const char* ssid = "toki";
const char* password = "bananana";

int main_led = 16; //D0 GPIO16
int green_led = 5; //D1 GPIO5
int yellow_led = 4; //D2 GPIO4
int red_led = 2 ; //D4 GPIO2

int light;
char* state;

WiFiServer server(80);
String request;

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
  if (client){
    Serial.println("New client");
    String currentLine = "";
    while (client.connected()) {
      if (client.available()) {
        char c = client.read();
        request += c;
        Serial.write(c);
        if (c == '\n') {
          if (currentLine.length() == 0) {
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
            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:application/json");
            client.println("Connection: close");
            client.println();
            client.println("[");
            client.println("  {");
            client.print("    \"main led\": ");                        
            if (light == 1){
              client.println("\"ON\",");
            } else {
              client.println("\"OFF\",");
            }
            client.print("    \"state\": ");
            client.print("\"");
            client.print(state);
            client.println("\"");
            client.println("  }");
            client.println("]");           
            client.println();
            break;
          } else {
            currentLine = "";
          }
        } else if (c != '\r') {
          currentLine += c;
        }
      }
    }

    request = "";
    client.stop();
    Serial.println("Client disconnected.");
    Serial.println("");
  }
}
