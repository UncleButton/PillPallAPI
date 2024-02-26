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

byte *data; // The array that will hold the data, allocated when it is read in
byte request = 0;   // The type of request received fromt the pi
byte dataLength = 0;    // The length of the data in bytes, used to allocate the data array
byte currentState = STATE_INITIALIZE;   // Set current state to initializing, cannot be modified outside of "loop"
byte nextState = STATE_READ_DATA;   // The next state in the machine, can be modified outside of "loop"

// Setup function, runs once when the Arduino first powers on, configuring pins and serial communication
void setup()
{
    delay(3000);    // The delay is needed here, otherwise the Arduino starts running code early
    Serial.begin(9600); // Open the serial port for communication
    currentState = STATE_READ_DATA; // Now in the waiting for data state
}

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
        if (request == REQUEST_REFILL)
            nextState = STATE_REFILL;
        else if (request == REQUEST_DISPENSE)
            nextState = STATE_DISPENSE;
        else
            isValid = RECEIVED_INVALID_MESSAGE;
    }
    else
    {
        isValid = RECEIVED_INVALID_MESSAGE;
    }
    return isValid;
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