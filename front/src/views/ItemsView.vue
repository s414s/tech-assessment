<script setup lang="ts">
import ItemTypeSelect from '@/components/ItemTypeSelect.vue';
import LocationSelect from '@/components/LocationSelect.vue';
import { GET } from '@/helpers/apiClient';
import type { GetFilteredItemsResponse, ItemDto } from '@/types/responseTypes';
import { computed, ref, watch } from 'vue'

const PAGE_SIZE = 10

const itemType = ref<number | null>(null)
const locationId = ref<number | null>(null)
const filtersSelected = computed(() => itemType.value !== null && locationId.value !== null)

const page = ref(1)
const totalPages = ref(0)
const items = ref<ItemDto[]>([])
const loading = ref(false)
const error = ref('')

async function fetchItems() {
  if (!filtersSelected.value) {
    items.value = []
    totalPages.value = 0
    return
  }

  loading.value = true
  error.value = ''

  const params = new URLSearchParams({ page: String(page.value), limit: String(PAGE_SIZE) })
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
watch([itemType, locationId], () => {
  if (page.value === 1) fetchItems()
  else page.value = 1
})
watch(page, fetchItems)

fetchItems()
</script>

<template>
  <main>
    <p>Items</p>

    <v-row>
      <v-col cols="12" md="6">
        <ItemTypeSelect v-model="itemType" />
      </v-col>
      <v-col cols="12" md="6">
        <LocationSelect v-model="locationId" />
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

    <v-list v-if="items.length" lines="two">
      <v-list-item
        v-for="item in items"
        :key="item.id"
        :title="item.name"
        :subtitle="item.locationName"
      />
    </v-list>

    <v-pagination
      v-if="totalPages > 1"
      v-model="page"
      :length="totalPages"
      :disabled="loading"
    />
  </main>
</template>
