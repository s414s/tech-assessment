<script setup lang="ts">
import { formatDate } from '@/helpers/dateHelpers'
import type { UserBooking } from '@/types/responseTypes'

defineProps<{
  bookings: UserBooking[]
}>()
</script>

<template>
  <div class="user-bookings">
    <div v-if="!bookings.length" class="empty-state">
      <v-icon
        icon="mdi-calendar-blank"
        size="48"
        color="grey"
      />
      <p class="empty-message">
        You have no bookings.
      </p>
    </div>

    <v-card
      v-for="booking in bookings"
      :key="booking.id"
      class="booking-card"
      variant="outlined"
    >
      <v-card-text class="booking-content">
        <div class="time-column">
          <div class="booking-date">
            {{ formatDate(booking.from.toLocaleDateString('en-CA')) }}
          </div>
          <div class="booking-from">
            From:
            {{ booking.from.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' }) }}
          </div>
          <div class="secondary-text">
            To:
            {{ booking.to.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' }) }}
          </div>
        </div>

        <v-divider vertical />

        <div class="booking-details">
          <div class="location-name">
            {{ booking.locationName }}
          </div>
          <div class="item-type secondary-text">
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
</template>

<style scoped>
.user-bookings {
  width: 100%;
}

.empty-state {
  padding: 24px;
  text-align: center;
}

.empty-message {
  margin-top: 12px;
  opacity: 0.7;
}

.booking-card {
  margin-bottom: 8px;
  border-radius: 8px;
  border-left: 4px solid rgb(var(--v-theme-primary));
}

.booking-content {
  display: flex;
  align-items: center;
  gap: 16px;
}

.time-column {
  min-width: 52px;
  font-size: 14px;
}

.booking-date {
  font-size: 12px;
  text-transform: capitalize;
}

.booking-from {
  font-weight: 700;
}

.booking-details {
  flex-grow: 1;
}

.location-name {
  font-weight: 500;
}

.item-type {
  font-size: 12px;
}

.secondary-text {
  opacity: 0.7;
}
</style>
