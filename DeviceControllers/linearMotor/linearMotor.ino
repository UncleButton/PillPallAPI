// linearMotor.ino
// Author: Justin Feldmann
// This is the code for the linear actuator motor.
//  ==============================  THIS NEEDS TO BE COMBINED WITH THE OTHER MOTOR FILE ==============================

const int LINEAR_STEP_PIN = 5;
const int LINEAR_DIR_PIN = 2;
const int VACUUM_PIN = 7;
int linearDelayTime = 50; // Delay time needed between switching the stepper on and off, also helps in controlling the speed of the stepper.
boolean linearDirection = 0; // 0 is up away from the cartiridge, 1 is down into the cartridge.

void setup()
{
  delay(2000);
  Serial.begin(9600);
  pinMode(LINEAR_STEP_PIN, OUTPUT);
  pinMode(LINEAR_DIR_PIN, OUTPUT);
  pinMode(VACUUM_PIN, OUTPUT);
  digitalWrite(VACUUM_PIN, LOW);
  digitalWrite(LINEAR_DIR_PIN, linearDirection);
}

void loop()
{
  // if (!spun)
  // {
    // for (unsigned int x = 0; x < 50000; x++)
    // { 
    //   digitalWrite(LINEAR_STEP_PIN, HIGH);
    //   delayMicroseconds(linearDelayTime);
    //   digitalWrite(LINEAR_STEP_PIN, LOW);
    //   delayMicroseconds(linearDelayTime);
    //   if (x >= 49999)
    //   {
    //     x = 0;
    //     digitalWrite(LINEAR_DIR_PIN, !linearDirection);
    //     linearDirection = linearDirection != 1;
    //   }
    // }
    Serial.println("Done with linear actuator");
    digitalWrite(VACUUM_PIN, HIGH);
    Serial.println("Vacuum is on");
    delay(3000);
    digitalWrite(VACUUM_PIN, LOW);
    Serial.println("Vacuum is off");
    delay(3000);
    // delay(1000);
    // Serial.print("Got here");
    // digitalWrite(dirPin, LOW);
    // for (unsigned int x = 0; x < 30000; x++)
    // {
    //   digitalWrite(stepPin, HIGH);
    //   delayMicroseconds(50);
    //   digitalWrite(stepPin, LOW);
    //   delayMicroseconds(50);
    //   delay(1000);
    // }
    // spun = true;
  // }
}
