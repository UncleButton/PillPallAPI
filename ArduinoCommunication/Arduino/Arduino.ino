// Arduino.ino

// This is the code that will go on the Arduino. It will take in the request from the ArduinoCommunicator and control the motors, vacuum, sensors,
// and other parts of the device according to the particular request.

// Justin Feldmann, February 2024

// The two-bit request needs to be shifted to bits 6 and 7 of the first byte in the message
const byte INFO_REQUEST_OFFSET = 6;

// The request types, either refill (1) or dispense (2)
const byte REQUEST_REFILL = 1;
const byte REQUEST_DISPENSE = 2;

// Valid and invalid message types, to be sent back to the pi
const byte RECEIVED_VALID_MESSAGE = 0;
const byte RECEIVED_INVALID_MESSAGE = -1;

// The states of the machine, starting with the initialize state
const byte STATE_INITIALIZE = 0;
const byte STATE_READ_DATA = 1;
const byte STATE_DETERMINE_REQUEST = 2;
const byte STATE_REFILL = 3;
const byte STATE_DISPENSE = 4;

// Pins on the arduino and what they're physically connected to
const int PIN_NEMA_DIRECTION = 3;
const int PIN_NEMA_STEP = 4;
const int PIN_LIGHT_SENSOR = 5;
const int PIN_VACUUM = 6;
const int PIN_FORWARDS = 7;
const int PIN_BACKWARDS = 8;

// These are specifically in relation to operation of the NEMA Stepper Motor, which spins the base that the cartridges are on
const byte NEMA_CLOCKWISE = 0;
const byte NEMA_COUNTER_CLOCKWISE = 1;
const int LITTLE_GEAR_TEETH = 12;
const int BIG_GEAR_TEETH = 40;
const int NEMA_STEPS_PER_REV = 200;     // Number of steps per revolution for the Nema Stepper motor itself at full resolution
const int NEMA_DRIVER_RESOLUTION = 16;  // Microstep that the motor driver is set to. Now a full revolution will take 200 * 16 steps
// NEMA_ACTUAL_STEPS_PER_REV is the steps per rev when taking into consideration the gear ratios and the driver resolution
const float NEMA_ACTUAL_STEPS_PER_REV = ((float) BIG_GEAR_TEETH/LITTLE_GEAR_TEETH) * NEMA_STEPS_PER_REV * NEMA_DRIVER_RESOLUTION;
const int NEMA_HALF_REV_LOCATION = NEMA_ACTUAL_STEPS_PER_REV / 2;

byte *data;             // The array that will hold the data, allocated when it is read in
byte request = 0;       // The type of request received fromt the pi
byte dataLength = 0;    // The length of the data in bytes, used to allocate the data array
byte currentState = STATE_INITIALIZE;   // Set current state to initializing, cannot be modified outside of "loop"
byte nextState = STATE_READ_DATA;       // The next state in the machine, can be modified outside of "loop"

byte nemaDelayTime = 50;    // Delay time needed between switching the nema on and off to spin
byte nemaDirection = 1;     // 1 is counter clockwise, 0 is clockwise
int nemaCurrentPos = 0;     // Current position of the base plate

// Setup function, runs once when the Arduino first powers on, configuring pins and serial communication
void setup()
{
    delay(3000);    // The delay is needed here, otherwise the Arduino starts running code early
    Serial.begin(9600); // Open the serial port for communication

    // Set these pins to output so we can write signals to them
    pinMode(PIN_NEMA_DIRECTION, OUTPUT);
    pinMode(PIN_NEMA_STEP, OUTPUT);
}

