#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "freertos/event_groups.h"
#include "esp_event.h"
#include "nvs_flash.h"
#include "esp_log.h"
#include "esp_nimble_hci.h"
#include "nimble/nimble_port.h"
#include "nimble/nimble_port_freertos.h"
#include "host/ble_hs.h"
#include "services/gap/ble_svc_gap.h"
#include "sdkconfig.h"

uint8_t ble_addr_type;

static int ble_gap_event(struct ble_gap_event *event, void *arg){
    struct ble_hs_adv_fields fields;
    
    switch (event->type){
        case BLE_GAP_EVENT_DISC:
            ESP_LOGI("GAP", "GAP EVENT DISCOVERY");
            ble_hs_adv_parse_fields(&fields, event->disc.data, event->disc.length_data);
            if (fields.name_len > 0){
                printf("Name: %.*s\n", fields.name_len, fields.name);
                printf("RSSI: %d\n", event->disc.rssi);
            }
            break;
        default:
            printf("EVENT OF TYPE: %d\n", event->type);
    }
    return 0;
}

void ble_app_on_sync(void){
    ble_hs_id_infer_auto(0, &ble_addr_type);
    
    printf("Start scanning ...\n");

    struct ble_gap_disc_params disc_params;
    disc_params.filter_duplicates = 0;
    disc_params.passive = 0;
    disc_params.itvl = 0;
    disc_params.window = 0;
    disc_params.filter_policy = 0;
    disc_params.limited = 0;

    ble_gap_disc(ble_addr_type, BLE_HS_FOREVER, &disc_params, ble_gap_event, NULL);
}

void app_main(){
    nvs_flash_init();
    nimble_port_init();

    ble_svc_gap_init();
    ble_svc_gap_device_name_set("BLE-Scan-Client");
                       
    ble_hs_cfg.sync_cb = ble_app_on_sync;
    nimble_port_run();
}
