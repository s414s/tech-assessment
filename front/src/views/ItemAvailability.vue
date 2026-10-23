<script setup lang="ts">
import CreateBookingDialog from '@/components/CreateBookingDialog.vue';
import DateSelect from '@/components/DateSelect.vue';
import ItemBookings from '@/components/ItemBookings.vue';
import ItemTypeSelect from '@/components/ItemTypeSelect.vue';
import LocationSelect from '@/components/LocationSelect.vue';
import { GET } from '@/helpers/apiClient';
import { firstBookableDate } from '@/helpers/dateHelpers';
import type { GetFilteredItemsResponse, ItemDto } from '@/types/responseTypes';
import { computed, ref, watch } from 'vue'

const PAGE_SIZE = 5

const itemType = ref<number | null>(null)
const locationId = ref<number | null>(null)
const today = new Date().toLocaleDateString('en-CA')
const date = ref(firstBookableDate())
const filtersSelected = computed(() => itemType.value !== null && locationId.value !== null && !!date.value)

const page = ref(1)
const totalPages = ref(0)
const items = ref<ItemDto[]>([])
const loading = ref(false)
const error = ref('')

const bookingDialogOpen = ref(false)
const itemToBook = ref<ItemDto | null>(null)
const bookingCreated = ref(false)

// Without an item the dialog lets the user pick location and item
function openBookingDialog(item: ItemDto | null) {
  itemToBook.value = item
  bookingDialogOpen.value = true
}

function onBookingCreated() {
  bookingCreated.value = true
  fetchItems()
}

// Only items with at least one free slot that day are returned, each with that day's bookings
async function fetchItems() {
  if (!filtersSelected.value) {
    items.value = []
    totalPages.value = 0
    return
  }

  loading.value = true
  error.value = ''

  const params = new URLSearchParams({ page: String(page.value), limit: String(PAGE_SIZE), availableOnDate: date.value })
  if (itemType.value !== null) params.set('type', String(itemType.value))
  if (locationId.value !== null) params.set('locationId', String(locationId.value))

  try {
    const response: GetFilteredItemsResponse = await GET(`api/items?${params}`)
    items.value = response.items.items
    totalPages.value = response.items.totalPages
  } catch {
    error.value = 'Could not load items'
    items.value = []
    totalPages.value = 0
  } finally {
    loading.value = false
  }
}

// Changing a filter restarts from the first page
watch([itemType, locationId, date], () => {
  if (page.value === 1) fetchItems()
  else page.value = 1
})
watch(page, fetchItems)

fetchItems()
</script>

<template>
  <main>
    <div class="d-flex align-center justify-space-between mb-4">
      <p>Bookings</p>
      <v-btn color="primary" variant="flat" @click="openBookingDialog(null)">Book</v-btn>
    </div>

    <v-row>
      <v-col cols="12" md="4">
        <ItemTypeSelect v-model="itemType" />
      </v-col>
      <v-col cols="12" md="4">
        <LocationSelect v-model="locationId" />
      </v-col>
      <v-col cols="12" md="4">
        <DateSelect v-model="date" :min="today" />
      </v-col>
    </v-row>

    <v-progress-linear v-if="loading" indeterminate class="mb-4" />
    <v-alert v-if="error" type="error" variant="tonal" class="mb-4">{{ error }}</v-alert>

    <p v-if="!filtersSelected" class="text-center text-medium-emphasis pa-6">
      Select filters
    </p>

    <p v-else-if="!loading && !error && !items.length" class="text-center text-medium-emphasis pa-6">
      No items found
    </p>

    <template v-else>
      <v-card v-for="item in items" :key="item.id" class="mb-4" variant="outlined">
        <v-card-title>{{ item.name }}</v-card-title>
        <v-card-subtitle>{{ item.locationName }}</v-card-subtitle>
        <v-card-actions>
          <v-btn color="primary" variant="flat" @click="openBookingDialog(item)">Book</v-btn>
        </v-card-actions>
        <v-card-text>
          <ItemBookings
            :date="date"
            :start-time="item.openingTime"
            :end-time="item.closingTime"
            :bookings="item.bookings"
          />
        </v-card-text>
      </v-card>
    </template>

    <v-pagination
      v-if="totalPages > 1"
      v-model="page"
      :length="totalPages"
      :disabled="loading"
    />

    <CreateBookingDialog
      v-model="bookingDialogOpen"
      :item="itemToBook"
      :date="date"
      @created="onBookingCreated"
    />

    <v-snackbar v-model="bookingCreated" color="success" timeout="3000">
      Booking created
    </v-snackbar>
  </main>
</template>
