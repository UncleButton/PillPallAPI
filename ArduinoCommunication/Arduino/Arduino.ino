// Arduino.ino

// This is the code that will go on the Arduino. It will take in the request from the ArduinoCommunicator and control the motors, vacuum, sensors,
// and other parts of the device according to the particular request.

// Justin Feldmann, February 2024

// The two-bit request needs to be shifted to bits 6 and 7 of the first byte in the message
const byte INFO_REQUEST_OFFSET = 6;
const byte EXPECTED_DISPENSE_LENGTH = 6;

// The request types, either refill (1) or dispense (2)
const byte REQUEST_REFILL = 1;
const byte REQUEST_DISPENSE = 2;

const byte RECEIVED_VALID_MESSAGE = 0;   // If the message was a success, we return a 0. Don't ask why it's a char, it just is
const byte RECEIVED_INVALID_MESSAGE = 1;      // If the message was a fail, we return a 1. Don't ask why it's a char, it just is
const byte HEARTBEAT = 2;         // Heartbeat, still communicating with Arduino correctly
const byte FINISHED_SUCCESS = 3;  // Finished dispensing or refilling correctly
const byte FINISHED_FAIL = 4;     // Did not successfully dispense or refill

//  Good data from ultrasonic sensor
const byte ULTRASONIC_SUCCESS = 3;
//  Bad data from ultrasonic sensor, read again
const byte ULTRASONIC_FAIL = -1;

//  The speed of sound is 343 m/s. This value is the same thing, just in terms of cm and microseconds, and dividing by 2 because the sensor reads the
//  time it takes for the ultrasonic signal to travel double the actual distance
const float ULTRASONIC_SPEED_CM_PER_US = 0.0343 / 2;
const float CM_FROM_ULTRASONIC_TO_CARTRIDGE = 7.577556;
const float MAX_DISTANCE = 7.577556;  // MAYBE???
const int MS_FROM_ULTRASONIC_TO_CARTRIDGE = 7300;
const float ACTUATOR_SPEED_CM_PER_SECOND = CM_FROM_ULTRASONIC_TO_CARTRIDGE / MS_FROM_ULTRASONIC_TO_CARTRIDGE;

// The states of the machine, starting with the initialize state
const byte STATE_CALIBRATE = 0;
const byte STATE_READ_DATA = 1;
const byte STATE_SELECT_CARTRIDGE = 2;
const byte STATE_ROTATE_REFILL = 3;
const byte STATE_REFILL_WAIT = 4;
const byte STATE_ROTATE_ULTRASONIC = 9;
const byte STATE_READ_ULTRASONIC = 10;
const byte STATE_ROTATE_TO_VACUUM = 11;
const byte STATE_LOWER_HOSE = 12;
const byte STATE_RAISE_HOSE = 13;
const byte STATE_DROP_PILL = 14;
const byte STATE_DONE = 20;

// Pins on the arduino and what they're physically connected to
const byte PIN_NEMA_DIRECTION = 3;
const byte PIN_NEMA_STEP = 4;
const byte PIN_VACUUM = 5;
const byte PIN_ACTUATOR_UP = 6;
const byte PIN_ACTUATOR_DOWN = 7;
const byte PIN_INFRARED = 12;
const byte PIN_ULTRASONIC_TRIG = 9;
const byte PIN_ULTRASONIC_ECHO = 10;
const byte PIN_LIGHT_SENSOR = 11;

