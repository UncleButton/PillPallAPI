// stepperMotor.ino
// Author: Justin Feldmann
// This is the code that will be downloaded to the arduino, used for communicating the the motor controls and various sensors.

const int MOTOR_STEPS_PER_REV = 200;  // 200 is number of steps per revolution for the Nema Stepper motor itself at full resolution.
const int MOTOR_RESOLUTION = 16;      // 16 is the Microstep that the motor driver is set to. Now a full revolution will take 200 * 16 steps.
const int LITTLE_GEAR_TEETH = 12;     // This is the number of teeth on the small gear attached to the motor.
const int BIG_GEAR_TEETH = 40;        // This is the number of teeth on the large gear underneath the plate of pill cartridges.
const float ACTUAL_STEPS_PER_REV = ((float)BIG_GEAR_TEETH/LITTLE_GEAR_TEETH) * MOTOR_STEPS_PER_REV * MOTOR_RESOLUTION;
const float STEPS_FOR_ONE_TWELFTH_REV = ACTUAL_STEPS_PER_REV / 12;
const int HALF_REV_LOCATION = ACTUAL_STEPS_PER_REV / 2;
const int stepPin = 5;
const int dirPin = 2;
const int enPin = 8;
const int cartridgePositions[] = {0, STEPS_FOR_ONE_TWELFTH_REV * 2, STEPS_FOR_ONE_TWELFTH_REV * 4, STEPS_FOR_ONE_TWELFTH_REV * 6, STEPS_FOR_ONE_TWELFTH_REV * 8, STEPS_FOR_ONE_TWELFTH_REV * 10};
int delayTime = 50;
char direction = 1; // 1 is counter clockwise, 0 is clockwise
boolean spun = false;
int currentPos = 0;
void setup()
{
  delay(2000);
  Serial.begin(9600);
  pinMode(stepPin, OUTPUT);
  pinMode(dirPin, OUTPUT);

  // for (int i = 0; i < 6; i++)
  // {
  //   Serial.print("Cartidge ");
  //   Serial.print(i);
  //   Serial.print(" at ");
  //   Serial.println(cartridgePositions[i]);
  // }

  //  Return to zero position here - METHOD AND CODE STILL NEEDED

  currentPos = 0;
}

boolean spinPlate(int nextLocation)
{
  int stepsToGo = nextLocation - currentPos; // 3555 - 7111 = 3556
  if (abs(stepsToGo) > HALF_REV_LOCATION)
  {
    // rotate counterclockwise
    if (stepsToGo < 0)
      stepsToGo += ACTUAL_STEPS_PER_REV;
    else
      stepsToGo -= ACTUAL_STEPS_PER_REV;
  }

  if (stepsToGo < 0)
  {
    // rotate counterclockwise
    stepsToGo *= -1;
    direction = 1;
    currentPos -= stepsToGo;
  }
  else
  {
    // rotate clockwise
    direction = 0;
    currentPos += stepsToGo;
  }

  Serial.println(stepsToGo);

  digitalWrite(dirPin, direction);
  for (int x = 0; x < stepsToGo; x++)
  {
    digitalWrite(stepPin, HIGH);
    delayMicroseconds(delayTime);
    digitalWrite(stepPin, LOW);
    delayMicroseconds(delayTime);
  }
  delay(1000);
  return true;
}

void loop()
{
  if (!spun)
  {
    spinPlate(cartridgePositions[2]);
    spun = true;
  }
}