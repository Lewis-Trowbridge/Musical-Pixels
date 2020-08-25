#include <Adafruit_NeoPixel.h>
Adafruit_NeoPixel pixels(16, 6);
char recieveBuffer [48];

void setup() {
  // put your setup code here, to run once:
  pixels.begin();
  pixels.setBrightness(10);
  pixels.show();
  Serial.begin(9600);
  while (!Serial){
    ;
  }
}

void loop() {
  // put your main code here, to run repeatedly:
  if (Serial.available()){
    Serial.readBytes(recieveBuffer, 48);
    int channelNum = 0;
    for (int pixelNum = 0; pixelNum < 16; channelNum = 3*(++pixelNum)){
      pixels.setPixelColor(pixelNum, recieveBuffer[channelNum], recieveBuffer[channelNum + 1], recieveBuffer[channelNum + 2]);
    }
    pixels.show();
  }
}
