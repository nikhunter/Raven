#include <Arduino.h>
#include <SPI.h>
#include <OBD.h>
#include <MultiLCD.h>
#include <TinyGPS.h>
#include <SD.h>
#include <ArduinoJson.h>
#include "Raven.h"
#include "RavenOBD.h"
#include "images.h"

#define PixSizeX  500
#define PixOffsX  0

#define PixSizeY  300
#define PixOffsY  0

void setup() {
  // put your setup code here, to run once:
  lcd.begin();

  BT.begin(9600);
  lcd.setXY(250 - (128 / 2),150 - (128 / 2));
  lcd.draw(*logo, 128,128);
  lcd.setXY(250 - (122 / 2),150 + (128 / 2) + 5);
  lcd.draw(*logotext, 122,20);
  //delay(500);
  obd.begin();  
  delay(3000);
  lcd.clear();
  hasMEMS = obd.memsInit();
  lcd.print("MEMS:");
  lcd.println(hasMEMS ? "Yes" : "No");

#if !DEBUG_MODE
  // send some commands for testing and show response for debugging purpose
  testOut();

  // initialize OBD-II adapter
  while(!obd.init()) {
    lcd.setCursor(0,0);
    lcd.println("Standby for OBD Connection");
  }
#endif

  char buf[64];
  if (obd.getVIN(buf, sizeof(buf))) {
      lcd.print("VIN:");
      lcd.println(buf);
  }
  
  unsigned int codes[6];
  byte dtcCount = obd.readDTC(codes, 6);
  if (dtcCount == 0) {
    lcd.println("No DTC"); 
  } else {
    lcd.print(dtcCount); 
    lcd.print(" DTC:");
    for (byte n = 0; n < dtcCount; n++) {
      lcd.print(' ');
      lcd.print(codes[n], HEX);
    }
    lcd.println();
  }
  lcd.clear();
  lcd.setFontSize(FONT_SIZE_XLARGE);
  lcd.print("STATUS: ");
  lcd.setColor(RGB16_GREEN);
  lcd.println("ACTIVE");
  lcd.println();
  lcd.setColor(RGB16_WHITE);
  delay(3000);
}

void loop() {
  //readPIDs();
  processGPS();
  lcd.setCursor(0,2);
  lcd.print("lat: ");
  lcd.print((float)lat / 100000, 5);
  lcd.setCursor(0,4);
  lcd.print("lng: ");
  lcd.print((float)lng / 100000, 5);
  /* if (hasMEMS) {
    readMEMS();
  }*/
  
}
