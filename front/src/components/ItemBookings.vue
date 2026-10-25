
<script setup lang="ts">
import type { ItemBookingDto } from '@/types/responseTypes';
import { computed, onUnmounted, ref } from 'vue'

// Slots come already formed from the backend; this only decides how each one looks
const props = withDefaults(defineProps<{
  slots?: ItemBookingDto[]
}>(), {
  slots: () => [],
})

// Refreshed every minute so slots turn unavailable as time passes
const now = ref(new Date())
const timer = setInterval(() => (now.value = new Date()), 60_000)
onUnmounted(() => clearInterval(timer))

const rows = computed(() => props.slots.map(slot => {
  const from = new Date(slot.from)
  return {
    key: slot.from,
    label: from.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' }),
    occupied: slot.userId !== null,
    // Bookings can only start from the next slot, so a slot already started is gone too
    past: from < now.value,
  }
}))
</script>

<template>
  <div class="booking-slots">
    <div
      v-for="slot in rows"
      :key="slot.key"
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
