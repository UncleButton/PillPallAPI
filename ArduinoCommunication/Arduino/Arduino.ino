// Arduino.ino

// This is the code that will go on the Arduino. It will take in the request from the ArduinoCommunicator and control the motors, vacuum, sensors,
// and other parts of the device according to the particular request.

// Justin Feldmann, February 2024

const byte REQUEST_OFFSET = 6;  // The two-bit request needs to be shifted to bits 6 and 7 of the first byte in the message
const byte RECEIVED_VALID_MESSAGE = 0;
const byte RECEIVED_INVALID_MESSAGE = -1;
byte *data; // The array that will hold the data, allocated when it is read in
byte request = 0;   // The type of request received fromt the pi
byte dataLength = 0;    // The length of the data in bytes, used to allocate the data array

// Setup function, runs once when the Arduino first powers on, configuring pins and serial communication
void setup()
{
    delay(3000);    // The delay is needed here, otherwise the Arduino starts running code early
    Serial.begin(9600); // Open the serial port for communication
}

void readData()
{
    //  Read in the information byte, and split it into the request and dataLength
    byte info = 0;
    Serial.readBytes(&info, 1);
    request = (info & 0b11000000) >> REQUEST_OFFSET;
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
    if (sum == checkSum)
        Serial.println(RECEIVED_VALID_MESSAGE);
    else
        Serial.println(RECEIVED_INVALID_MESSAGE);
}

// Arduino runs in an infinite loop through this function, which is where the main state machine of this device will be
void loop()
{
    if (Serial.available())
    {
        readData();
    }
    delay(1000);
}