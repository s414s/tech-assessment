
<script setup lang="ts">
import { formatTime, toDate, toMinutes } from '@/helpers/dateHelpers';
import type { Booking } from '@/types/responseTypes';
import { computed, onUnmounted, ref } from 'vue'

interface TimeSlot {
  start: number
  end: number
  label: string
  occupied: boolean
  past: boolean
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

// Refreshed every minute so slots turn unavailable as time passes
const now = ref(new Date())
const timer = setInterval(() => (now.value = new Date()), 60_000)
onUnmounted(() => clearInterval(timer))

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
      label: formatTime(minute),
      occupied,
      // Bookings can only start from the next slot, so a slot already started is gone too
      past: slotStart < now.value,
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
      :class="{ occupied: slot.occupied, past: slot.past && !slot.occupied }"
    >
      <span class="slot-time">
        {{ slot.label }}
      </span>

      <span class="slot-status">
        {{ slot.occupied ? 'Occupied' : slot.past ? 'Not available' : 'Available' }}
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

.slot.past {
  background: #f3f4f6;
  border-color: #d1d5db;
  color: #6b7280;
}

.slot-time {
  font-weight: 600;
  font-variant-numeric: tabular-nums;
}

.slot-status {
  font-size: 12px;
}
</style>
