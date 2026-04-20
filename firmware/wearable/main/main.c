/* STD APIs */
#include <stdbool.h>
#include <stdio.h>
#include <string.h>

/* ESP APIs */
#include "esp_log.h"
#include "nvs_flash.h"
#include "sdkconfig.h"

/* FreeRTOS APIs */
#include <freertos/FreeRTOS.h>
#include <freertos/task.h>

/* NimBLE stack APIs */
#include "host/ble_hs.h"
#include "host/ble_uuid.h"
#include "host/util/util.h"
#include "nimble/ble.h"
#include "nimble/nimble_port.h"
#include "nimble/nimble_port_freertos.h"
#include "esp_nimble_hci.h"

/* NimBLE GAP APIs */
#include "services/gap/ble_svc_gap.h"

#define DEVICE_NAME "SafeStep-WB"
uint8_t ble_addr_type;

// The application
void ble_app_on_sync(void){
    ble_hs_id_infer_auto(0, &ble_addr_type);
    
    struct ble_hs_adv_fields adv_fields = {0};
    adv_fields.flags = BLE_HS_ADV_F_DISC_GEN | BLE_HS_ADV_F_BREDR_UNSUP;
    adv_fields.name = (uint8_t *) ble_svc_gap_device_name();
    adv_fields.name_len = strlen((const char*)adv_fields.name);
    adv_fields.name_is_complete = 1;
    adv_fields.tx_pwr_lvl = BLE_HS_ADV_TX_PWR_LVL_AUTO;
    adv_fields.tx_pwr_lvl_is_present = 1;
    ble_gap_adv_set_fields(&adv_fields);

    struct ble_gap_adv_params adv_params = {0};
    adv_params.itvl_max = 160;
    adv_params.itvl_min = 160;
    adv_params.conn_mode = BLE_GAP_CONN_MODE_NON;
    adv_params.disc_mode = BLE_GAP_DISC_MODE_GEN;
    ble_gap_adv_start(ble_addr_type, NULL, BLE_HS_FOREVER, &adv_params, NULL, NULL);
}

void app_main(){
    nvs_flash_init();
    nimble_port_init();   

    ble_svc_gap_init();                        
    ble_svc_gap_device_name_set(DEVICE_NAME);
    
    ble_hs_cfg.sync_cb = ble_app_on_sync;      
    nimble_port_run();
}
