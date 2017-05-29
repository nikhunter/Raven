bool hasMEMS;
int deltatime = 0;

int32_t lat, lng;
static uint32_t distance = 0;
static uint32_t startTime = 0;
static uint16_t lastSpeed = 0;
static uint32_t lastSpeedTime = 0;
static uint32_t gpsDate = 0;
uint32_t dataTime;
static uint32_t lastGPSDataTime = 0;


void testOut()
{
    static const char cmds[][6] = {"ATZ\r", "ATH0\r", "ATRV\r", "0100\r", "010C\r", "0902\r"};
    char buf[128];

    for (byte i = 0; i < sizeof(cmds) / sizeof(cmds[0]); i++) {
        const char *cmd = cmds[i];
        lcd.print("Sending ");
        lcd.println(cmd);
        if (obd.sendCommand(cmd, buf, sizeof(buf))) {
            char *p = strstr(buf, cmd);
            if (p)
                p += strlen(cmd);
            else
                p = buf;
            while (*p == '\r') p++;
            while (*p) {
                lcd.write(*p);
                if (*p == '\r' && *(p + 1) != '\r')
                    lcd.write('\n');
                p++;
            }
        } else {
            lcd.println("Timeout");
        }
        delay(1000);
    }
    lcd.println();
    delay(5000);
    lcd.clear();
}

void readMEMS()
{
    int acc[3];
    int gyro[3];
    int temp;

    if (!obd.memsRead(acc, gyro, 0, &temp)) return;

    BT.print('[');
    BT.print(millis());
    BT.print(']');

    BT.print("ACC:");
    BT.print(acc[0]);
    BT.print('/');
    BT.print(acc[1]);
    BT.print('/');
    BT.print(acc[2]);

    BT.print(" GYRO:");
    BT.print(gyro[0]);
    BT.print('/');
    BT.print(gyro[1]);
    BT.print('/');
    BT.print(gyro[2]);

    BT.print(" TEMP:");
    BT.print((float)temp / 10, 1);
    BT.println("C");
}

void readPIDs()
{
    static const byte pidlist[] = {PID_ENGINE_LOAD, PID_COOLANT_TEMP, PID_RPM, PID_SPEED, PID_TIMING_ADVANCE, PID_INTAKE_TEMP, PID_THROTTLE, PID_FUEL_LEVEL};

      StaticJsonBuffer<200> jsonBuffer;
      JsonObject& json = jsonBuffer.createObject();

#if !DEBUG_MODE

      json["timeDelta"] = millis();
      
      for (byte i = 0; i < sizeof(pidlist) / sizeof(pidlist[0]); i++) {
          byte pid = pidlist[i];
          bool valid = obd.isValidPID(pid);
          
          String pidHex = (String) ((int)pid | 0x100, HEX);
          
          if (valid) {
              int value;
              if (obd.readPID(pid, value)) {
                json[pidHex] = value;
              }
          }
          else{
            json[pidHex] = "";
          }
       }
       
       json.printTo(BT);
       BT.println();
       deltatime = millis();
       delay(1000);
#endif
#if DEBUG_MODE
  String out = "";

  json["timeDelta"] = 4242;
  json["104"] = 104;
  json["105"] = 92;
  json["10C"] = 845;
  json["10D"] = 68;
  json["10E"] = 76;
  json["10F"] = 87;
  json["111"] = 80;
  json["12F"] = 42;

  json.printTo(BT);
  BT.println();
  delay(1000);

#endif
     
     
}

void processGPS()
{
    // process GPS data
    char c = GPS.read();
    if (!gps.encode(c))
        return;

    // parsed GPS data is ready
    uint32_t time;
    uint32_t date;

    

    /*gps.get_datetime(&date, &time, 0);
    if (date != gpsDate) {
        // log date only if it's changed and valid
        int year = date % 100;
        if (date < 1000000 && date >= 10000 && year >= 15 && (gpsDate == 0 || year - (gpsDate % 100) <= 1)) {
          gpsDate = date;
        }
    }*/

    gps.get_position(&lat, &lng, 0);

    BT.println("lat: " + lat);
    BT.println("lng: " + lng);

    // keep current data time as last GPS time
    //lastGPSDataTime = dataTime;

    /*// display UTC date/time
    lcd.setFlags(FLAG_PAD_ZERO);
    lcd.setCursor(216, 24);
    lcd.printLong(time, 8);

    // display latitude
    lcd.setCursor(216, 27);
    lcd.print((float)lat / 100000, 5);
    // display longitude
    lcd.setCursor(216, 30);
    lcd.print((float)lng / 100000, 5);
    lcd.setFlags(0);*/
    
}

