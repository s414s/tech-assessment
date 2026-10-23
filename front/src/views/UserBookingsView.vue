<script setup lang="ts">
import type { Booking, UserBooking } from '@/types/responseTypes';
import { useRouter } from 'vue-router';
import { ref } from 'vue';
import UserBookings from '@/components/UserBookings.vue';

const router = useRouter();
const props = defineProps<{ bookings: Booking[]; }>();
// const { user } = useUserStore();

const bookings = ref<UserBooking[]>([
  {
    id: 1,
    from: new Date('2026-10-23T08:30:00'), 
    to: new Date('2026-10-23T09:30:00'), 
    locationId: 1,
    locationName: 'Meeting Room A',
    itemType: 'Room 1',
  },
  {
    id: 2,
    from: new Date('2026-10-24T08:30:00'), 
    to: new Date('2026-10-24T09:30:00'), 
    locationId: 2,
    locationName: 'Meeting Room B',
    itemType: 'Room 1',
  },
])
</script>

<template>
    <UserBookings :bookings="bookings" />
</template>