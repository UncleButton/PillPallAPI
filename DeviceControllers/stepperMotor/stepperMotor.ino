// stepperMotor.ino
// Author: Justin Feldmann
// This is the code that will be downloaded to the arduino, used for communicating the the motor controls and various sensors.

const int SPINNER_MOTOR_STEPS_PER_REV = 200;  // 200 is number of steps per revolution for the Nema Stepper motor itself at full resolution.
const int SPINNER_MOTOR_RESOLUTION = 16;      // 16 is the Microstep that the motor driver is set to. Now a full revolution will take 200 * 16 steps.
const int LITTLE_GEAR_TEETH = 12;   // This is the number of teeth on the small gear attached to the motor.
const int BIG_GEAR_TEETH = 40;      // This is the number of teeth on the large gear underneath the plate of pill cartridges.
const float SPINNER_ACTUAL_STEPS_PER_REV = ((float) BIG_GEAR_TEETH/LITTLE_GEAR_TEETH) * SPINNER_MOTOR_STEPS_PER_REV * SPINNER_MOTOR_RESOLUTION;
const float SPINNER_STEPS_FOR_ONE_TWELFTH_REV = SPINNER_ACTUAL_STEPS_PER_REV / 12;
const int SPINNER_HALF_REV_LOCATION = SPINNER_ACTUAL_STEPS_PER_REV / 2;
const int spinStepPin = 5;
const int spinDirPin = 2;
// const int enPin = 8; // not needed?
int spinDelayTime = 50;
const int cartridgePositions[] = {0, SPINNER_STEPS_FOR_ONE_TWELFTH_REV * 2, SPINNER_STEPS_FOR_ONE_TWELFTH_REV * 4, SPINNER_STEPS_FOR_ONE_TWELFTH_REV * 6, SPINNER_STEPS_FOR_ONE_TWELFTH_REV * 8, SPINNER_STEPS_FOR_ONE_TWELFTH_REV * 10};
char spinnerDirection = 1; // 1 is counter clockwise, 0 is clockwise
int spinnerCurrentPos = 0;
boolean spun = false;

void setup()
{
  delay(2000);
  Serial.begin(9600);
  pinMode(spinStepPin, OUTPUT);
  pinMode(spinDirPin, OUTPUT);

  // for (int i = 0; i < 6; i++)
  // {
  //   Serial.print("Cartidge ");
  //   Serial.print(i);
  //   Serial.print(" at ");
  //   Serial.println(cartridgePositions[i]);
  // }

  //  Return to zero position here - METHOD AND CODE STILL NEEDED

  spinnerCurrentPos = 0;
}

boolean spinPlate(int nextLocation)
{
  int stepsToGo = nextLocation - spinnerCurrentPos;
  if (abs(stepsToGo) > SPINNER_HALF_REV_LOCATION)
  {
    // rotate counterclockwise
    if (stepsToGo < 0)
      stepsToGo += SPINNER_ACTUAL_STEPS_PER_REV;
    else
      stepsToGo -= SPINNER_ACTUAL_STEPS_PER_REV;
  }

  if (stepsToGo < 0)
  {
    // rotate counterclockwise
    stepsToGo *= -1;
    spinnerDirection = 1;
    spinnerCurrentPos -= stepsToGo;
  }
  else
  {
    // rotate clockwise
    spinnerDirection = 0;
    spinnerCurrentPos += stepsToGo;
  }

  Serial.println(stepsToGo);

  digitalWrite(spinDirPin, spinnerDirection);
  for (int x = 0; x < stepsToGo; x++)
  {
    digitalWrite(spinStepPin, HIGH);
    delayMicroseconds(spinDelayTime);
    digitalWrite(spinStepPin, LOW);
    delayMicroseconds(spinDelayTime);
  }
  delay(1000);
  return true;
}

void loop()
{
  if (!spun)
  {
    spinPlate(cartridgePositions[2]);
    spinPlate(cartridgePositions[0]);
    spun = true;
  }
}