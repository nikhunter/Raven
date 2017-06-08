bool hasMEMS;
int deltaTime = 0;

long lat, lng;
int rpm,_speed;
unsigned long fix_age, time, date, speed, course;
unsigned long chars;
unsigned short sentences, failed_checksum;

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

void readPIDs()
{
    //static const byte pidlist[] = {0x0C, 0x0D};

      StaticJsonBuffer<200> jsonBuffer;
      JsonObject& json = jsonBuffer.createObject();

#if !DEBUG_MODE

      json["DeltaTime"] = millis();

      static const byte pidlist[] = {PID_RPM, PID_SPEED};

      static const String pidNames[] = {"Rpm", "Speed"};
      
      for (byte i = 0; i < sizeof(pidlist) / sizeof(pidlist[0]); i++) {
          byte pid = pidlist[i];
          bool valid = obd.isValidPID(pid);
          
          if (valid) {
              int value;
              if (obd.readPID(pid, value)) {
                //BT.print(value);
                
                //String pidHex = (String) ((int)pid | 0x100, HEX); 
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

  json["timeDelta"] = 4242;
  json["10C"] = 845;
  json["10D"] = 68;
  json["date"] = date;
  json["time"] = time; 
  json["lat"] = ((float)lat / 100000);
  json["lng"] = ((float)lng / 100000);
  
#endif

  json.printTo(BT);
  BT.println("");
  delay(500);
  deltaTime = millis();
}

void processGPS(){
   while (GPS.available()){
      int c = GPS.read();
      if (gps.encode(c)){         
        // retrieves +/- lat/long in 100000ths of a degree
        gps.get_position(&lat, &lng, &fix_age);
         
        // time in hhmmsscc, date in ddmmyy
        gps.get_datetime(&date, &time, &fix_age);
         
        // returns speed in 100ths of a knot
        speed = gps.speed();
         
        // course in 100ths of a degree
        course = gps.course();
        
      }
   }
}