// These are specifically in relation to operation of the NEMA Stepper Motor, which spins the base that the cartridges are on
const int DISPENSE_DELAY_TIME = 400;  //  Delay time needed between switching the nema on and off when dispensing
const int RESET_DELAY_TIME = 400;     //  Delay time needed for resetting the plate to 0
const byte NEMA_CLOCKWISE = 0;
const byte NEMA_COUNTER_CLOCKWISE = 1;
const int LITTLE_GEAR_TEETH = 12;
const int BIG_GEAR_TEETH = 40;
const int NEMA_STEPS_PER_REV = 200;     // Number of steps per revolution for the Nema Stepper motor itself at full resolution
const int NEMA_DRIVER_RESOLUTION = 16;  // Microstep that the motor driver is set to. Now a full revolution will take 200 * 16 steps
// NEMA_ACTUAL_STEPS_PER_REV is the steps per rev when taking into consideration the gear ratios and the driver resolution
const float NEMA_ACTUAL_STEPS_PER_REV = ((float)BIG_GEAR_TEETH / LITTLE_GEAR_TEETH) * NEMA_STEPS_PER_REV * NEMA_DRIVER_RESOLUTION;
const float HALF_DISTANCE_BETWEEN_CARTRIDGES = NEMA_ACTUAL_STEPS_PER_REV / (EXPECTED_DISPENSE_LENGTH * 2);
const float DISTANCE_BETWEEN_CARTRIDGES = HALF_DISTANCE_BETWEEN_CARTRIDGES * 2;
const int NEMA_HALF_REV_LOCATION = NEMA_ACTUAL_STEPS_PER_REV / 2;
const int CARTRIDGE_LOCATIONS[] = { 0, DISTANCE_BETWEEN_CARTRIDGES, DISTANCE_BETWEEN_CARTRIDGES * 2, DISTANCE_BETWEEN_CARTRIDGES * 3, DISTANCE_BETWEEN_CARTRIDGES * 4, DISTANCE_BETWEEN_CARTRIDGES * 5 };
const int REFILL_LOCATIONS[] = { CARTRIDGE_LOCATIONS[4] };
const int REFILL_LOCATION = HALF_DISTANCE_BETWEEN_CARTRIDGES * 4;
const int ULTRASONIC_SENSOR_LOCATION = NEMA_ACTUAL_STEPS_PER_REV - HALF_DISTANCE_BETWEEN_CARTRIDGES;

byte *data;                           // The array that will hold the data, allocated when it is read in
byte request = 0;                     // The type of request received fromt the pi
byte cartridges = 0;                  // The number of cartridges, used to allocate the data array
byte currentState = STATE_READ_DATA;  // Current state of the machine, cannot be modified outside of "loop." Start at read data state
byte nextState = STATE_READ_DATA;     // The next state in the machine, CAN be modified outside of "loop"

byte nemaDirection = 0;  // 1 is counter clockwise, 0 is clockwise
int nemaCurrentPos = 0;  // Current position of the base plate

byte currentCartridge = 0;  // The cartridge of medication currently being dispensed

byte ultrasonicAttempts = 0;
float ultrasonicMicroTime = 0;  //  Time it took for the ultrasonic sensor to receieve the echo back
float distanceInCm = 0;         //  The distance calculated from the time it took to receieve the echo
int timeToMoveActuator = 0;     // Time in milliseconds for the hose to be lowered into the cartridge

// Setup function, runs once when the Arduino first powers on, configuring pins and serial communication
void setup() {
  delay(3000);         // The delay is needed here, otherwise the Arduino starts running code early
  Serial.begin(9600);  // Open the serial port for communication

  //  Set these pins to output so we can write signals to them
  pinMode(PIN_NEMA_DIRECTION, OUTPUT);
  pinMode(PIN_NEMA_STEP, OUTPUT);
  pinMode(PIN_ULTRASONIC_TRIG, OUTPUT);
  pinMode(PIN_VACUUM, OUTPUT);
  pinMode(PIN_ACTUATOR_UP, OUTPUT);
  pinMode(PIN_ACTUATOR_DOWN, OUTPUT);

  //  Set these pins to input so we can read signals from them
  pinMode(PIN_INFRARED, INPUT);
  pinMode(PIN_ULTRASONIC_ECHO, INPUT);

  //  Write LOW to pins so they start off
  // digitalWrite(PIN_ACTUATOR_DOWN, LOW);
  digitalWrite(PIN_ACTUATOR_UP, HIGH);
  delay(3000);
  digitalWrite(PIN_ACTUATOR_UP, LOW);
  // digitalWrite(PIN_ACTUATOR_DOWN, HIGH);
  // delay(700);
  // digitalWrite(PIN_ACTUATOR_DOWN, LOW);
}

