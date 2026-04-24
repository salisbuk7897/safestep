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
#include "esp_bt.h"

/* NimBLE GAP APIs */
#include "services/gap/ble_svc_gap.h"

/* Deep Sleep */
#include "esp_sleep.h"

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
    ble_gap_adv_set_fields(&adv_fields);
    esp_ble_tx_power_set(ESP_BLE_PWR_TYPE_ADV, ESP_PWR_LVL_P9);

    struct ble_gap_adv_params adv_params = {0};
    adv_params.itvl_max = 160;
    adv_params.itvl_min = 160;
    adv_params.conn_mode = BLE_GAP_CONN_MODE_NON;
    adv_params.disc_mode = BLE_GAP_DISC_MODE_GEN;
    ble_gap_adv_start(ble_addr_type, NULL, BLE_HS_FOREVER, &adv_params, NULL, NULL);
}

void timer_stop_nimble_port(TimerHandle_t xTimer){
	nimble_port_stop();
}

void app_main(){
    nvs_flash_init();
    nimble_port_init();

    ble_svc_gap_init();
    ble_svc_gap_device_name_set(DEVICE_NAME);

	TimerHandle_t t = xTimerCreate("timer_stop_nimble_port", pdMS_TO_TICKS(5000), pdFALSE, 0, timer_stop_nimble_port);
	xTimerStart(t, 0);

	ble_hs_cfg.sync_cb = ble_app_on_sync;
	nimble_port_run();

	// After nimble stops
	nimble_port_deinit();
	uint32_t wakeup_time_ms = 5000;
	ESP_ERROR_CHECK(esp_sleep_enable_timer_wakeup(wakeup_time_ms * 1000));
	esp_deep_sleep_start();
}
