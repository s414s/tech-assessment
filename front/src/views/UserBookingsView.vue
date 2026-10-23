<script setup lang="ts">
import type { GetUsersBookingsResponse, UserBooking } from '@/types/responseTypes';
import { GET } from '@/helpers/apiClient';
import { ref, watch } from 'vue';
import UserBookings from '@/components/UserBookings.vue';
import UserSelect from '@/components/UserSelect.vue';

const PAGE_SIZE = 10

const userId = ref<number | null>(null)

const page = ref(1)
const totalPages = ref(0)
const bookings = ref<UserBooking[]>([])
const loading = ref(false)
const error = ref('')

async function fetchBookings() {
  if (userId.value === null) {
    bookings.value = []
    totalPages.value = 0
    return
  }

  loading.value = true
  error.value = ''

  const params = new URLSearchParams({ page: String(page.value), limit: String(PAGE_SIZE) })

  try {
    const response: GetUsersBookingsResponse = await GET(`api/bookings/user/${userId.value}?${params}`)
    bookings.value = response.bookings.items.map(booking => ({
      id: booking.id,
      locationId: booking.locationId,
      locationName: booking.locationName,
      itemType: booking.itemName,
      from: new Date(booking.from),
      to: new Date(booking.to),
    }))
    totalPages.value = response.bookings.totalPages
  } catch {
    error.value = 'Could not load bookings'
    bookings.value = []
    totalPages.value = 0
  } finally {
    loading.value = false
  }
}

// Changing the user restarts from the first page
watch(userId, () => {
  if (page.value === 1) fetchBookings()
  else page.value = 1
})
watch(page, fetchBookings)
</script>

<template>
  <p>Bookings by user</p>

  <v-row>
    <v-col cols="12" md="6">
      <UserSelect v-model="userId" />
    </v-col>
  </v-row>

  <p v-if="userId === null" class="text-center text-medium-emphasis pa-6">
    Select filters
  </p>

  <template v-else>
    <v-progress-linear v-if="loading" indeterminate class="mb-4" />
    <v-alert v-if="error" type="error" variant="tonal" class="mb-4">{{ error }}</v-alert>

    <UserBookings v-if="!loading && !error" :bookings="bookings" />

    <v-pagination
      v-if="totalPages > 1"
      v-model="page"
      :length="totalPages"
      :disabled="loading"
    />
  </template>
</template>
