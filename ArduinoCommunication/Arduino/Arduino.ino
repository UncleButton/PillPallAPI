// Arduino.ino

// This is the code that will go on the Arduino. It will take in the request from the ArduinoCommunicator and control the motors, vacuum, sensors,
// and other parts of the device according to the particular request.

// Setup function, runs once when the Arduino first powers on, configuring pins and serial communication
void setup()
{
    delay(3000);    // delay is needed here, otherwise the Arduino starts running code early
    Serial.begin(9600); // Open the serial port for communication
}

// Arduino runs in an infinite loop through this function, which is where the main state machine of this device will be
void loop()
{
    if (Serial.available())
    {
        // byte info, sum = 0;
        // Serial.readBytes(&info, 1);
        byte sum = 0;
        int info = Serial.read();
        Serial.print((int)info);
        int request = (info & 0b11000000);
        Serial.print("Request is ");
        Serial.print(request);
        byte dataLength = info & 0b00111111;
        Serial.print(" , data length is ");
        Serial.print((int)dataLength);
        byte placeHolder = 0;
        Serial.print(", data is ");
        for (int i = 0; i < 6; i++)
        {
            Serial.readBytes(&placeHolder, 1);
            Serial.print((int)placeHolder);
            sum += placeHolder;
        }
        Serial.readBytes(&placeHolder, 1);
        Serial.print(", and checksum is ");
        Serial.print(placeHolder);
        // sum += Serial.read();
        if (sum == 0)
            Serial.println("Recieved correct message");
        else
            Serial.println("Received CORRUPTED message");
    }
    delay(1000);
}