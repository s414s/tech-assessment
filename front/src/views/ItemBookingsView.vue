<script setup lang="ts">
import DateSelect from '@/components/DateSelect.vue';
import ItemBookings from '@/components/ItemBookings.vue';
import ItemTypeSelect from '@/components/ItemTypeSelect.vue';
import LocationSelect from '@/components/LocationSelect.vue';
import type { Booking } from '@/types/responseTypes';
import { ref } from 'vue'

const itemType = ref<number | null>(null)
const locationId = ref<number | null>(null)
const date = ref('2026-10-23')

const bookings = ref<Booking[]>([
  {
    id: 1,
    locationId: 1,
    locationName: "Zaragoza",
    itemType: "Room 1",
    from: new Date('2026-10-23T08:30:00'), 
    to: new Date('2026-10-23T09:30:00'), 
    userId: 1,
  },
  {
    id: 2,
    locationId: 1,
    locationName: "Zaragoza",
    itemType: "Room 1",
    from: new Date('2026-10-23T10:15:00'),
    to: new Date('2026-10-23T11:00:00'),
    userId: 2,
  },
])
</script>

<template>
  <main>
    <p>Bookings</p>

    <v-row>
      <v-col cols="12" md="4">
        <ItemTypeSelect v-model="itemType" />
      </v-col>
      <v-col cols="12" md="4">
        <LocationSelect v-model="locationId" />
      </v-col>
      <v-col cols="12" md="4">
        <DateSelect v-model="date" />
      </v-col>
    </v-row>

      <ItemBookings
        :date="date"
        start-time="08:00"
        end-time="18:00"
        :bookings="bookings"
      />
  </main>
</template>
