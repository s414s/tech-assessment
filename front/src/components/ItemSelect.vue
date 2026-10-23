<script setup lang="ts">
import { GET } from '@/helpers/apiClient'
import type { GetFilteredItemsResponse, ItemDto } from '@/types/responseTypes'
import { ref, watch } from 'vue'

const props = defineProps<{
  locationId: number | null // Items are listed for this location only
}>()

const model = defineModel<number | null>({ default: null })

const items = ref<ItemDto[]>([])
const loading = ref(false)
const error = ref('')

watch(() => props.locationId, async locationId => {
  items.value = []
  error.value = ''

  if (locationId === null) {
    model.value = null
    return
  }

  loading.value = true
  try {
    const params = new URLSearchParams({ locationId: String(locationId), page: '1', limit: '100' })
    const response: GetFilteredItemsResponse = await GET(`api/items?${params}`)
    items.value = response.items.items
    // Keep the selection only if it belongs to the new location
    if (!items.value.some(x => x.id === model.value)) model.value = null
  } catch {
    error.value = 'Could not load items'
  } finally {
    loading.value = false
  }
}, { immediate: true })
</script>

<template>
  <v-select
    v-model="model"
    :items="items"
    item-title="name"
    item-value="id"
    label="Item"
    :loading="loading"
    :error-messages="error"
    :disabled="locationId === null"
    clearable
  />
</template>
