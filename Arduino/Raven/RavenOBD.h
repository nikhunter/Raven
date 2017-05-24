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

      BT.print('{');
      
      BT.print("\"TimeDelta\": ");
      
      BT.print('"');
      BT.print(millis());
      BT.print('"');
      
      BT.print(", ");
      
      for (byte i = 0; i < sizeof(pidlist) / sizeof(pidlist[0]); i++) {
          byte pid = pidlist[i];
          bool valid = obd.isValidPID(pid);
          BT.print('"');
          BT.print((int)pid | 0x100, HEX);
          BT.print("\": ");
          if (valid) {
              int value;
              if (obd.readPID(pid, value)) {
                BT.print('"'+value+'"');
              }
          }
          else{
            BT.print("\"\"");
          }
          
          if((i + 1) < sizeof(pidlist) / sizeof(pidlist[0])){
            BT.print(", ");
          }
       }
       BT.print('}');
       BT.println();
       deltatime = millis();
       delay(1000);
     
     
}

