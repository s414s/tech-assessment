
<script setup lang="ts">
import { customFormatTime, toDate, toMinutes } from '@/helpers/dateHelpers';
import type { Booking } from '@/types/responseTypes';
import { computed, ref } from 'vue'

interface TimeSlot {
  start: number
  end: number
  label: string
  occupied: boolean
}

const props = withDefaults(defineProps<{
  date: string // YYYY-MM-DD
  startTime: string // HH:mm
  endTime: string // HH:mm
  bookings?: Booking[]
}>(), {
  bookings: () => [],
})

const STEP = 15

const slots = computed<TimeSlot[]>(() => {
  const start = toMinutes(props.startTime)
  const end = toMinutes(props.endTime)

  if (end <= start) return []

  const result: TimeSlot[] = []

  for (let minute = start; minute + STEP <= end; minute += STEP) {
    const slotStart = toDate(minute, props.date)
    const slotEnd = toDate(minute + STEP, props.date)

    const occupied = props.bookings.some(booking => {
      const from = new Date(booking.from)
      const to = new Date(booking.to)

      return from < slotEnd && to > slotStart
    })

    result.push({
      start: minute,
      end: minute + STEP,
      label: customFormatTime(minute),
      occupied,
    })
  }

  return result
})
</script>

<template>
  <div class="booking-slots">
    <div
      v-for="slot in slots"
      :key="slot.start"
      class="slot"
      :class="{ occupied: slot.occupied }"
    >
      <span class="slot-time">
        {{ slot.label }}
      </span>

      <span class="slot-status">
        {{ slot.occupied ? 'Occupied' : 'Available' }}
      </span>
    </div>
  </div>
</template>

<style scoped>
.booking-slots {
  display: flex;
  flex-direction: column;
  gap: 4px;
  width: 100%;
}

.slot {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border: 1px solid #93c5fd;
  border-radius: 6px;
  background: #dbeafe;
  color: #1d4ed8;
  font-size: 14px;
}

.slot.occupied {
  background: #fee2e2;
  border-color: #fca5a5;
  color: #b91c1c;
}

.slot-time {
  font-weight: 600;
  font-variant-numeric: tabular-nums;
}

.slot-status {
  font-size: 12px;
}
</style>