//  This reads in the data coming in on the serial port. It will determine what type of request is being made, allocate the appropriate amount of
//  space in memory to store the data, and determine the next state in the state machine. If the data is valid, it returns a "valid" indication,
//  otherwise it returns "invalid."
byte readData() {
  //  Read in the information byte, and split it into the request and cartridges
  byte info = 0;
  Serial.readBytes(&info, 1);
  request = (info & 0b11000000) >> INFO_REQUEST_OFFSET;
  cartridges = info & 0b00111111;

  // Need to add the info byte and data bytes to check against the checksum
  byte sum = info;

  // Allocate <cartridges> bytes of memory for the data, then read the data in and sum it
  data = malloc(cartridges);
  Serial.readBytes(data, cartridges);
  for (int i = 0; i < cartridges; i++)
    sum += data[i];

  // Read in the checkSum and see if it matches the sum of the data given
  byte checkSum = 0;
  Serial.readBytes(&checkSum, 1);
  //  As long as the check sums match AND the request is either a valid dispense request or a refill request
  if (sum == checkSum && (request == REQUEST_DISPENSE && cartridges == EXPECTED_DISPENSE_LENGTH || request == REQUEST_REFILL)) {
    //  Set the next state to be selecting the cartridge
    nextState = STATE_SELECT_CARTRIDGE;
    return RECEIVED_VALID_MESSAGE;
  } else {  //  The checksums did not match so the message is invalid
    return RECEIVED_INVALID_MESSAGE;
  }
}

//  This rotates the base plate until cartridge 0 is underneath the vacuum hose. A strip of tape has been placed on the side of the base plate, and
//  there is an infrared sensor that will detect this strip of tape. The base will keep spinning until the sensor detects the tape, and then stops
//  spinning. The sensor and tape have been placed such that, when it stops, cartridge 0 will be in the correct place.
void resetPlate() {
  int count = 0;
  while (digitalRead(PIN_INFRARED) && count <= NEMA_ACTUAL_STEPS_PER_REV) {
    digitalWrite(PIN_NEMA_STEP, HIGH);
    delayMicroseconds(RESET_DELAY_TIME);
    digitalWrite(PIN_NEMA_STEP, LOW);
    delayMicroseconds(RESET_DELAY_TIME);
    count++;
  }
  if (count >= NEMA_ACTUAL_STEPS_PER_REV) {
    Serial.print(FINISHED_FAIL);
    nextState = STATE_READ_DATA;
  }
  // for (int x = 0; x < 350; x++) {
  //   digitalWrite(PIN_NEMA_STEP, HIGH);
  //   delayMicroseconds(RESET_DELAY_TIME);
  //   digitalWrite(PIN_NEMA_STEP, LOW);
  //   delayMicroseconds(RESET_DELAY_TIME);
  // }
  currentCartridge = 0;
}

