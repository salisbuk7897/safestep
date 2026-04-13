#include "driver/uart.h"
#include "driver/gpio.h"

#define TXD_PIN (GPIO_NUM_1)
#define RXD_PIN (GPIO_NUM_3)

void app_main() {

    // Init UART
    const uart_config_t uart_config = {
        .baud_rate = 115200,
        .data_bits = UART_DATA_8_BITS,
        .parity    = UART_PARITY_DISABLE,
        .stop_bits = UART_STOP_BITS_1,
        .flow_ctrl = UART_HW_FLOWCTRL_DISABLE
    };
    uart_param_config(UART_NUM_0, &uart_config);
    
    uart_set_pin(UART_NUM_0, TXD_PIN, RXD_PIN, UART_PIN_NO_CHANGE, UART_PIN_NO_CHANGE);
    uart_driver_install(UART_NUM_0, 2048, 0, 0, NULL, 0);

    // Simulate input and send through UART
    int sim_rssi = -20;
    while(1){
        char data[256];
        snprintf(data, 256, "ID=TAG001;NAME=John;BAT=87;Z1=0;Z2=1;Z3=0;RSSI=%d;TEMP=36.5;PING=1\r\n", sim_rssi);
        sim_rssi -= 1;
        uart_write_bytes(UART_NUM_0, data, strlen(data));
        vTaskDelay(2000/portTICK_PERIOD_MS);
    }
    
}