//  This reads in the data coming in on the serial port. It will determine what type of request is being made, allocate the appropriate amount of
//  space in memory to store the data, and determine the next state in the state machine. If the data is valid, it returns a "valid" indication,
//  otherwise it returns "invalid."
byte readData()
{
    //  Read in the information byte, and split it into the request and dataLength
    byte info = 0;
    Serial.readBytes(&info, 1);
    request = (info & 0b11000000) >> INFO_REQUEST_OFFSET;
    dataLength = info & 0b00111111;

    // Need to add the info byte and data bytes to check against the checksum
    byte sum = info;

    // Allocate <dataLength> bytes of memory for the data, then read the data in and sum it
    data = malloc(dataLength);
    Serial.readBytes(data, dataLength);
    for (int i = 0; i < dataLength; i++)
        sum += data[i];
    
    // Read in the checkSum and see if it matches the sum of the data given
    byte checkSum = 0;
    Serial.readBytes(&checkSum, 1);
    byte isValid = RECEIVED_VALID_MESSAGE;
    if (sum == checkSum)
    {
        // Determine the next state based off which type of request was received
        if (request == REQUEST_REFILL)  //  If refill request, next state is refill
            nextState = STATE_REFILL;
        else if (request == REQUEST_DISPENSE)   // If dispense request, next state is dispense
            nextState = STATE_DISPENSE;
        else
            isValid = RECEIVED_INVALID_MESSAGE; //  Otherwise, the request is not recognized
    }
    else
    {   //  The checksums did not match so the message is invalid
        isValid = RECEIVED_INVALID_MESSAGE;
    }
    return isValid;
}

//  This takes in whatever the next location of the plate needs to be rotated to, for it to be under the vacuum. It looks at its last known location
//  and calculates how much it needs to rotate to get that location under the vacuum. It then figures out which is faster, rotating clockwise or
//  counter clockwise, and then finally actually rotates the plate accordingly.
void spinPlate(int nextLocation)
{
    int stepsToGo = nextLocation - nemaCurrentPos;

    //  If the rotate is more than half the total steps possible, we can just spin the plate in the opposite direction
    if (abs(stepsToGo) > NEMA_HALF_REV_LOCATION)
    {
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
    if (stepsToGo < 0)
    {
        stepsToGo *= -1;
        nemaDirection = NEMA_COUNTER_CLOCKWISE;
        nemaCurrentPos -= stepsToGo;
    }
    else    //  If it's positive, then set the direction to NEMA_CLOCKWISE (0) and update the currentPos tracker
    {
        nemaDirection = NEMA_CLOCKWISE;
        nemaCurrentPos += stepsToGo;
    }

    //  Set the direction pin and then start the rotate. The motor works by rotating a step every time it senses a high transition, so in every
    //  iteration of the loop we start by sending a HIGH (1) signal to get it to step, then send LOW (0) so that it's ready for the next step.
    //  The "nemaDelayTime" is needed so there is some delay between the pin getting the high and low signal. This also allows us to control the
    //  speed at which the plate actually spins.
    digitalWrite(PIN_NEMA_DIRECTION, nemaDirection);
    for (int x = 0; x < stepsToGo; x++)
    {
        digitalWrite(PIN_NEMA_STEP, HIGH);
        delayMicroseconds(nemaDelayTime);
        digitalWrite(PIN_NEMA_STEP, LOW);
        delayMicroseconds(nemaDelayTime);
    }
    return;
}

// Arduino runs in an infinite loop through this function, which is where the main state machine of this device will be
void loop()
{
    switch (currentState)
    {
        // The initialization just completed in the setup method, so all we do here is switch to the next state
        case STATE_INITIALIZE:
        {
            currentState = nextState;
            break;
        }
        // In the read data state, we wait until there is data available on the serial port and then try to read it on
        case STATE_READ_DATA:
        {
            if (Serial.available())
            {
                //  Read the data, if it was a success then nextState will have changed to either refill or dispense, otherwise it is still the
                //  read data state and we stay in read data
                byte success = readData();
                currentState = nextState;

                // Send response back to pi, letting it know whether the data we got was valid
                Serial.println(success);
            }
            break;
        }
        // This is the beginning of the refill
        case STATE_REFILL:
        {
            break;
        }
        // This is the beginning of the dispense
        case STATE_DISPENSE:
        {
            break;
        }
    }
    delay(500);    // REMOVE THIS DELAY FOR FINAL PRODUCT, ONLY SLOWS THINGS DOWN FOR DEBUGGING
}