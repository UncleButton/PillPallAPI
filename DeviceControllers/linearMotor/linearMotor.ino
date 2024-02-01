// linearMotor.ino
// Author: Justin Feldmann
// This is the code for the linear actuator motor.
//  ==============================  THIS NEEDS TO BE COMBINED WITH THE OTHER MOTOR FILE ==============================

const int LINEAR_STEP_PIN = 5;
const int LINEAR_DIR_PIN = 2;
int linearDelayTime = 50; // Delay time needed between switching the stepper on and off, also helps in controlling the speed of the stepper.
char linearDirection = 0; // 0 is up away from the cartiridge, 1 is down into the cartridge.

void setup()
{
  delay(2000);
  Serial.begin(9600);
  pinMode(LINEAR_STEP_PIN, OUTPUT);
  pinMode(LINEAR_DIR_PIN, OUTPUT);
  digitalWrite(LINEAR_DIR_PIN, linearDirection);
}

void loop()
{
  // if (!spun)
  // {
    for (unsigned int x = 0; x < 52000; x++)
    { 
      digitalWrite(LINEAR_STEP_PIN, HIGH);
      delayMicroseconds(linearDelayTime);
      digitalWrite(LINEAR_STEP_PIN, LOW);
      delayMicroseconds(linearDelayTime);
      if (x >= 51999)
      {
        x = 0;
        digitalWrite(LINEAR_DIR_PIN, !linearDirection);
        linearDirection = linearDirection != 1;
      }
    }
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
