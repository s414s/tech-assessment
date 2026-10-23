<script setup lang="ts">
import { formatDate } from '@/helpers/dateHelpers'
import type { UserBooking } from '@/types/responseTypes'
import { computed } from 'vue'

const props = defineProps<{
  bookings: UserBooking[]
}>()

const groupedBookings = computed<Record<string, UserBooking[]>>(() => {
  const groups: Record<string, UserBooking[]> = {}

  for (const booking of props.bookings) {
    const date = new Date(booking.from).toLocaleDateString('en-CA')

    if (!groups[date]) {
      groups[date] = []
    }

    groups[date].push(booking)
  }

  return groups
})
</script>

<template>
  <div class="user-bookings">
    <div v-if="!bookings.length" class="text-center pa-6">
      <v-icon
        icon="mdi-calendar-blank"
        size="48"
        color="grey"
      />
      <p class="mt-3 text-medium-emphasis">
        You have no bookings.
      </p>
    </div>

    <div
      v-for="(dayBookings, date) in groupedBookings"
      :key="date"
      class="mb-6"
    >
      <h3 class="mb-3 text-capitalize">
        {{ formatDate(String(date)) }}
      </h3>

      <v-card
        v-for="booking in dayBookings"
        :key="booking.id"
        class="mb-2 booking-card"
        variant="outlined"
      >
        <v-card-text class="d-flex align-center ga-4">
          <div class="time-column">
            <div class="font-weight-bold">
                From:
                {{ booking.from.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' }) }}
            </div>
            <div class="text-medium-emphasis">
                To:
              {{ booking.to.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' }) }}
            </div>
          </div>

          <v-divider vertical />

          <div class="flex-grow-1">
            <div class="font-weight-medium">
              {{ booking.locationName }}
            </div>
            <div class="text-caption text-medium-emphasis">
              {{ booking.itemType }}
            </div>
          </div>

          <v-icon
            icon="mdi-calendar-check"
            color="primary"
          />
        </v-card-text>
      </v-card>
    </div>
  </div>
</template>

<style scoped>
.user-bookings {
  width: 100%;
}

.booking-card {
  border-radius: 8px;
  border-left: 4px solid rgb(var(--v-theme-primary));
}

.time-column {
  min-width: 52px;
  font-size: 14px;
}
</style>