//  This takes in whatever the next location of the plate needs to be rotated to, for it to be under the vacuum. It looks at its last known location
//  and calculates how much it needs to rotate to get that location under the vacuum. It then figures out which is faster, rotating clockwise or
//  counter clockwise, and then finally actually rotates the plate accordingly.
void spinPlate(int nextLocation) {
  int stepsToGo = nextLocation - nemaCurrentPos;

  //  If the rotate is more than half the total steps possible, we can just spin the plate in the opposite direction
  if (abs(stepsToGo) > NEMA_HALF_REV_LOCATION) {
    //  If stepsToGo was a negative value, it's a negative value above the half. If total steps was 2000, then half is 1000. So if stepsToGo was
    //  -1500, rather than rotating counter clockwise -1500 steps, we can just go clockwise 500 steps. Adding the total steps fixes this
    if (stepsToGo < 0)
      stepsToGo += NEMA_ACTUAL_STEPS_PER_REV;
    //  Similarly, if stepsToGo clockwise is 1500, just subtract 2000 so now we simply rotate -500 steps counter clockwise
    else
      stepsToGo -= NEMA_ACTUAL_STEPS_PER_REV;
  }

  //  If stepsToGo is negative, we go counter clockwise. For the rotation further below, stepsToGo needs to be positive. We set the direction pin
  //  to NEMA_COUNTER_CLOCKWISE (1), and also update the currentPos tracker for after the rotate
  if (stepsToGo < 0) {
    stepsToGo *= -1;
    nemaDirection = NEMA_COUNTER_CLOCKWISE;
    nemaCurrentPos -= stepsToGo;
  } else  //  If it's positive, then set the direction to NEMA_CLOCKWISE (0) and update the currentPos tracker
  {
    nemaDirection = NEMA_CLOCKWISE;
    nemaCurrentPos += stepsToGo;
  }

  //  Set the direction pin and then start the rotate. The motor works by rotating a step every time it senses a high transition, so in every
  //  iteration of the loop we start by sending a HIGH (1) signal to get it to step, then send LOW (0) so that it's ready for the next step.
  //  The "DISPENSE_DELAY_TIME" is needed so there is some delay between the pin getting the high and low signal. This also allows us to control
  //  the speed at which the plate actually spins.
  digitalWrite(PIN_NEMA_DIRECTION, nemaDirection);
  for (int x = 0; x < stepsToGo; x++) {
    digitalWrite(PIN_NEMA_STEP, HIGH);
    delayMicroseconds(DISPENSE_DELAY_TIME);
    digitalWrite(PIN_NEMA_STEP, LOW);
    delayMicroseconds(DISPENSE_DELAY_TIME);
  }
  return;
}

void selectCartridge() {
  //  Loop through the data array to find the next cartridge/medication that needs to be dispensed
  while (currentCartridge < cartridges && data[currentCartridge] == 0)
    currentCartridge++;
  if (currentCartridge < cartridges)
    // Determine the next state based off which type of request was received
    nextState = request == REQUEST_REFILL ? STATE_ROTATE_REFILL : STATE_ROTATE_ULTRASONIC;
  else
    nextState = STATE_DONE;
}

void readUltrasonic() {
  int ultrasonicTimes[5];
  for (int i = 0; i < 5; i++) {
    digitalWrite(PIN_ULTRASONIC_TRIG, LOW);
    delayMicroseconds(2);
    digitalWrite(PIN_ULTRASONIC_TRIG, HIGH);
    delayMicroseconds(10);
    digitalWrite(PIN_ULTRASONIC_TRIG, LOW);
    ultrasonicTimes[i] = pulseIn(PIN_ULTRASONIC_ECHO, HIGH);
    delay(50);
  }
  int sum = 0;
  for (int i = 0; i < 5; i++)
    sum += ultrasonicTimes[i];

  ultrasonicMicroTime = sum / 5;
  distanceInCm = ultrasonicMicroTime * ULTRASONIC_SPEED_CM_PER_US;
  if (distanceInCm < MAX_DISTANCE)
    nextState = STATE_ROTATE_TO_VACUUM;
  // Serial.println(ultrasonicMicroTime);
  // Serial.print(" ");
}

void lowerHose() {
  digitalWrite(PIN_ACTUATOR_DOWN, HIGH);
  delay(timeToMoveActuator);
  digitalWrite(PIN_ACTUATOR_DOWN, LOW);
  nextState = STATE_RAISE_HOSE;
}

void raiseHose() {
  digitalWrite(PIN_VACUUM, HIGH);
  digitalWrite(PIN_ACTUATOR_UP, HIGH);
  delay(timeToMoveActuator);
  digitalWrite(PIN_ACTUATOR_UP, LOW);
  nextState = STATE_DROP_PILL;
}

