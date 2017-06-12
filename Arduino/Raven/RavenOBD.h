bool hasMEMS;
int deltaTime = 0;
int writeInterval = 2000;
   
uint32_t time;
uint32_t date;
int32_t lat, lng;
int32_t rpm,_speed;
unsigned long chars;
unsigned short sentences, failed_checksum;

static const byte pidlist[] = {PID_RPM, PID_SPEED};
static const String pidNames[] = {"Rpm", "Speed"};

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
        delay(250);
    }
    lcd.println();
    delay(2500);
    lcd.clear();
}

void readPIDs(){
  int timeDiff = millis() - deltaTime;
  if (timeDiff > writeInterval){
    StaticJsonBuffer<120> jsonBuffer;
    JsonObject& json = jsonBuffer.createObject();
    
    json["DeltaTime"] = timeDiff - writeInterval;

#if !DEBUG_MODE

    for (byte i = 0; i < sizeof(pidlist) / sizeof(pidlist[0]); i++) {
        byte pid = pidlist[i];
        bool valid = obd.isValidPID(pid);
        
        if (valid) {
            int value;
            if (obd.readPID(pid, value)) {
              json[pidNames[i]] = value;
            }
        }else{
          json[pidNames[i]] = "";  
        }
     }
  
     json["Lat"] = ((float)lat / 100000);
     json["Lng"] = ((float)lng / 100000);
     json["Date"] = date;
     json["Time"] = time;

     rpm = json["Rpm"];
     _speed = json["Speed"];
#endif
#if DEBUG_MODE

    json["Rpm"] = 800;
    json["Speed"] = 20;
    json["Lat"] = ((float)lat / 100000);
    json["Lng"] = ((float)lng / 100000);
    json["Date"] = date;
    json["Time"] = time; 
  
#endif

    deltaTime = millis();
    json.printTo(BT);
    BT.println("");
  }
}

void processGPS(){
   while (GPS.available()){
      char c = GPS.read();
      
      if (gps.encode(c)){
        // retrieves +/- lat/long in 100000ths of a degree
        gps.get_position(&lat, &lng, 0);
           
        // time in hhmmsscc, date in ddmmyy
        gps.get_datetime(&date, &time, 0);
      }

      #if DEBUG_MODE
        _speed = _speed + 1;
      #endif
   }
}
