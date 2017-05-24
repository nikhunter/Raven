#include <Arduino.h>
#include <OBD.h>
#include <MultiLCD.h>
#include <TinyGPS.h>
#include <SD.h>
#include "Raven.h"
#include "RavenOBD.h"

void setup() {
  // put your setup code here, to run once:
  lcd.begin();

  BT.begin(9600);
  delay(500);
  obd.begin();

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
  lcd.setFontSize(FONT_SIZE_SMALL);
  lcd.setColor(RGB16_WHITE);
  delay(3000);
}

void loop() {
  readPIDs();

  if (GPS.available()){
    BT.println(GPS.read());  
  }
  
  /* if (hasMEMS) {
    readMEMS();
  }*/
  
}