void dropPill() {
  spinPlate(CARTRIDGE_LOCATIONS[currentCartridge] + HALF_DISTANCE_BETWEEN_CARTRIDGES);
  digitalWrite(PIN_VACUUM, LOW);
  delay(5000);
  data[currentCartridge]--;
  if (data[currentCartridge] == 0)
    nextState = STATE_SELECT_CARTRIDGE;
  else
    nextState = STATE_ROTATE_TO_VACUUM;
}

// Arduino runs in an infinite loop through this function, which is where the main state machine of this device will be
void loop() {
  switch (currentState) {
    case STATE_CALIBRATE:
      {
        float times = 0;
        for (int i = 0; i < 50; i++) {
          readUltrasonic();
          // if (maxTime < ultrasonicMicroTime)
          //     maxTime = ultrasonicMicroTime;
          times += ultrasonicMicroTime;
          // Serial.println(ultrasonicMicroTime);
        }
        Serial.print("avg time is: ");
        Serial.println(times / 50);
        // timeToMoveActuator = MAX_DISTANCE / ACTUATOR_SPEED_CM_PER_SECOND;
        // lowerHose();
        // delay(2000);
        // raiseHose();
        nextState = STATE_DONE;
        break;
      }
    // In the read data state, we wait until there is data available on the serial port and then try to read it on
    case STATE_READ_DATA:
      {
        if (Serial.available()) {
          //  Read the data, if it was a success then nextState will have changed to either refill or dispense, otherwise it is still the
          //  read data state and we stay in read data
          byte success = readData();

          //  Send response back to pi, letting it know whether the data we got was valid
          Serial.print(success);

          //  Reset the plate so that cartidge 0 is underneath the vacuum hose
          if (digitalRead(PIN_INFRARED))
            resetPlate();
          // spinPlate(NEMA_HALF_REV_LOCATION);
        }
        break;
      }
    case STATE_SELECT_CARTRIDGE:
      {
        selectCartridge();
        break;
      }
    //  This is the beginning of the refill
    case STATE_ROTATE_REFILL:
      {
        spinPlate(CARTRIDGE_LOCATIONS[currentCartridge] + 5);
        nextState = STATE_DONE;
        break;
      }
    //  Rotate to where ultrasonic sensor is and read in data
    case STATE_ROTATE_ULTRASONIC:
      {
        spinPlate(CARTRIDGE_LOCATIONS[currentCartridge] + (DISTANCE_BETWEEN_CARTRIDGES * 1.1));
        nextState = STATE_READ_ULTRASONIC;
        break;
      }
    case STATE_READ_ULTRASONIC:
      {
        readUltrasonic();
        if (ultrasonicAttempts > 2)
        {
          Serial.print(FINISHED_FAIL);
          nextState = STATE_READ_DATA;
          ultrasonicAttempts = 0;
          break;
        }
        ultrasonicAttempts++;
        break;
      }
    case STATE_ROTATE_TO_VACUUM:
      {
        spinPlate(CARTRIDGE_LOCATIONS[currentCartridge]);
        timeToMoveActuator = distanceInCm / ACTUATOR_SPEED_CM_PER_SECOND;
        // Serial.print(ultrasonicMicroTime);
        // Serial.print(" ");
        // Serial.print(distanceInCm);
        // Serial.print(" ");
        nextState = STATE_LOWER_HOSE;
        break;
      }
    case STATE_LOWER_HOSE:
      {
        lowerHose();
        // Serial.println(timeToMoveActuator);
        break;
      }
    case STATE_RAISE_HOSE:
      {
        raiseHose();
        break;
      }
    case STATE_DROP_PILL:
      {
        dropPill();
        //  Reset the plate so that cartidge 0 is underneath the vacuum hose
        if (currentCartridge >= cartridges and digitalRead(PIN_INFRARED))
          resetPlate();
        break;
      }
    case STATE_DONE:
      {
        Serial.print(FINISHED_SUCCESS);
        nextState = STATE_READ_DATA;
        break;
      }
  }
  currentState = nextState;
  if (currentState != STATE_READ_DATA && currentState != STATE_DONE && currentState != STATE_READ_ULTRASONIC)
    Serial.print(HEARTBEAT);
}
