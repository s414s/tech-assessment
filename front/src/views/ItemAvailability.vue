<script setup lang="ts">
import CreateBookingDialog from '@/components/CreateBookingDialog.vue';
import DateSelect from '@/components/DateSelect.vue';
import ItemBookings from '@/components/ItemBookings.vue';
import ItemTypeSelect from '@/components/ItemTypeSelect.vue';
import LocationSelect from '@/components/LocationSelect.vue';
import { GET } from '@/helpers/apiClient';
import type { Booking, GetFilteredItemsResponse, GetItemBookingsResponse, ItemDto } from '@/types/responseTypes';
import { computed, ref, watch } from 'vue'

const PAGE_SIZE = 5

const itemType = ref<number | null>(null)
const locationId = ref<number | null>(null)
const date = ref(new Date().toLocaleDateString('en-CA'))
const filtersSelected = computed(() => itemType.value !== null && locationId.value !== null && !!date.value)

const page = ref(1)
const totalPages = ref(0)
const items = ref<ItemDto[]>([])
const bookingsByItem = ref<Record<number, Booking[]>>({})
const loading = ref(false)
const error = ref('')

const bookingDialogOpen = ref(false)
const itemToBook = ref<ItemDto | null>(null)
const bookingCreated = ref(false)

function openBookingDialog(item: ItemDto) {
  itemToBook.value = item
  bookingDialogOpen.value = true
}

function onBookingCreated() {
  bookingCreated.value = true
  fetchItems()
}

async function fetchItemBookings(item: ItemDto): Promise<Booking[]> {
  const params = new URLSearchParams({ from: `${date.value}T00:00:00`, to: `${date.value}T23:59:59` })
  try {
    const response: GetItemBookingsResponse = await GET(`api/bookings/item/${item.id}?${params}`)
    return response.bookings.map((booking, index) => ({
      id: index,
      locationId: item.locationId,
      locationName: item.locationName,
      itemType: item.name,
      userId: booking.userId,
      from: new Date(booking.from),
      to: new Date(booking.to),
    }))
  } catch (e) {
    console.error(e)
    return []
  }
}

async function fetchItems() {
  if (!filtersSelected.value) {
    items.value = []
    totalPages.value = 0
    bookingsByItem.value = {}
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

    const bookings = await Promise.all(items.value.map(fetchItemBookings))
    bookingsByItem.value = Object.fromEntries(items.value.map((item, i) => [item.id, bookings[i] ?? []]))
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

    <v-progress-linear v-if="loading" indeterminate class="mb-4" />
    <v-alert v-if="error" type="error" variant="tonal" class="mb-4">{{ error }}</v-alert>

    <p v-if="!filtersSelected" class="text-center text-medium-emphasis pa-6">
      Select filters
    </p>

    <p v-else-if="!loading && !error && !items.length" class="text-center text-medium-emphasis pa-6">
      No items match the selected filters.
    </p>

    <v-card v-for="item in items" :key="item.id" class="mb-4" variant="outlined">
      <v-card-title>{{ item.name }}</v-card-title>
      <v-card-subtitle>{{ item.locationName }}</v-card-subtitle>
      <v-card-text>
        <ItemBookings
          :date="date"
          start-time="08:00"
          end-time="18:00"
          :bookings="bookingsByItem[item.id] ?? []"
        />
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn color="primary" variant="flat" @click="openBookingDialog(item)">Book</v-btn>
      </v-card-actions>
    </v-card>

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
