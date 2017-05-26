bool hasMEMS;
int deltatime = 0;

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
#if !DEBUG_MODE
      String out = "";

      out += '{';
      
      out += "\"timeDelta\": ";
      

      out += (String) millis();

      
      out += (", ");
      
      for (byte i = 0; i < sizeof(pidlist) / sizeof(pidlist[0]); i++) {
          byte pid = pidlist[i];
          bool valid = obd.isValidPID(pid);
          
          out += '"';
          out += ((int)pid | 0x100, HEX);
          out += "\": ";
          
          if (valid) {
              int value;
              if (obd.readPID(pid, value)) {
                out += '"';
                out += (String)value;
                out += '"';
              }
          }
          else{
            out += "\"\"";
          }
          
          if((i + 1) < sizeof(pidlist) / sizeof(pidlist[0])){
            out += ", ";
          }
       }
       out += '}';
       BT.println(out);
       deltatime = millis();
       delay(1000);
#endif
#if DEBUG_MODE
  String out = "";

  out += '{';
  out += "\"timeDelta\": ";
  out += "\"4242\"";
  out += ", ";
  
  out += "\"104\": ";
  out += "\"104\"";
  out += ", ";

  out += "\"105\": ";
  out += "\"92\"";
  out += ", ";
  
  out += "\"10C\": ";
  out += "\"845\"";
  out += ", ";

  out += "\"10D\": ";
  out += "\"68\"";
  out += ", ";

  out += "\"10E\": ";
  out += "\"76\"";
  out += ", ";

  out += "\"10F\": ";
  out += "\"87\"";
  out += ", ";

  out += "\"111\": ";
  out += "\"80\"";
  out += ", ";

  out += "\"12F\": ";
  out += "\"42\"";

  out += "}";
  BT.println(out);
  delay(1000);

#endif
     
     
}

/*void processGPS()
{
    // process GPS data
    char c = GPS.read();
    if (!gps.encode(c))
        return;

    // parsed GPS data is ready
    uint32_t time;
    uint32_t date;

    

    gps.get_datetime(&date, &time, 0);
    if (date != gpsDate) {
        // log date only if it's changed and valid
        int year = date % 100;
        if (date < 1000000 && date >= 10000 && year >= 15 && (gpsDate == 0 || year - (gpsDate % 100) <= 1)) {
          logger.logData(PID_GPS_DATE, (int32_t)date);
          gpsDate = date;
        }
    }
    logger.logData(PID_GPS_TIME, (int32_t)time);

    int32_t lat, lng;
    gps.get_position(&lat, &lng, 0);

    byte sat = gps.satellites();

    // show GPS data interval
    lcd.setFontSize(FONT_SIZE_MEDIUM);
    if (lastGPSDataTime) {
        lcd.setCursor(380, 31);
        lcd.printInt((uint16_t)logger.dataTime - lastGPSDataTime);
        lcd.print("ms");
        lcd.printSpace(2);
    }

    // keep current data time as last GPS time
    lastGPSDataTime = logger.dataTime;

    // display UTC date/time
    lcd.setFlags(FLAG_PAD_ZERO);
    lcd.setCursor(216, 24);
    lcd.printLong(time, 8);

    // display latitude
    lcd.setCursor(216, 27);
    lcd.print((float)lat / 100000, 5);
    // display longitude
    lcd.setCursor(216, 30);
    lcd.print((float)lng / 100000, 5);
    // log latitude/longitude
    logger.logData(PID_GPS_LATITUDE, lat);
    logger.logData(PID_GPS_LONGITUDE, lng);

    // display altitude
    int32_t alt = gps.altitude();
    lcd.setFlags(0);
    if (alt > -1000000 && alt < 1000000) {
        lcd.setCursor(216, 33);
        lcd.print(alt / 100);
        lcd.print("m ");
    }
    // log altitude
    logger.logData(PID_GPS_ALTITUDE, (int)(alt / 100));

    // display number of satellites
    if (sat < 100) {
        lcd.setCursor(216, 36);
        lcd.printInt(sat);
        lcd.write(' ');
    }

    // only log these data when satellite status is good
    if (sat >= 3) {
        gpsSpeed = gps.speed() * 1852 / 100000;
        logger.logData(PID_GPS_SPEED, gpsSpeed);
    }
}
*/

