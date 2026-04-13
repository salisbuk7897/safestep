# Because the ESP32 sends logs through the UART channel, some undesired messages will appear.
# To counteract some of this, one can set (on KConfig, on the firmware) the channel for console output option to 
# none (or in general something != UART0). But after doing this, there will still be undesired logs, sent from the bootloader.
# These bootloader logs can be turned off grounding GPIO15 with the appropiate resistor (see this on esp-idf examples/system/deep_sleep_wake_stub/README.md); 
# another (though more hacky) option would be to ignore all messages that don't begin with "ID=".

import serial

PORT = "COM6"
BAUDRATE = 115200
IGNORE_ESP32_BOOT_LOGS = False # Hack

ser = serial.Serial(PORT, BAUDRATE, timeout=1)

while True:
    try: # Get and print the message
        message = ser.readline().decode('utf-8').strip()
        if message and (message[0:3] == "ID=" or not IGNORE_ESP32_BOOT_LOGS):
            print(message)
    except Exception as e1: # If the device is not conected/restarting or something along those lines, try to reconnect 
        print(f"Error: {e1}")
        try:
            ser = serial.Serial(PORT, BAUDRATE, timeout=1)
        except Exception as e2:
            print(f"Error: {e2}")