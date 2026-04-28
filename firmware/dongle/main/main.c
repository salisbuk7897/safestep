#include <stdio.h>
#include <stdlib.h>
#include "freertos/idf_additions.h"
#include "freertos/task.h"
#include "freertos/event_groups.h"
#include "nvs_flash.h"
#include "esp_log.h"
#include "nimble/nimble_port.h"
#include "host/ble_hs.h"
#include "portmacro.h"
#include "services/gap/ble_svc_gap.h"

uint8_t ble_addr_type;
#define SAMPLES_FOR_AVERAGE 10
int rssi_samples[SAMPLES_FOR_AVERAGE] = {};
int next_sample_index = 0;
int taken_samples = 0; // max = SAMPLES_FOR_AVERAGE
int cumulative_rssi = 0;

SemaphoreHandle_t mutex;

static int ble_gap_event(struct ble_gap_event *event, void *arg){
    struct ble_hs_adv_fields fields;

    switch (event->type){
        case BLE_GAP_EVENT_DISC:
            ESP_LOGI("GAP", "GAP EVENT DISCOVERY");
            ble_hs_adv_parse_fields(&fields, event->disc.data, event->disc.length_data);
            if (fields.name_len > 0){

				xSemaphoreTake(mutex, portMAX_DELAY);
				{
					// Smooth RSSI with moving averages and send info to PC
					cumulative_rssi -= rssi_samples[next_sample_index];
					rssi_samples[next_sample_index] = event->disc.rssi;
					cumulative_rssi += rssi_samples[next_sample_index];

					next_sample_index = (next_sample_index+1)%SAMPLES_FOR_AVERAGE;

					taken_samples = (taken_samples+1 < SAMPLES_FOR_AVERAGE)? taken_samples+1: SAMPLES_FOR_AVERAGE;

					int smoothed_rssi = cumulative_rssi/taken_samples;
					printf("Name: %.*s\n", fields.name_len, fields.name);
					printf("RSSI: %d\n", smoothed_rssi);
				}
				xSemaphoreGive(mutex);
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
	mutex = xSemaphoreCreateMutex();

	nvs_flash_init();
    nimble_port_init();

    ble_svc_gap_init();
    ble_svc_gap_device_name_set("BLE-Scan-Client");

    ble_hs_cfg.sync_cb = ble_app_on_sync;
    nimble_port_run();
}
